using System.Windows.Forms;
using GameOverlay.Drawing;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Font = GameOverlay.Drawing.Font;

namespace GameOverlayExtension.UI
{
    public class DxToggle : DxControl
    {
        #region Events

        public delegate void ToggleEventHandler(DxToggle tgl, bool enabled);

        public event ToggleEventHandler EnableChanged;

        public virtual void OnEnableChanged(DxToggle tgl, bool enabled)
        {
            EnableChanged?.Invoke(tgl, enabled);
        }

        #endregion

        public SolidBrush FontBrush { get; set; }
        public Font Font { get; set; }


        public SolidBrush FillBrush { get; set; }
        public SolidBrush StrokeBrush { get; set; }
        public SolidBrush HoverFillBrush { get; set; }
        public SolidBrush HoverStrokeBrush { get; set; }
        public SolidBrush IndicatorFill { get; set; }
        public SolidBrush IndicatorHoverFill { get; set; }
        public SolidBrush IndicatorStroke { get; set; }
        public SolidBrush IndicatorHoverStroke { get; set; }
        public SolidBrush IndicatorInactiveFillBrush { get; set; }
        public SolidBrush IndicatorInactiveHoverFillBrush { get; set; }
        public SolidBrush IndicatorActiveFillBrush { get; set; }
        public SolidBrush IndicatorActiveHoverFillBrush { get; set; }

        public TextHelper Text { get; set; }

        public bool IsMouseOverIndicator { get; set; }
        public bool IsActive { get; set; }

        public DxToggle(string name, string text) : base(name)
        {
            Width = 200;
            Height = 22;
            Margin = new Thickness(11, 11, 1, 1);

            Text = new TextHelper(text)
            {
                TextAlignment = TextAlignment.Leading,
                ParagraphAlignment = ParagraphAlignment.Center,
                WordWrapping = WordWrapping.NoWrap
            };

            Font = FontCollection.Get("Control.Font").Font;
            FontBrush = BrushCollection.Get("Control.Font").Brush;

            FillBrush = BrushCollection.Get("Control.Fill").Brush;
            StrokeBrush = BrushCollection.Get("Control.Stroke").Brush;
            HoverFillBrush = BrushCollection.Get("Control.Fill.MouseOver").Brush;
            HoverStrokeBrush = BrushCollection.Get("Control.Stroke.MouseOver").Brush;
            IndicatorFill = BrushCollection.Get("Toggle.Indicator.Fill").Brush;
            IndicatorHoverFill = BrushCollection.Get("Toggle.Indicator.Fill").Brush;
            IndicatorStroke = BrushCollection.Get("Toggle.Indicator.Stroke").Brush;
            IndicatorHoverStroke = BrushCollection.Get("Toggle.Indicator.Hover.Stroke").Brush;
            IndicatorActiveFillBrush = BrushCollection.Get("Toggle.Indicator.Active.Fill").Brush;
            IndicatorActiveHoverFillBrush = BrushCollection.Get("Toggle.Indicator.Active.Hover.Fill").Brush;
            IndicatorInactiveFillBrush = BrushCollection.Get("Toggle.Indicator.Inactive.Fill").Brush;
            IndicatorInactiveHoverFillBrush = BrushCollection.Get("Toggle.Indicator.Inactive.Hover.Fill").Brush;

        }

        public override void Draw()
        {
            //base.Draw();
            if (IsMouseOver)
            {
                g.Graphics.OutlineFillRectangle(HoverStrokeBrush, HoverFillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);
                g.Graphics.OutlineFillRectangle(IndicatorHoverStroke, IndicatorHoverFill, Rect.X + Rect.Width - 52, Rect.Y + 2, 50, Rect.Height - 4, 2, 0);
                if (IsActive)
                    g.Graphics.FillRectangle(IndicatorActiveHoverFillBrush, Rect.X + Rect.Width - 52 + 24, Rect.Y + 5, 23, Rect.Height - 10);
                else
                    g.Graphics.FillRectangle(IndicatorInactiveHoverFillBrush, Rect.X + Rect.Width - 52 + 3, Rect.Y + 5, 23, Rect.Height - 10);
            }
            else
            {
                g.Graphics.OutlineFillRectangle(StrokeBrush, FillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);
                g.Graphics.OutlineFillRectangle(IndicatorStroke, IndicatorFill, Rect.X + Rect.Width - 52, Rect.Y + 2, 50, Rect.Height - 4, 2, 0);
                if (IsActive)
                    g.Graphics.FillRectangle(IndicatorActiveFillBrush, Rect.X + Rect.Width - 52 + 24, Rect.Y + 5, 23, Rect.Height - 10);
                else
                    g.Graphics.FillRectangle(IndicatorInactiveFillBrush, Rect.X + Rect.Width - 52 + 3, Rect.Y + 5, 23, Rect.Height - 10);
            }

            g.Graphics.DrawText(Text, Font, FontBrush, Rect.X + 3, Rect.Y, Rect.Width - 60, Rect.Height, true);


        }

        public override bool IntersectTest(int x, int y)
        {
            var res = base.IntersectTest(x, y);
            IsMouseOverIndicator = res && x >= Rect.X + Rect.Width - 52 && y >= Rect.Y + 2 && x <= Rect.X + Rect.Width - 2 && y <= Rect.Y + Rect.Height - 2;
            return res;
        }

        public override void OnMouseDown(DxControl ctl, MouseEventArgs args, Point pt)
        {
            if (IsMouseOverIndicator)
            {
                IsActive = !IsActive;
                OnEnableChanged(this, IsActive);
            }
            base.OnMouseDown(ctl, args, pt);
        }
    }
}
