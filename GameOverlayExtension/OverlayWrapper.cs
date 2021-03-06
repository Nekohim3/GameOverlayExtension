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
        public override event DrawHandler            OnBeforeDraw;

        #endregion

        #region hook

        public override event KeyHandler   OnKeyDown;
        public override event KeyHandler   OnKeyUp;
        public override event MouseHandler OnMouseDown;
        public override event MouseHandler OnMouseUp;
        public override event MouseHandler OnMouseMove;
        public override event MouseHandler OnMouseWheel;

        #endregion

        #region base

        public delegate void AppExitHandler();

        public event AppExitHandler AppExit;

        public virtual void OnAppExit()
        {
            AppExit?.Invoke();
        }

        #endregion

        #endregion

        private bool   _graphicLoaded;
        private bool   _windowLoaded;

        private string _attachTargetName;

        private int    _targetProcessId;
        private int    _currentProcessId;
        private int    _foregroundProcessId;

        private string _foregroundWindowTitle;

        public bool WorkWithoutProcess { get; set; }

        private double _aspectRatio;
        private AttachTarget _attachTargetMode;

        public OverlayWrapper() : base()
        {
            _currentProcessId = Process.GetCurrentProcess().Id;
            _attachTargetName = "";

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
                X = SystemInformation.VirtualScreen.X,
                Y = SystemInformation.VirtualScreen.Y,
                Width = SystemInformation.VirtualScreen.Width,
                Height = SystemInformation.VirtualScreen.Height,
                ExtendedWStyle = ExtendedWindowStyle.Transparent | ExtendedWindowStyle.Layered | ExtendedWindowStyle.ToolWindow
            };

            Window.SetupGraphics += _window_SetupGraphics;
            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.BeforeDrawGraphics += _window_BeforeDrawGraphics;
            Window.DrawGraphics += _window_DrawGraphics;
        }

        public OverlayWrapper(string p, AttachTarget target = AttachTarget.Process) : base()
        {
            _currentProcessId = Process.GetCurrentProcess().Id;
            _attachTargetName = p;

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
                ExtendedWStyle = ExtendedWindowStyle.Transparent | ExtendedWindowStyle.Layered | ExtendedWindowStyle.ToolWindow
            };

            _attachTargetMode = target;

            Window.SetupGraphics += _window_SetupGraphics;
            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.BeforeDrawGraphics += _window_BeforeDrawGraphics;
            Window.DrawGraphics += _window_DrawGraphics;
        }

        public OverlayWrapper(string p, int surfaceWidth, int surfaceHeight, AttachTarget target = AttachTarget.Process) : base()
        {
            _currentProcessId = Process.GetCurrentProcess().Id;
            _attachTargetName = p;

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
                Width = surfaceWidth,
                Height = surfaceHeight,
                ExtendedWStyle = ExtendedWindowStyle.Transparent | ExtendedWindowStyle.Layered | ExtendedWindowStyle.ToolWindow,
                StaticSize = true,
                KeepAspectRatio = true
            };

            _attachTargetMode = target;
            _aspectRatio = (double) surfaceWidth / (double) surfaceHeight;

            Window.SetupGraphics += _window_SetupGraphics;
            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.BeforeDrawGraphics += _window_BeforeDrawGraphics;
            Window.DrawGraphics += _window_DrawGraphics;
        }

        public OverlayWrapper(string p, double aspectRatio, AttachTarget target = AttachTarget.Process) : base()
        {
            _currentProcessId = Process.GetCurrentProcess().Id;
            _attachTargetName = p;

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
                ExtendedWStyle = ExtendedWindowStyle.Transparent | ExtendedWindowStyle.Layered | ExtendedWindowStyle.ToolWindow,
                KeepAspectRatio = true
            };

            _attachTargetMode = target;
            _aspectRatio = aspectRatio;

            Window.SetupGraphics += _window_SetupGraphics;
            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.BeforeDrawGraphics += _window_BeforeDrawGraphics;
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

            Window.Deactivate();
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

        internal override void GHook_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - g.Overlay.Window.X;
            var my = e.Y - g.Overlay.Window.Y;

            //if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            //{
            //    mx = (int)(mx / Scale.X);
            //    my = (int)(my / Scale.Y);
            //}

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

            //if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            //{
            //    mx = (int)(mx / Scale.X);
            //    my = (int)(my / Scale.Y);
            //}

            var c = Controls.ControlList.LastOrDefault(x => x.IsMouseDown && !x.IsTransparent);
            c?.OnMouseUp(c, e, new Point(mx, my));

            OnMouseUp?.Invoke(sender, e);
        }

        internal override void GHook_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - g.Overlay.Window.X;
            var my = e.Y - g.Overlay.Window.Y;

            //if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            //{
            //    mx = (int)(mx / Scale.X);
            //    my = (int)(my / Scale.Y);
            //}

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
                Thread.Sleep(1);

            BrushCollection.Init();

            FontCollection.Init();

            BrushCollection.Add("Test",  0xffcc1111);
            BrushCollection.Add("Test2", 0x33cc1111);
            FontCollection.Add("TestFont", "Verdana", 36);

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

        internal override void _window_BeforeDrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            if (!Loaded) return;
            if (!_windowLoaded) return;

            #region Attach

            if (_attachTargetName != "")
            {
                if (_attachTargetMode == AttachTarget.Process)
                {
                    _foregroundProcessId = User32.GetForegroundWindow().ToInt32();
                    var processes = Process.GetProcessesByName(_attachTargetName);

                    if (processes.Length != 0)
                    {
                        if (!g.Window.IsVisible)
                            g.Window.Show();

                        _targetProcessId = processes.First().Id;
                        var targetProcessMainWindowHandle = processes.First().MainWindowHandle;
                        g.Window.PlaceAboveWindow(targetProcessMainWindowHandle);

                        if(_aspectRatio.CloseTo(0))
                            g.Window.FitToWindow(targetProcessMainWindowHandle);
                        else
                            g.Window.FitToWindow(targetProcessMainWindowHandle, _aspectRatio);
                    }
                    else
                    {
                        _targetProcessId = -1;

                        if (WorkWithoutProcess)
                        {
                            if (g.Window.IsVisible)
                                g.Window.Hide();
                        }
                        else
                        {
                            OnAppExit();
                            g.Overlay.StopHook();
                            g.Overlay.Window.Graphics.Dispose();
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                }
                else if(_attachTargetMode == AttachTarget.ActiveWindowTitle)
                {
                    _foregroundWindowTitle = User32.GetActiveWindowTitle();

                    if (_foregroundWindowTitle != null)
                    {
                        if (_foregroundWindowTitle.Contains(_attachTargetName))
                        {
                            if (!g.Window.IsVisible)
                                g.Window.Show();

                            var targetProcessMainWindowHandle = User32.GetForegroundWindow();
                            g.Window.PlaceAboveWindow(targetProcessMainWindowHandle);

                            g.Window.FitToWindow(targetProcessMainWindowHandle, _aspectRatio);
                        }
                        else
                        {
                            if (g.Window.IsVisible)
                                g.Window.Hide();
                        }
                    }
                }
            }

            #endregion

            OnBeforeDraw?.Invoke(sender, e);
        }

        internal override void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            if (!Loaded) return;
            if (!_windowLoaded) return;

            e.Graphics.ClearScene();

            Controls.Draw();

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

    public enum AttachTarget
    {
        Process,
        ActiveWindowTitle
    }
}