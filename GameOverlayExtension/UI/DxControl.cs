using System.Windows.Forms;
using GameOverlay.Drawing;

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

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        

        public virtual void OnMouseMove(DxControl ctl, MouseEventArgs args, Point pt)
        {
            MouseMove?.Invoke(ctl, args, pt);
        }

        public virtual void OnMouseDown(DxControl ctl, MouseEventArgs args, Point pt)
        {
            IsMouseDown = true;
            MouseDown?.Invoke(ctl, args, pt);
        }

        public virtual void OnMouseUp(DxControl ctl, MouseEventArgs args, Point pt)
        {
            IsMouseDown = false;
            MouseUp?.Invoke(ctl, args, pt);
        }

        public virtual void OnMouseEnter(DxControl ctl, MouseEventArgs args, Point pt)
        {
            IsMouseOver = true;
            MouseEnter?.Invoke(ctl, args, pt);
        }

        public virtual void OnMouseLeave(DxControl ctl, MouseEventArgs args, Point pt)
        {
            IsMouseOver = false;
            MouseLeave?.Invoke(ctl, args, pt);
        }

        public virtual void OnKeyDown(DxControl ctl, KeyEventArgs args)
        {
            KeyDown?.Invoke(ctl, args);
        }

        public virtual void OnKeyUp(DxControl ctl, KeyEventArgs args)
        {
            KeyUp?.Invoke(ctl, args);
        }

        #endregion

        #region Private properties

        private Thickness _margin;
        private int _width;
        private int _height;
        private HorizontalAlignment _horizontalAlignment;
        private VerticalAlignment _verticalAlignment;

        #endregion

        #region Public properties

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

        public bool IsTransparent { get; set; }
        public bool IsMouseOver { get; set; }
        public bool IsMouseDown { get; set; }
        public string Name { get; set; }

        #endregion



        protected ControlRectangle Rect { get; set; }

        protected DxControl(string name)
        {
            _width = 1;
            _height = 1;
            _margin = new Thickness(1, 1, 1, 1);
            Rect = new ControlRectangle(0, 0, 0, 0);
            Name = name;
            

        }

        public virtual void Draw()
        {
            //g.Graphics.OutlineFillRectangle(StrokeBrush, FillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);
        }

        public virtual bool IntersectTest(int x, int y)
        {
            return x >= Rect.X && y >= Rect.Y && x <= Rect.X + Rect.Width && y <= Rect.Y + Rect.Height;
        }

        private void RefreshRect()
        {
            if (HorizontalAlignment == HorizontalAlignment.Left)
            {
                Rect.X = Margin.Left;
                Rect.Width = Width;
            }
            if (HorizontalAlignment == HorizontalAlignment.Center)
            {
                Rect.X = (g.Graphics.Width / 2) - (Width / 2);
                Rect.Width = Width;
            }
            if (HorizontalAlignment == HorizontalAlignment.Right)
            {
                Rect.X = g.Graphics.Width - Width - Margin.Right;
                Rect.Width = Width;
            }
            if (HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                Rect.X = Margin.Left;
                Rect.Width = g.Graphics.Width - Margin.Left - Margin.Right;
            }


            if (VerticalAlignment == VerticalAlignment.Top)
            {
                Rect.Y = Margin.Top;
                Rect.Height = Height;
            }
            if (VerticalAlignment == VerticalAlignment.Center)
            {
                Rect.Y = (g.Graphics.Height / 2) - (Height / 2);
                Rect.Height = Height;
            }
            if (VerticalAlignment == VerticalAlignment.Bottom)
            {
                Rect.Y = g.Graphics.Height - Height - Margin.Bottom;
                Rect.Height = Height;
            }
            if (VerticalAlignment == VerticalAlignment.Stretch)
            {
                Rect.Y = Margin.Top;
                Rect.Height = g.Graphics.Height - Margin.Top - Margin.Bottom;
            }
        }
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
