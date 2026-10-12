using System.ComponentModel;

namespace TeleVisionGuide
{
    public class TimeTableColumn
    {
        public TimeTableColumn(string name) => Name = name;

        public string Name { get; }
        public Image? HeaderImage { get; set; }
        public Color? HeaderBackColor { get; set; }
        public Color? BackColor { get; set; }
    }

    public class TimeTableControl : ScrollableControl
    {
        private sealed record TimeTableEntry(string Column, TimeSpan Start, TimeSpan Duration, TeleVisionEventControl Control);

        private readonly List<string> _columns = new();
        private readonly Dictionary<string, TimeTableColumn> _columnInfos = new();
        private Size _headerImageSize = new Size(24, 24);
        private readonly List<TimeTableEntry> _entries = new();

        private TimeSpan _startTime = TimeSpan.FromHours(6);
        private TimeSpan _endTime = TimeSpan.FromHours(24);
        private TimeSpan _rowInterval = TimeSpan.FromMinutes(30);
        private int _rowHeight = 60;
        private int _columnWidth = 250;
        private int _timeColumnWidth = 60;
        private int _headerHeight = 30;
        private Color _headerBackColor = Color.SteelBlue;
        private Color _headerForeColor = Color.White;
        private Font _headerFont = new Font("Segoe UI", 14f, FontStyle.Bold);
        private Color _gridLineColor = Color.LightGray;
        private readonly Dictionary<string, int> _columnIndexCache = new();

        public TimeTableControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
            AutoScroll = true;
            BackColor = Color.White;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IReadOnlyList<string> Columns => _columns;

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TimeSpan StartTime { get => _startTime; set { _startTime = value; UpdateLayout(); } }

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TimeSpan EndTime { get => _endTime; set { _endTime = value; UpdateLayout(); } }

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TimeSpan RowInterval
        {
            get => _rowInterval;
            set { _rowInterval = value > TimeSpan.Zero ? value : TimeSpan.FromMinutes(30); UpdateLayout(); }
        }

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RowHeight { get => _rowHeight; set { _rowHeight = Math.Max(10, value); UpdateLayout(); } }

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ColumnWidth { get => _columnWidth; set { _columnWidth = Math.Max(20, value); UpdateLayout(); } }

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TimeColumnWidth { get => _timeColumnWidth; set { _timeColumnWidth = Math.Max(0, value); UpdateLayout(); } }

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int HeaderHeight { get => _headerHeight; set { _headerHeight = Math.Max(0, value); UpdateLayout(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HeaderBackColor { get => _headerBackColor; set { _headerBackColor = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HeaderForeColor { get => _headerForeColor; set { _headerForeColor = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font HeaderFont { get => _headerFont; set { _headerFont = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color GridLineColor { get => _gridLineColor; set { _gridLineColor = value; Invalidate(); } }

        [Category("Layout")]
        [Description("Display size of column header images. Empty uses the image's original size.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size HeaderImageSize { get => _headerImageSize; set { _headerImageSize = value; Invalidate(); } }

        public TimeTableColumn AddColumn(string name, Image? headerImage = null, Color? headerBackColor = null, Color? backColor = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            if (_columns.Contains(name))
                throw new ArgumentException($"Column '{name}' already exists.", nameof(name));
            var column = new TimeTableColumn(name)
            {
                HeaderImage = headerImage,
                HeaderBackColor = headerBackColor,
                BackColor = backColor
            };
            _columns.Add(name);
            _columnInfos[name] = column;
            _columnIndexCache[name] = _columns.Count - 1;
            UpdateLayout();
            return column;
        }

        public TimeTableColumn GetColumn(string name)
        {
            if (!_columnInfos.TryGetValue(name, out var column))
                throw new ArgumentException($"Unknown column '{name}'.", nameof(name));
            return column;
        }

        public void SetColumnImage(string name, Image? image)
        {
            GetColumn(name).HeaderImage = image;
            Invalidate();
        }

        public void SetColumnColor(string name, Color? headerBackColor, Color? backColor = null)
        {
            var column = GetColumn(name);
            column.HeaderBackColor = headerBackColor;
            column.BackColor = backColor;
            Invalidate();
        }

        public void SetColumns(IEnumerable<string> names)
        {
            Clear();
            foreach (var name in names)
                AddColumn(name);
        }

        public TeleVisionEventControl AddEvent(string column, TimeSpan start, TimeSpan duration, TeleVisionEventControl? control = null)
        {
            if (!_columns.Contains(column))
                throw new ArgumentException($"Unknown column '{column}'.", nameof(column));

            control ??= new TeleVisionEventControl();
            _entries.Add(new TimeTableEntry(column, start, duration, control));
            Controls.Add(control);
            UpdateLayout();
            return control;
        }

        public void Clear()
        {
            SuspendLayout();
            foreach (var entry in _entries)
            {
                Controls.Remove(entry.Control);
                entry.Control.Dispose();
            }
            _entries.Clear();
            _columns.Clear();
            _columnInfos.Clear();
            _columnIndexCache.Clear();
            ResumeLayout();
            UpdateLayout();
        }

        private int RowCount => Math.Max(0, (int)Math.Ceiling((_endTime - _startTime) / _rowInterval));

        private double PixelsPerMinute => _rowHeight / _rowInterval.TotalMinutes;

        private void UpdateLayout()
        {
            AutoScrollMinSize = new Size(
                _timeColumnWidth + _columns.Count * _columnWidth + 1,
                _headerHeight + RowCount * _rowHeight + 1);

            SuspendLayout();
            int autoScrollX = AutoScrollPosition.X;
            int autoScrollY = AutoScrollPosition.Y;
            int headerHeightLocal = _headerHeight;
            double pixelPerMin = PixelsPerMinute;
            TimeSpan startTimeLocal = _startTime;

            foreach (var entry in _entries)
            {
                int columnIndex;
                if (!_columnIndexCache.TryGetValue(entry.Column, out columnIndex))
                {
                    columnIndex = _columns.IndexOf(entry.Column);
                    _columnIndexCache[entry.Column] = columnIndex;
                }

                int top = headerHeightLocal + (int)Math.Round((entry.Start - startTimeLocal).TotalMinutes * pixelPerMin);
                int height = Math.Max(1, (int)Math.Round(entry.Duration.TotalMinutes * pixelPerMin));
                int controlY = autoScrollY + top + 1;

                entry.Control.Bounds = new Rectangle(
                    autoScrollX + _timeColumnWidth + columnIndex * _columnWidth + 1,
                    controlY,
                    _columnWidth - 1,
                    height - 1);
            }
            ResumeLayout();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            int offsetX = AutoScrollPosition.X;
            int offsetY = AutoScrollPosition.Y;
            int headerOffsetY = offsetY;
            int totalWidth = _timeColumnWidth + _columns.Count * _columnWidth;
            int totalHeight = _headerHeight + RowCount * _rowHeight;
            int colCount = _columns.Count;
            int rowH = _rowHeight;
            int colW = _columnWidth;
            TimeSpan startTimeLocal = _startTime;
            double pixPerMin = PixelsPerMinute;

            using var gridPen = new Pen(_gridLineColor);
            using var headerBrush = new SolidBrush(_headerBackColor);
            Dictionary<Color, SolidBrush> brushCache = new();

            // Draw column backgrounds
            for (int col = 0; col < colCount; col++)
            {
                var info = _columnInfos[_columns[col]];
                if (info.BackColor is Color columnBack)
                {
                    if (!brushCache.TryGetValue(columnBack, out var brush))
                    {
                        brush = new SolidBrush(columnBack);
                        brushCache[columnBack] = brush;
                    }
                    g.FillRectangle(brush, offsetX + _timeColumnWidth + col * colW, offsetY + _headerHeight, colW, RowCount * rowH);
                }
            }

            // Draw grid lines for rows
            int rowCount = RowCount;
            for (int row = 0; row <= rowCount; row++)
            {
                int y = offsetY + _headerHeight + row * rowH;
                g.DrawLine(gridPen, offsetX, y, offsetX + totalWidth, y);

                if (row < rowCount)
                {
                    TimeSpan time = startTimeLocal + _rowInterval * row;
                    var timeRect = new Rectangle(offsetX, y, _timeColumnWidth, rowH);
                    TextRenderer.DrawText(g, $"{(int)time.TotalHours % 24:00}:{time.Minutes:00}", Font, timeRect, ForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.Top);
                }
            }

            // Draw grid lines for columns
            for (int col = 0; col <= colCount; col++)
            {
                int x = offsetX + _timeColumnWidth + col * colW;
                g.DrawLine(gridPen, x, offsetY, x, offsetY + totalHeight);
            }

            // Draw header on top - this is drawn AFTER child controls so it appears over them
            if (_headerHeight > 0)
            {
                // Draw full header background (including time column area)
                g.FillRectangle(headerBrush, offsetX, headerOffsetY, totalWidth, _headerHeight);

                // Draw time column header area (if any)
                if (_timeColumnWidth > 0)
                {
                    var timeHeaderRect = new Rectangle(offsetX, headerOffsetY, _timeColumnWidth, _headerHeight);
                    TextRenderer.DrawText(g, "Time", _headerFont, timeHeaderRect, _headerForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                // Draw column headers
                for (int col = 0; col < colCount; col++)
                {
                    var info = _columnInfos[_columns[col]];
                    Rectangle headerRect = new Rectangle(offsetX + _timeColumnWidth + col * colW, headerOffsetY, colW, _headerHeight);
                    if (info.HeaderBackColor is Color columnHeaderBack)
                    {
                        if (!brushCache.TryGetValue(columnHeaderBack, out var brush))
                        {
                            brush = new SolidBrush(columnHeaderBack);
                            brushCache[columnHeaderBack] = brush;
                        }
                        g.FillRectangle(brush, headerRect);
                    }

                    Rectangle textRect = headerRect;
                    if (info.HeaderImage is Image image)
                    {
                        Size size = _headerImageSize.IsEmpty ? image.Size : _headerImageSize;
                        int maxHeight = Math.Max(0, _headerHeight - 4);
                        if (size.Height > maxHeight && size.Height > 0)
                            size = new Size(size.Width * maxHeight / size.Height, maxHeight);
                        Rectangle imageRect = new Rectangle(headerRect.Left + 4, headerRect.Top + (_headerHeight - size.Height) / 2, size.Width, size.Height);
                        g.DrawImage(image, imageRect);
                        textRect = new Rectangle(imageRect.Right + 4, headerRect.Top, Math.Max(0, headerRect.Right - imageRect.Right - 8), headerRect.Height);
                    }

                    TextRenderer.DrawText(g, _columns[col], _headerFont, textRect, _headerForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
                }

                // Draw header grid lines
                for (int col = 0; col <= colCount; col++)
                {
                    int x = offsetX + _timeColumnWidth + col * colW;
                    g.DrawLine(gridPen, x, headerOffsetY, x, headerOffsetY + _headerHeight);
                }
                // Draw horizontal line at bottom of header
                g.DrawLine(gridPen, offsetX, headerOffsetY + _headerHeight, offsetX + totalWidth, headerOffsetY + _headerHeight);
            }

            // Dispose cached brushes
            foreach (var brush in brushCache.Values)
            {
                brush.Dispose();
            }
        }

        protected override void OnScroll(ScrollEventArgs se)
        {
            base.OnScroll(se);
            Invalidate();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            Invalidate();
        }
    }
}
