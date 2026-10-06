using System.ComponentModel;
using System.Globalization;

namespace TeleVisionGuide
{
    public class DaySelectorControl : Control
    {
        private DateTime _startDate = DateTime.Today;
        private int _dayCount = 10;
        private DateTime _selectedDate = DateTime.Today;
        private int _hoverIndex = -1;
        private Color _dayBackColor = Color.WhiteSmoke;
        private Color _selectedBackColor = Color.SteelBlue;
        private Color _selectedForeColor = Color.White;
        private Color _hoverBackColor = Color.LightSteelBlue;
        private Color _borderColor = Color.LightGray;
        private CultureInfo _culture = CultureInfo.CurrentCulture;

        public event EventHandler? SelectedDateChanged;

        public DaySelectorControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint |
                     ControlStyles.Selectable, true);
            Size = new Size(600, 50);
        }

        [Category("Behavior")]
        [Description("First day shown in the selector.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value.Date;
                SelectedDate = ClampDate(_selectedDate);
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DefaultValue(10)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int DayCount
        {
            get => _dayCount;
            set
            {
                _dayCount = Math.Max(1, value);
                SelectedDate = ClampDate(_selectedDate);
                Invalidate();
            }
        }

        [Category("Behavior")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                DateTime date = ClampDate(value.Date);
                if (date == _selectedDate)
                    return;
                _selectedDate = date;
                Invalidate();
                SelectedDateChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CultureInfo Culture { get => _culture; set { _culture = value ?? CultureInfo.CurrentCulture; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color DayBackColor { get => _dayBackColor; set { _dayBackColor = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SelectedBackColor { get => _selectedBackColor; set { _selectedBackColor = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color SelectedForeColor { get => _selectedForeColor; set { _selectedForeColor = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color HoverBackColor { get => _hoverBackColor; set { _hoverBackColor = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor { get => _borderColor; set { _borderColor = value; Invalidate(); } }

        private DateTime ClampDate(DateTime date)
        {
            DateTime last = _startDate.AddDays(_dayCount - 1);
            if (date < _startDate) return _startDate;
            if (date > last) return last;
            return date;
        }

        private Rectangle GetDayBounds(int index)
        {
            int width = ClientSize.Width / _dayCount;
            int x = index * width;
            int w = index == _dayCount - 1 ? ClientSize.Width - x : width;
            return new Rectangle(x, 0, w, ClientSize.Height);
        }

        private int HitTest(Point p)
        {
            for (int i = 0; i < _dayCount; i++)
                if (GetDayBounds(i).Contains(p))
                    return i;
            return -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            using var borderPen = new Pen(_borderColor);
            using var boldFont = new Font(Font, FontStyle.Bold);

            for (int i = 0; i < _dayCount; i++)
            {
                DateTime date = _startDate.AddDays(i);
                Rectangle rect = GetDayBounds(i);
                bool selected = date == _selectedDate;

                Color back = selected ? _selectedBackColor : i == _hoverIndex ? _hoverBackColor : _dayBackColor;
                Color fore = selected ? _selectedForeColor : ForeColor;
                using (var brush = new SolidBrush(back))
                    g.FillRectangle(brush, rect);
                g.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                string dayName = date == DateTime.Today ? "Today" : _culture.DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek);
                string dayText = date.ToString("dd.MM.", _culture);

                var top = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height / 2);
                var bottom = new Rectangle(rect.X, rect.Y + rect.Height / 2, rect.Width, rect.Height - rect.Height / 2);
                TextRenderer.DrawText(g, dayName, boldFont, top, fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom | TextFormatFlags.EndEllipsis);
                TextRenderer.DrawText(g, dayText, Font, bottom, fore,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.EndEllipsis);

                if (selected && Focused)
                    ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(rect, -3, -3), fore, back);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            int index = HitTest(e.Location);
            if (index >= 0 && e.Button == MouseButtons.Left)
                SelectedDate = _startDate.AddDays(index);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int index = HitTest(e.Location);
            if (index != _hoverIndex)
            {
                _hoverIndex = index;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverIndex = -1;
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData) =>
            keyData is Keys.Left or Keys.Right or Keys.Home or Keys.End || base.IsInputKey(keyData);

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Left: SelectedDate = _selectedDate.AddDays(-1); break;
                case Keys.Right: SelectedDate = _selectedDate.AddDays(1); break;
                case Keys.Home: SelectedDate = _startDate; break;
                case Keys.End: SelectedDate = _startDate.AddDays(_dayCount - 1); break;
            }
        }

        protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }

        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }
    }
}
