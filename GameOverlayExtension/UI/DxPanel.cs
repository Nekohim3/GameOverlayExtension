using GameOverlay.Drawing;

namespace GameOverlayExtension.UI
{
    public class DxPanel : DxControl
    {
        public SolidBrush FillBrush { get; set; }
        public SolidBrush StrokeBrush { get; set; }

        public DxPanel(string name, string text) : base(name)
        {
            Width = 100;
            Height = 100;
            Margin = new Thickness(11, 11, 1, 1);

            FillBrush = BrushCollection.Get("Control.Transparent").Brush;
            StrokeBrush = BrushCollection.Get("Control.Transparent").Brush;
            IsTransparent = true;

        }

        public override void Draw()
        {
            g.Graphics.OutlineFillRectangle(StrokeBrush, FillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);
        }
    }
}
