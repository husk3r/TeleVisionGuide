using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace TeleVisionGuide
{
    public class TeleVisionEventControl : Control
    {
        public enum ImageAlignment
        {
            Left,
            Right
        }

        private Color _leftColumnStart = Color.LightBlue;
        private Color _leftColumnEnd = Color.White;
        private Color _rightColumnStart = Color.LightCyan;
        private Color _rightColumnEnd = Color.White;
        private int _columnGap = 0;
        private int _leftColumnWidth = 45;
        private string _leftHeadline = string.Empty;
        private string _leftText = string.Empty;
        private string _rightHeadline = string.Empty;
        private string _rightText = string.Empty;
        private Font? _headlineFont;
        private Color _textColor = Color.Black;
        private int _textPadding = 2;
        private Image? _rightImage;
        private Size _rightImageSize = Size.Empty;
        private ImageAlignment _rightImageAlignment = ImageAlignment.Left;
        private int _rightImagePadding = 4;

        public TeleVisionEventControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.UserPaint, true);
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color LeftColumnStart { get => _leftColumnStart; set { _leftColumnStart = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color LeftColumnEnd { get => _leftColumnEnd; set { _leftColumnEnd = value; Invalidate(); } }

        [Category("Layout")]
        [Description("Width of the left column in pixels. 0 splits the area evenly.")]
        [DefaultValue(30)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int LeftColumnWidth { get => _leftColumnWidth; set { _leftColumnWidth = Math.Max(0, value); Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color RightColumnStart { get => _rightColumnStart; set { _rightColumnStart = value; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color RightColumnEnd { get => _rightColumnEnd; set { _rightColumnEnd = value; Invalidate(); } }

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ColumnGap { get => _columnGap; set { _columnGap = Math.Max(0, value); Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string LeftHeadline { get => _leftHeadline; set { _leftHeadline = value ?? string.Empty; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string LeftText { get => _leftText; set { _leftText = value ?? string.Empty; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string RightHeadline { get => _rightHeadline; set { _rightHeadline = value ?? string.Empty; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string RightText { get => _rightText; set { _rightText = value ?? string.Empty; Invalidate(); } }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font HeadlineFont
        {
            get => _headlineFont ?? new Font(Font.FontFamily, Font.Size, FontStyle.Bold);
            set { _headlineFont = value; Invalidate(); }
        }

        [Category("Appearance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TextColor { get => _textColor; set { _textColor = value; Invalidate(); } }

        [Category("Layout")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int TextPadding { get => _textPadding; set { _textPadding = Math.Max(0, value); Invalidate(); } }

        [Category("Layout")]
        [Description("Space in pixels between the right image and the surrounding text.")]
        [DefaultValue(10)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int RightImagePadding { get => _rightImagePadding; set { _rightImagePadding = Math.Max(0, value); Invalidate(); } }

        [Category("Appearance")]
        [Description("Image drawn in the right column below the headline. Text floats around it.")]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image? RightImage { get => _rightImage; set { _rightImage = value; Invalidate(); } }

        [Category("Appearance")]
        [Description("Display size of the right image. Empty uses the image's original size.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Size RightImageSize { get => _rightImageSize; set { _rightImageSize = value; Invalidate(); } }

        [Category("Appearance")]
        [DefaultValue(ImageAlignment.Left)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ImageAlignment RightImageAlignment { get => _rightImageAlignment; set { _rightImageAlignment = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle area = ClientRectangle;
            if (area.Width <= _columnGap || area.Height <= 0)
                return;

            int columnWidth = _leftColumnWidth > 0
                ? Math.Min(_leftColumnWidth, area.Width - _columnGap)
                : (area.Width - _columnGap) / 2;
            if (columnWidth <= 0)
                return;

            Rectangle left = new Rectangle(area.Left, area.Top, columnWidth, area.Height);
            Rectangle right = new Rectangle(area.Left + columnWidth + _columnGap, area.Top, area.Width - columnWidth - _columnGap, area.Height);

            DrawGradient(e.Graphics, left, _leftColumnStart, _leftColumnEnd);
            DrawGradient(e.Graphics, right, _rightColumnStart, _rightColumnEnd);

            DrawColumnContent(e.Graphics, left, _leftHeadline, _leftText, null);
            DrawColumnContent(e.Graphics, right, _rightHeadline, _rightText, _rightImage);
        }

        private void DrawColumnContent(Graphics g, Rectangle column, string headline, string text, Image? image)
        {
            Rectangle inner = Rectangle.Inflate(column, -_textPadding, -_textPadding);
            if (inner.Width <= 0 || inner.Height <= 0)
                return;

            const TextFormatFlags flags = TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.EndEllipsis;
            int y = inner.Top;

            if (!string.IsNullOrEmpty(headline))
            {
                Font headlineFont = HeadlineFont;
                try
                {
                    Size size = TextRenderer.MeasureText(g, headline, headlineFont, new Size(inner.Width, int.MaxValue), flags);
                    Rectangle headlineRect = new Rectangle(inner.Left, y, inner.Width, Math.Min(size.Height, inner.Bottom - y));
                    TextRenderer.DrawText(g, headline, headlineFont, headlineRect, _textColor, flags);
                    y += headlineRect.Height + _textPadding / 2;
                }
                finally
                {
                    if (_headlineFont == null)
                        headlineFont.Dispose();
                }
            }

            if (y >= inner.Bottom)
                return;

            Rectangle imageRect = Rectangle.Empty;
            if (image != null)
            {
                Size size = _rightImageSize.IsEmpty ? image.Size : _rightImageSize;
                int availableWidth = Math.Max(1, inner.Width - _rightImagePadding);
                if (size.Width > availableWidth)
                    size = new Size(availableWidth, (int)((long)size.Height * availableWidth / Math.Max(1, size.Width)));
                int x = _rightImageAlignment == ImageAlignment.Left
                    ? inner.Left + _rightImagePadding
                    : inner.Right - _rightImagePadding - size.Width;
                imageRect = new Rectangle(x, y + _rightImagePadding, size.Width, size.Height);
                GraphicsState state = g.Save();
                g.SetClip(inner);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, imageRect);
                g.Restore(state);
            }

            if (string.IsNullOrEmpty(text))
                return;

            if (imageRect.IsEmpty)
            {
                Rectangle textRect = new Rectangle(inner.Left, y, inner.Width, inner.Bottom - y);
                TextRenderer.DrawText(g, text, Font, textRect, _textColor, flags);
                return;
            }

            DrawFloatingText(g, text, inner, y, imageRect);
        }

        private void DrawFloatingText(Graphics g, string text, Rectangle inner, int y, Rectangle imageRect)
        {
            const TextFormatFlags lineFlags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine | TextFormatFlags.Left | TextFormatFlags.Top;
            int lineHeight = TextRenderer.MeasureText(g, "Ag", Font, Size.Empty, lineFlags).Height;
            int gap = _rightImagePadding;

            foreach (string paragraph in text.Replace("\r\n", "\n").Split('\n'))
            {
                string[] words = paragraph.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int index = 0;

                do
                {
                    if (y + lineHeight > inner.Bottom)
                        return;

                    int left = inner.Left;
                    int right = inner.Right;
                    bool besideImage = y < imageRect.Bottom + gap && y + lineHeight > imageRect.Top - gap;
                    if (besideImage)
                    {
                        if (_rightImageAlignment == ImageAlignment.Left)
                            left = imageRect.Right + gap;
                        else
                            right = imageRect.Left - gap;
                    }

                    int width = right - left;
                    if (width < lineHeight * 2)
                    {
                        y = besideImage ? imageRect.Bottom + gap : y + lineHeight;
                        continue;
                    }

                    string line = string.Empty;
                    while (index < words.Length)
                    {
                        string candidate = line.Length == 0 ? words[index] : line + " " + words[index];
                        if (TextRenderer.MeasureText(g, candidate, Font, Size.Empty, lineFlags).Width > width && line.Length > 0)
                            break;
                        line = candidate;
                        index++;
                    }

                    TextRenderer.DrawText(g, line, Font, new Rectangle(left, y, width, lineHeight), _textColor, lineFlags | TextFormatFlags.EndEllipsis);
                    y += lineHeight;
                }
                while (index < words.Length);
            }
        }

        private static void DrawGradient(Graphics g, Rectangle rect, Color start, Color end)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            using LinearGradientBrush brush = new LinearGradientBrush(rect, start, end, LinearGradientMode.Horizontal);
            g.FillRectangle(brush, rect);
        }
    }
}
