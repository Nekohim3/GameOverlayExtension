using System.Windows.Forms;
using GameOverlay.Drawing;
using SharpDX.Direct2D1;
using Image = GameOverlay.Drawing.Image;

namespace GameOverlayExtension.UI
{
    public class DxImage : DxControl
    {
        public SolidBrush FillBrush { get; set; }
        public SolidBrush StrokeBrush { get; set; }
        public SolidBrush MouseOverFillBrush { get; set; }
        public SolidBrush MouseOverStrokeBrush { get; set; }
        public SolidBrush PressedFillBrush { get; set; }
        public SolidBrush PressedStrokeBrush { get; set; }

        public Image Image { get; set; }


        public DxImage(string name, Image image) : base(name)
        {
            Width = 100;
            Height = 100;
            Margin = new Thickness(11, 11, 1, 1);

            Image = image;

            FillBrush = BrushCollection.Get("Control.Transparent").Brush;
            StrokeBrush = BrushCollection.Get("Control.Transparent").Brush;
            MouseOverFillBrush = BrushCollection.Get("Control.Transparent").Brush;
            MouseOverStrokeBrush = BrushCollection.Get("Control.Transparent").Brush;
            PressedFillBrush = BrushCollection.Get("Control.Transparent").Brush;
            PressedStrokeBrush = BrushCollection.Get("Control.Transparent").Brush;
            IsTransparent = true;

        }

        public override void Draw()
        {
            if (IsMouseOver)
            {
                if (IsMouseDown)
                    g.Graphics.OutlineFillRectangle(PressedStrokeBrush, PressedFillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);
                else
                    g.Graphics.OutlineFillRectangle(MouseOverStrokeBrush, MouseOverFillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);
            }
            else
                g.Graphics.OutlineFillRectangle(StrokeBrush, FillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);

            var scale = 1f;
            if (Rect.Width < Image.Width)
                scale = Rect.Width / Image.Width;
            if (Rect.Height < Image.Height * scale)
                scale = Rect.Height / Image.Height;

            if (Image != null)
                g.Graphics.DrawImage(Image, Rect.X + (Rect.Width / 2) - (Image.Width * scale / 2), Rect.Y + (Rect.Height / 2) - (Image.Height * scale / 2), scale);
        }
    }
}
