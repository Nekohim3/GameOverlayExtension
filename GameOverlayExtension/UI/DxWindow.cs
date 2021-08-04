using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameOverlay.Drawing;

namespace GameOverlayExtension.UI
{
    public class DxWindow : DxControl
    {
        #region Variables

        public SolidBrush Border;
        public SolidBrush Fill;

        #endregion

        #region Functions

        public DxWindow(string name) : base(name)
        {
            BorderThickness = 1;
            Width           = g.Window.Width;
            Height          = g.Window.Height;
            Border          = g.Graphics.CreateSolidBrush(0, 0, 0, 0);
            Fill            = g.Graphics.CreateSolidBrush(0, 0, 0, 0);
            Brushes.Add(Border);
            Brushes.Add(Fill);
        }

        public override void Draw()
        {
            g.Graphics.OutlineFillRectangle(Border, Fill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);
            base.Draw();
        }

        public new void RefreshRect()
        {
            Width  = g.Window.Width;
            Height = g.Window.Height;
            base.RefreshRect();
        }

        #endregion
    }
}
