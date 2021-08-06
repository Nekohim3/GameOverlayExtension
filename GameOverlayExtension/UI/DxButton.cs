using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GameOverlay.Drawing;

using SharpDX.DirectWrite;

using Color = GameOverlay.Drawing.Color;
using Font = GameOverlay.Drawing.Font;
using Graphics = GameOverlay.Drawing.Graphics;
using Point = SharpDX.Point;
using SolidBrush = GameOverlay.Drawing.SolidBrush;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace GameOverlayExtension.UI
{
    public class DxButton : DxControl
    {
        #region Variables

        private VerticalAlignment _verticalContentAligment;
        private HorizontalAlignment _horizontalContentAlignment;

        public SolidBrush Border { get; set; }
        public SolidBrush Fill { get; set; }
        public SolidBrush HoverBorder { get; set; }
        public SolidBrush HoverFill { get; set; }
        public SolidBrush DownBorder { get; set; }
        public SolidBrush DownFill { get; set; }
        public SolidBrush FontBrush { get; set; }
        public static Font Font { get; set; }
        public TextHelper Text { get; set; }

        public VerticalAlignment VerticalContentAligment
        {
            get => _verticalContentAligment;
            set
            {
                _verticalContentAligment = value;

                if (VerticalContentAligment == VerticalAlignment.Top)
                    Text.ParagraphAlignment = ParagraphAlignment.Near;
                if (VerticalContentAligment == VerticalAlignment.Bottom)
                    Text.ParagraphAlignment = ParagraphAlignment.Far;
                if (VerticalContentAligment == VerticalAlignment.Center)
                    Text.ParagraphAlignment = ParagraphAlignment.Center;
                if (VerticalContentAligment == VerticalAlignment.Stretch)
                    Text.ParagraphAlignment = ParagraphAlignment.Center;
            }
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get => _horizontalContentAlignment;
            set
            {
                _horizontalContentAlignment = value;

                if (HorizontalContentAlignment == HorizontalAlignment.Left)
                    Text.TextAlignment = TextAlignment.Leading;

                if (HorizontalContentAlignment == HorizontalAlignment.Right)
                    Text.TextAlignment = TextAlignment.Trailing;

                if (HorizontalContentAlignment == HorizontalAlignment.Center)
                    Text.TextAlignment = TextAlignment.Center;

                if (HorizontalContentAlignment == HorizontalAlignment.Stretch)
                    Text.TextAlignment = TextAlignment.Justified;
            }
        }

        #endregion

        #region Functions

        public DxButton(GameOverlayExtension overlay, string name, string text) : base(overlay, name)
        {
            Width = 124;
            Height = 21;
            Text = new TextHelper(text) { TextAntialiasMode = TextAntialiasMode.Grayscale, FontWeight = FontWeight.SemiBold };
            VerticalContentAligment = VerticalAlignment.Center;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            BorderThickness = 1;

            Fill        = overlay.Window.Graphics.CreateSolidBrush(8, 8, 13);
            HoverFill   = overlay.Window.Graphics.CreateSolidBrush(8,   8,   13);
            DownFill    = overlay.Window.Graphics.CreateSolidBrush(8,   8,   13);
            Border      = overlay.Window.Graphics.CreateSolidBrush(6,   25,  37);
            HoverBorder = overlay.Window.Graphics.CreateSolidBrush(6,   25,  37);
            DownBorder  = overlay.Window.Graphics.CreateSolidBrush(6,   25,  37);
            FontBrush   = overlay.Window.Graphics.CreateSolidBrush(153, 176, 189);
            Font        = overlay.Window.Graphics.CreateFont("museosanscyrl-500", 14);
        }

        public override void Draw(Graphics graphics)
        {
            if (IsMouseOver)
            {
                if (IsMouseDown)
                    graphics.OutlineFillRectangle(DownBorder, DownFill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);
                else
                    graphics.OutlineFillRectangle(HoverBorder, HoverFill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);
            }
            else
                graphics.OutlineFillRectangle(Border, Fill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);

            graphics.DrawText(Text, Font, FontBrush, null, Rect.X, Rect.Y - 1, Rect.Width, Rect.Height);

            base.Draw(graphics);
        }

        #endregion
    }
}
