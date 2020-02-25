﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using GameOverlay.Drawing;
using GameOverlay.PInvoke;
using GameOverlay.Windows;

using GameOverlayExtension.UI;

using Nini;

using SharpDX;
using SharpDX.Mathematics.Interop;

using Point = GameOverlay.Drawing.Point;

namespace GameOverlayExtension
{
    public class OverlayWrapper : GameOverlayExtension
    {
        #region events

        #region graphic

        public override event GraphicsSetupHandler   OnGraphicsSetup;
        public override event GraphicsDestroyHandler OnGraphicsDestroy;
        public override event DrawHandler            OnDraw;
        public override event DrawHandler            OnPreDraw;

        #endregion

        #region hook

        public override event KeyHandler   OnKeyDown;
        public override event KeyHandler   OnKeyUp;
        public override event MouseHandler OnMouseDown;
        public override event MouseHandler OnMouseUp;
        public override event MouseHandler OnMouseMove;
        public override event MouseHandler OnMouseWheel;

        #endregion

        #endregion

        private bool _graphicLoaded;
        private bool _windowLoaded;
        private int _currentProcessId;
        private string _targetProcessName;
        private bool _workWithoutProcess;

        private DrawingAreaScaleMode _scaleMode = DrawingAreaScaleMode.ScaleOnlyDrawingArea;
        private Vector2 _scale = new Vector2(1, 1);

        public DrawingAreaScaleMode ScaleMode
        {
            get => _scaleMode;
            set
            {
                _scaleMode = value;
                g.Graphics.GetRenderTarget().Transform = _scaleMode == DrawingAreaScaleMode.ScaleOnlyDrawingArea ? Matrix3x2.Identity : Matrix3x2.Scaling(_scale.X, _scale.Y);
            }
        }

        public Vector2 Scale => _scale;

        public void SetScale(float x, float y)
        {
            _scale = new Vector2(x, y);
            g.Graphics.GetRenderTarget().Transform = _scaleMode == DrawingAreaScaleMode.ScaleAll ? Matrix3x2.Scaling(_scale.X, _scale.Y) : Matrix3x2.Identity;
        }

        public OverlayWrapper() : base()
        {
            Init(SystemInformation.VirtualScreen.X, SystemInformation.VirtualScreen.Y, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height, "");
        }

        public OverlayWrapper(int x, int y, int width, int height) : base(x, y, width, height)
        {
            Init(x, y, width, height, "");
        }

        public OverlayWrapper(string p, bool workWithoutProcess = false) : base()
        {
            Init(SystemInformation.VirtualScreen.X, SystemInformation.VirtualScreen.Y, SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height, p, workWithoutProcess);
        }

        public OverlayWrapper(int x, int y, int width, int height, string p, bool workWithoutProcess = false) : base(x, y, width, height)
        {
            Init(x, y, width, height, p, workWithoutProcess);
        }

        public void Init(int x, int y, int width, int height, string p, bool workWithoutProcess = false, DrawingAreaScaleMode scaleMode = DrawingAreaScaleMode.ScaleOnlyDrawingArea)
        {
            _currentProcessId = Process.GetCurrentProcess().Id;
            _targetProcessName = p;
            _workWithoutProcess = workWithoutProcess;
            LoadConfig();

            var graphics = new Graphics
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = false,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = false,
                WindowHandle = IntPtr.Zero
            };

            Window = new GraphicsWindow(graphics)
            {
                IsTopmost = true,
                IsVisible = true,
                FPS = g.Config.GetInt("System", "fps"),
                X = x,
                Y = y,
                Width = width,
                Height = height,
                ExtendedWStyle = ExtendedWindowStyle.Transparent | ExtendedWindowStyle.Layered | ExtendedWindowStyle.ToolWindow
            };


            Window.SetupGraphics += _window_SetupGraphics;
            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.DrawGraphics += _window_DrawGraphics;
        }

        public static void LoadConfig()
        {
            g.Config = new NiniConfig("Config.ini");
            g.Config.AddConfig("Brushes", true);
            g.Config.AddConfig("Fonts",   true);
            g.Config.AddConfig("System",  true);
            g.Config.SetNew("System", "fps", 60, 60);
        }

        public override void Run()
        {
            base.Run();

            while (!_graphicLoaded)
            {
                Thread.Sleep(1);
            }

            Window.Activate();
            _windowLoaded = true;
        }

        internal override void GHook_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Loaded) return;

            if (e.Alt && e.KeyCode == Keys.LShiftKey)
                KeyboardHelper.SwitchLang();

            foreach (var q in Controls.ControlList.Where(x => x is DxTextBox c && c.IsFocused))
                q.OnKeyDown(q, e);

            OnKeyDown?.Invoke(sender, e);
        }

        internal override void GHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (!Loaded) return;

            foreach (var q in Controls.ControlList.Where(x => x is DxTextBox c && c.IsFocused))
                q.OnKeyUp(q, e);

            OnKeyUp?.Invoke(sender, e);
        }

        private int Mx = 0;
        private int My = 0;
        private int Mx1 = 0;
        private int My1 = 0;
        internal override void GHook_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - g.Overlay.Window.X;
            var my = e.Y - g.Overlay.Window.Y;

            if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            {
                mx = (int)(mx / Scale.X);
                my = (int)(my / Scale.Y);
            }

            foreach (var q in Controls.ControlList.Where(x => x is DxTextBox))
                ((DxTextBox) q).IsFocused = false;

            var c = Controls.ControlList.LastOrDefault(x => x.IsMouseOver && !x.IsTransparent);
            c?.OnMouseDown(c, e, new Point(mx, my));

            OnMouseDown?.Invoke(sender, e);
        }

        internal override void GHook_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - g.Overlay.Window.X;
            var my = e.Y - g.Overlay.Window.Y;

            if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            {
                mx = (int)(mx / Scale.X);
                my = (int)(my / Scale.Y);
            }

            var c = Controls.ControlList.LastOrDefault(x => x.IsMouseDown && !x.IsTransparent);
            c?.OnMouseUp(c, e, new Point(mx, my));

            OnMouseUp?.Invoke(sender, e);
        }

        internal override void GHook_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - g.Overlay.Window.X;
            var my = e.Y - g.Overlay.Window.Y;

            Mx = mx;
            My = my;

            if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            {
                mx = (int)(mx / Scale.X);
                my = (int)(my / Scale.Y);
            }

            Mx1 = mx;
            My1 = my;

            foreach (var q in Controls.ControlList.Where(x => !x.IsTransparent))
            {
                if (q.IntersectTest(mx, my))
                {
                    if (!q.IsMouseOver)
                    {
                        q.OnMouseEnter(q, e, new Point(mx, my));
                    }

                    q.OnMouseMove(q, e, new Point(mx, my));
                }
                else
                {
                    if (q.IsMouseOver)
                    {
                        q.OnMouseLeave(q, e, new Point(mx, my));
                    }
                }
            }

            OnMouseMove?.Invoke(sender, e);
        }

        internal override void GHook_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;
            
            OnMouseWheel?.Invoke(sender, e);
        }

        internal override void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            while (!Loaded)
            {
                Thread.Sleep(1);
            }

            BrushCollection.Init();

            FontCollection.Init();

            BrushCollection.Add("Test", 0xffcc1111);
            BrushCollection.Add("Test2", 0x33cc1111);

            //TODO: sort names

            BrushCollection.Add("Window.Fill",   0xff111111);
            BrushCollection.Add("Window.Stroke", 0xffcc1111);

            BrushCollection.Add("Window.Font", 0xffcc1111);
            FontCollection.Add("Window.Title.Font", "Verdana", 12);

            BrushCollection.Add("Control.Transparent", 0x0);

            BrushCollection.Add("Control.Fill",   0xff191919);
            BrushCollection.Add("Control.Stroke", 0xffcc1111);

            BrushCollection.Add("Control.Font", 0xffcc1111);
            FontCollection.Add("Control.Font", "Verdana", 12);

            BrushCollection.Add("Control.Fill.MouseOver",   0xff292929);
            BrushCollection.Add("Control.Stroke.MouseOver", 0xffcc1111);

            BrushCollection.Add("Control.Fill.Pressed",   0xff295929);
            BrushCollection.Add("Control.Stroke.Pressed", 0xff11cc11);

            BrushCollection.Add("Toggle.Indicator.Fill",                0);
            BrushCollection.Add("Toggle.Indicator.Hover.Fill",          0);
            BrushCollection.Add("Toggle.Indicator.Stroke",              0xff222222);
            BrushCollection.Add("Toggle.Indicator.Hover.Stroke",        0xff444444);
            BrushCollection.Add("Toggle.Indicator.Active.Fill",         0xff326496);
            BrushCollection.Add("Toggle.Indicator.Active.Hover.Fill",   0xff6496C8);
            BrushCollection.Add("Toggle.Indicator.Inactive.Fill",       0xff642E2E);
            BrushCollection.Add("Toggle.Indicator.Inactive.Hover.Fill", 0xff9E4848);

            BrushCollection.Add("TextBox.Focused.Fill",   0xff292929);
            BrushCollection.Add("TextBox.Focused.Stroke", 0xffcc5555);

            BrushCollection.Add("TrackBar.Font", 0xffcccccc);
            FontCollection.Add("TrackBar.Font", "Verdana", 12);

            BrushCollection.Add("TrackBar.Fill",                0);
            BrushCollection.Add("TrackBar.Fill.Hover",          0);
            BrushCollection.Add("TrackBar.Stroke",              0xff444450);
            BrushCollection.Add("TrackBar.Stroke.Hover",        0xff444450);
            BrushCollection.Add("TrackBar.Bar.Fill",            0xff326496);
            BrushCollection.Add("TrackBar.Bar.Fill.Hover",      0xff6496c8);
            BrushCollection.Add("TrackBar.Bar.Stroke",          0xff444450);
            BrushCollection.Add("TrackBar.Bar.Stroke.Hover",    0xff444450);
            BrushCollection.Add("TrackBar.Slider.Fill",         0xff444450);
            BrushCollection.Add("TrackBar.Slider.Fill.Hover",   0xffcccccc);
            BrushCollection.Add("TrackBar.Slider.Stroke",       0xff444450);
            BrushCollection.Add("TrackBar.Slider.Stroke.Hover", 0xffcccccc);

            OnGraphicsSetup?.Invoke(sender, e);

            _graphicLoaded = true;
        }

        internal override void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            if (!Loaded) return;
            if (!_windowLoaded) return;



            OnPreDraw?.Invoke(sender, e);

            e.Graphics.ClearScene();

            Controls.Draw();

            g.Graphics.DrawText(Mx.ToString(), FontCollection.Get("Control.Font").Font, BrushCollection.Get("Control.Font").Brush, 10, 10);
            g.Graphics.DrawText(My.ToString(), FontCollection.Get("Control.Font").Font, BrushCollection.Get("Control.Font").Brush, 50, 10);
            g.Graphics.DrawText(Mx1.ToString(), FontCollection.Get("Control.Font").Font, BrushCollection.Get("Control.Font").Brush, 10, 50);
            g.Graphics.DrawText(My1.ToString(), FontCollection.Get("Control.Font").Font, BrushCollection.Get("Control.Font").Brush, 50, 50);

            g.Graphics.DrawText(Scale.X.ToString(), FontCollection.Get("Control.Font").Font, BrushCollection.Get("Control.Font").Brush, 10, 70);
            g.Graphics.DrawText(Scale.Y.ToString(), FontCollection.Get("Control.Font").Font, BrushCollection.Get("Control.Font").Brush, 50, 70);

            OnDraw?.Invoke(sender, e);
        }

        internal override void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            if (!Loaded) return;

            OnGraphicsDestroy?.Invoke(sender, e);
        }

        ~OverlayWrapper()
        {
            // you do not need to dispose the Graphics surface
            Window.Dispose();
        }
    }

    public enum DrawingAreaScaleMode
    {
        ScaleAll,
        ScaleOnlyDrawingArea
    }
}