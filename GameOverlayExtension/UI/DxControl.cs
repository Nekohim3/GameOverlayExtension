using System.Collections.Generic;
using System.Windows.Forms;
using GameOverlay.Drawing;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

using Point = SharpDX.Point;

namespace GameOverlayExtension.UI
{

    public abstract class DxControl
    {
        #region Events

        public delegate void MouseEventHandler(DxControl ctl, MouseEventArgs args, Point pt);
        public delegate void KeyEventHandler(DxControl ctl, KeyEventArgs args);

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseLeave;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseWheel;

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        public virtual bool OnMouseMove(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            if (ClipToBonds && !IntersectTest(pt.X, pt.Y))
            {
                RecursiveMouseLeave(window, ctl, args, pt);

                return false;
            }

            var f = false;
            if (Childs != null)
                for (var i = Childs.Count - 1; i >= 0; i--)
                    if (!f)
                    {
                        if (Childs[i].OnMouseMove(window, Childs[i], args, pt))
                        {
                            f = true;
                            OnMouseLeave(window, ctl, args, pt);
                        }
                    }
                    else
                        Childs[i].OnMouseLeave(window, ctl, args, pt);

            if (f) return true;

            if (!ClipToBonds && !IntersectTest(pt.X, pt.Y))
            {
                OnMouseLeave(window, ctl, args, pt);

                return false;
            }

            MouseMove?.Invoke(ctl, args, pt);
            OnMouseEnter(window, ctl, args, pt);

            return true;
        }

        public virtual bool OnMouseDown(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            if (Childs != null)
                for (var i = Childs.Count - 1; i >= 0; i--)
                    if (Childs[i].OnMouseDown(window, Childs[i], args, pt))
                        return true;

            if (!IntersectTest(pt.X, pt.Y)) return false;
            if (!IsMouseOver) return false;

            for (var i = window.DrawOnTopList.Count - 1; i >= 0; i--)
                if (window.DrawOnTopList[i].IntersectTest(pt.X, pt.Y))
                    return false;

            IsMouseDown = true;
            MouseDown?.Invoke(ctl, args, pt);

            return true;
        }

        public virtual bool OnMouseUp(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            if (Childs != null)
                for (var i = Childs.Count - 1; i >= 0; i--)
                    if (Childs[i].OnMouseUp(window, Childs[i], args, pt))
                        return true;

            if (!IntersectTest(pt.X, pt.Y)) return false;
            if (!IsMouseDown || !IsMouseOver) return false;
            IsMouseDown = false;
            MouseUp?.Invoke(ctl, args, pt);

            return true;
        }

        public virtual void OnMouseEnter(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            if (IsMouseOver) return;

            IsMouseOver = true;
            MouseEnter?.Invoke(ctl, args, pt);
        }

        public virtual void OnMouseLeave(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            if (!IsMouseOver) return;
            IsMouseOver = false;
            IsMouseDown = false;
            MouseLeave?.Invoke(ctl, args, pt);
        }

        public virtual bool OnMouseWheel(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            if (Childs != null)
                for (var i = Childs.Count - 1; i >= 0; i--)
                    if (Childs[i].OnMouseWheel(window, Childs[i], args, pt))
                        return true;

            if (!IntersectTest(pt.X, pt.Y)) return false;
            if (!IsMouseOver) return false;

            MouseWheel?.Invoke(ctl, args, pt);

            return true;
        }

        public virtual bool OnKeyDown(DxWindow window, DxControl ctl, KeyEventArgs args)
        {
            KeyDown?.Invoke(ctl, args);
            return true;
        }

        public virtual bool OnKeyUp(DxWindow window, DxControl ctl, KeyEventArgs args)
        {
            KeyUp?.Invoke(ctl, args);
            return true;
        }

        #endregion

        #region Variables

        private Thickness _margin;
        private int _width;
        private int _height;
        private HorizontalAlignment _horizontalAlignment;
        private VerticalAlignment _verticalAlignment;
        private float _opacity;

        public DxControl Parent { get; set; }
        public List<DxControl> Childs { get; set; }
        public bool ClipToBonds { get; set; }
        public bool IsMouseOver { get; set; }
        public bool IsMouseDown { get; set; }
        public string Name { get; set; }
        public ControlRectangle Rect { get; set; }
        public int BorderThickness { get; set; }
        public bool TopMost { get; set; }

        public Thickness Margin
        {
            get => _margin;
            set
            {
                _margin = value;
                RefreshRect();
            }
        }

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                RefreshRect();
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                RefreshRect();
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get => _horizontalAlignment;
            set
            {
                _horizontalAlignment = value;
                RefreshRect();
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get => _verticalAlignment;
            set
            {
                _verticalAlignment = value;
                RefreshRect();
            }
        }

        public float Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
            }
        }

        #endregion

        #region Functions

        protected DxControl(GameOverlayExtension overlay, string name)
        {
            Name = name;
            ClipToBonds = true;
            _width = 1;
            _height = 1;
            _opacity = 1f;
            _margin = new Thickness(0, 0, 0, 0);
            Rect = new ControlRectangle(0, 0, 1, 1);
            _horizontalAlignment = HorizontalAlignment.Left;
            _verticalAlignment = VerticalAlignment.Top;
            Childs = new List<DxControl>();
            RefreshRect();
        }

        protected void RefreshRect()
        {
            if (HorizontalAlignment == HorizontalAlignment.Left)
            {
                Rect.X = (Parent?.Rect.X ?? 0) + Margin.Left;
                Rect.Width = Width;
            }
            if (HorizontalAlignment == HorizontalAlignment.Center)
            {
                Rect.X = (Parent?.Rect.X ?? 0) + (Parent?.Rect.Width ?? 0) / 2 - Width / 2 + Margin.Left / 2 - Margin.Right / 2;
                Rect.Width = Width;
            }
            if (HorizontalAlignment == HorizontalAlignment.Right)
            {
                Rect.X = (Parent?.Rect.X ?? 0) + (Parent?.Rect.Width ?? 0) - Width - Margin.Right;
                Rect.Width = Width;
            }
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                Rect.X = (Parent?.Rect.X ?? 0) + Margin.Left;
                Rect.Width = (Parent?.Rect.Width ?? 0) - Margin.Left - Margin.Right;
            }


            if (VerticalAlignment == VerticalAlignment.Top)
            {
                Rect.Y = (Parent?.Rect.Y ?? 0) + Margin.Top;
                Rect.Height = Height;
            }
            if (VerticalAlignment == VerticalAlignment.Center)
            {
                Rect.Y = (Parent?.Rect.Y ?? 0) + (Parent?.Rect.Height ?? 0) / 2 - Height / 2 + Margin.Top / 2 - Margin.Bottom / 2;
                Rect.Height = Height;
            }
            if (VerticalAlignment == VerticalAlignment.Bottom)
            {
                Rect.Y = (Parent?.Rect.Y ?? 0) + (Parent?.Rect.Height ?? 0) - Height - Margin.Bottom;
                Rect.Height = Height;
            }
            if (VerticalAlignment == VerticalAlignment.Stretch)
            {
                Rect.Y = (Parent?.Rect.Y ?? 0) + Margin.Top;
                Rect.Height = (Parent?.Rect.Height ?? 0) - Margin.Top - Margin.Bottom;
            }

            if (Childs != null)
                for (var i = 0; i < Childs.Count; i++)
                    Childs[i].RefreshRect();
        }

        protected void RecursiveMouseLeave(DxWindow window, DxControl ctl, MouseEventArgs args, Point pt)
        {
            ctl.OnMouseLeave(window, ctl, args, pt);

            if (ctl.Childs == null) return;

            for (var i = ctl.Childs.Count - 1; i >= 0; i--)
                ctl.Childs[i].RecursiveMouseLeave(window, ctl.Childs[i], args, pt);
        }

        public virtual void AddChild(DxControl ctl)
        {
            if (Childs == null) return;
            Childs.Add(ctl);
            ctl.Parent = this;
            RefreshRect();
        }

        public virtual void RemoveChild(DxControl ctl)
        {
            Childs?.Remove(ctl);
        }

        public virtual void RemoveChildAt(int i)
        {
            Childs?.RemoveAt(i);
        }

        public virtual void Draw(Graphics graphics)
        {
            if (Childs != null)
            {
                var clipped = false;
                
                if (ClipToBonds)
                {
                    graphics.ClipRegionStart(Rect.X, Rect.Y, Rect.X + Rect.Width, Rect.Y + Rect.Height);
                    clipped = true;
                }

                var   layerUsed = false;
                if (!Opacity.CloseTo(1))
                {
                    var lp = new LayerParameters() { ContentBounds = new RawRectangleF(Rect.X, Rect.Y, Rect.X + Rect.Width, Rect.Y + Rect.Height), Opacity = Opacity };
                    graphics.AddOpacity(lp);
                    layerUsed = true;
                }

                for (var i = 0; i < Childs.Count; i++)
                    if (!Childs[i].TopMost)
                        Childs[i].Draw(graphics);

                if (layerUsed)
                    graphics.RemoveOpacity();

                if (clipped)
                    graphics.ClipRegionEnd();
            }
        }

        public virtual bool IntersectTest(int x, int y)
        {
            return x >= Rect.X && y >= Rect.Y && x <= Rect.X + Rect.Width && y <= Rect.Y + Rect.Height;
        }

        #endregion
    }

    public struct Thickness
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public Thickness(int uniformLength)
        {
            Left = Top = Right = Bottom = uniformLength;
        }

        public Thickness(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public override string ToString()
        {
            return $"{Left} {Top} {Right} {Bottom}";
        }
    }

    public class ControlRectangle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ControlRectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{X} {Y} {Width} {Height}";
        }
    }

    public enum HorizontalAlignment
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Stretch = 3
    }

    public enum VerticalAlignment
    {
        Top = 0,
        Center = 1,
        Bottom = 2,
        Stretch = 3
    }
}
