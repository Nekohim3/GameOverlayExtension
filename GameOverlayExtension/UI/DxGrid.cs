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

        public DxGrid(string name) : base(name)
        {
            Width           = 100;
            Height          = 100;
            BorderThickness = 0;

            Fill        = g.Graphics.CreateSolidBrush(0, 0, 0, 1);
            HoverFill   = g.Graphics.CreateSolidBrush(0, 0, 0, 1);
            DownFill    = g.Graphics.CreateSolidBrush(0, 0, 0, 1);
            Border      = g.Graphics.CreateSolidBrush(0, 0, 0, 0);
            HoverBorder = g.Graphics.CreateSolidBrush(0, 0, 0, 0);
            DownBorder  = g.Graphics.CreateSolidBrush(0, 0, 0, 0);
        }

        public override void Draw()
        {
            if (IsMouseOver)
            {
                if (IsMouseDown)
                    g.Graphics.OutlineFillRectangle(DownBorder, DownFill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);
                else
                    g.Graphics.OutlineFillRectangle(HoverBorder, HoverFill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);
            }
            else
                g.Graphics.OutlineFillRectangle(Border, Fill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);

            base.Draw();
        }

        #endregion
    }
}
