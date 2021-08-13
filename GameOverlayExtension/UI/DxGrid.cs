using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameOverlay.Drawing;

namespace GameOverlayExtension.UI
{
    public class DxGrid : DxControl
    {
        #region Variables

        public SolidBrush Border      { get; set; }
        public SolidBrush Fill        { get; set; }
        public SolidBrush HoverBorder { get; set; }
        public SolidBrush HoverFill   { get; set; }
        public SolidBrush DownBorder  { get; set; }
        public SolidBrush DownFill    { get; set; }

        #endregion

        #region Functions

        public DxGrid(GameOverlayExtension overlay, string name) : base(overlay, name)
        {
            Width           = 100;
            Height          = 100;
            BorderThickness = 0;

            Fill        = overlay.Window.Graphics.CreateSolidBrush(0, 0, 0, 1);
            HoverFill   = overlay.Window.Graphics.CreateSolidBrush(0, 0, 0, 1);
            DownFill    = overlay.Window.Graphics.CreateSolidBrush(0, 0, 0, 1);
            Border      = overlay.Window.Graphics.CreateSolidBrush(0, 0, 0, 0);
            HoverBorder = overlay.Window.Graphics.CreateSolidBrush(0, 0, 0, 0);
            DownBorder  = overlay.Window.Graphics.CreateSolidBrush(0, 0, 0, 0);
        }

        public override void Draw(Graphics graphics, Action action)
        {
            action = () =>
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
            };
            base.Draw(graphics, action);
        }

        #endregion
    }
}
