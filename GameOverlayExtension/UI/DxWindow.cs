using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GameOverlay.Drawing;

using Point = SharpDX.Point;

namespace GameOverlayExtension.UI
{
    public class DxWindow : DxControl
    {
        #region Variables

        public SolidBrush      Border;
        public SolidBrush      Fill;
        public List<DxControl> DrawOnTopList;

        #endregion

        #region Functions

        public DxWindow(string name) : base(name)
        {
            BorderThickness = 1;
            Width           = g.Window.Width;
            Height          = g.Window.Height;
            Border          = g.Graphics.CreateSolidBrush(0, 0, 0, 0);
            Fill            = g.Graphics.CreateSolidBrush(0, 0, 0, 0);
            DrawOnTopList   = new List<DxControl>();
        }

        public override void Draw()
        {
            g.Graphics.OutlineFillRectangle(Border, Fill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);
            base.Draw();

            for (var i = 0; i < DrawOnTopList.Count; i++)
                DrawOnTopList[i].Draw();
        }

        public new void RefreshRect()
        {
            Width  = g.Window.Width;
            Height = g.Window.Height;
            base.RefreshRect();
        }

        #endregion

        public override bool OnMouseDown(DxControl ctl, MouseEventArgs args, Point pt)
        {
            for (var i = DrawOnTopList.Count - 1; i >= 0; i--)
                if (DrawOnTopList[i].OnMouseDown(DrawOnTopList[i], args, pt))
                    return true;

            return base.OnMouseDown(ctl, args, pt);
        }

        public override bool OnMouseMove(DxControl ctl, MouseEventArgs args, Point pt)
        {
            for (var i = DrawOnTopList.Count - 1; i >= 0; i--)
                if (DrawOnTopList[i].OnMouseMove(DrawOnTopList[i], args, pt))
                    return true;

            return base.OnMouseMove(ctl, args, pt);
        }
    }
}
