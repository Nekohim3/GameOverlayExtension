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

        public DxWindow(GameOverlayExtension overlay, string name) : base(overlay, name)
        {
            BorderThickness = 1;
            Width           = overlay.Window.Width;
            Height          = overlay.Window.Height;
            Border          = overlay.Window.Graphics.CreateSolidBrush(0, 0, 0, 0);
            Fill            = overlay.Window.Graphics.CreateSolidBrush(0, 0, 0, 0);
            DrawOnTopList   = new List<DxControl>();
        }

        public override void Draw(Graphics graphics, Action action = null)
        {
            action = () =>
            {
                graphics.OutlineFillRectangle(Border, Fill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);
            };
            base.Draw(graphics, action);

            for (var i = 0; i < DrawOnTopList.Count; i++)
                DrawOnTopList[i].Draw(graphics);
        }

        public new void RefreshRect(int width, int height)
        {
            Width  = width;
            Height = height;
            base.RefreshRect();
        }

        #endregion

        public override bool OnMouseDown(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            for (var i = DrawOnTopList.Count - 1; i >= 0; i--)
                if (DrawOnTopList[i].OnMouseDown(window, DrawOnTopList[i], args, pt))
                    return true;

            return base.OnMouseDown(window, ctl, args, pt);
        }

        public override bool OnMouseMove(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            for (var i = DrawOnTopList.Count - 1; i >= 0; i--)
                if (DrawOnTopList[i].OnMouseMove(window, DrawOnTopList[i], args, pt))
                    return true;

            return base.OnMouseMove(window, ctl, args, pt);
        }
    }
}
