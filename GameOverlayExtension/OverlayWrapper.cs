using System;
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

using SharpDX;
using SharpDX.Direct2D1;
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

        #region Attach
        
        public delegate void            AttachEventHandler(TargetStateEnum state);

        public event AttachEventHandler AttachEvent;

        #endregion

        #region base

        public delegate void AppExitHandler();

        public event AppExitHandler AppExit;

        public void OnAppExit()
        {
            AppExit?.Invoke();
        }

        #endregion

        #endregion

        private bool   _graphicLoaded;
        private bool   _windowLoaded;

        private readonly string _attachTargetName;
        private readonly int    _currentProcessId;
        private          int    _targetProcessId;
        private          IntPtr _targetWindowHandle;
        private          uint   _foregroundProcessId;
        private          IntPtr _foregroundWindowHandle;
        //private          string _foregroundWindowTitle;

        private readonly AttachTargetEnum       _attachTargetType;
        private          TargetStateEnum        _targetState;
        
        public           AttachToTargetModeEnum AttachToTargetMode { get; set; } = AttachToTargetModeEnum.Automatic;

        public AttachEventsRaiseTypeEnum       AttachEventsRaiseType            { get; set; }
        public ActionWhenTargetStateChangeEnum ActionWhenTargetStateForeground  { get; set; } = ActionWhenTargetStateChangeEnum.Show;
        public ActionWhenTargetStateChangeEnum ActionWhenTargetStateBackground  { get; set; } = ActionWhenTargetStateChangeEnum.Hide;
        public ActionWhenTargetStateChangeEnum ActionWhenTargetStateNone        { get; set; } = ActionWhenTargetStateChangeEnum.Hide;
        public float                           OpacityWhenTargetStateForeground { get; set; } = 1f;
        public float                           OpacityWhenTargetStateBackground { get; set; } = 1f;
        public float                           OpacityWhenTargetStateNone       { get; set; } = 1f;

        public float CurrentOpacity = 1f;

        public OverlayWrapper()
        {
            _currentProcessId = Process.GetCurrentProcess().Id;
            _attachTargetName = "";

            var graphics = new Graphics
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = false,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = false,
                WindowHandle = IntPtr.Zero,
                
            };

            Window = new GraphicsWindow(graphics)
            {
                IsTopmost = true,
                IsVisible = true,
                X = SystemInformation.VirtualScreen.X,
                Y = SystemInformation.VirtualScreen.Y,
                Width = SystemInformation.VirtualScreen.Width,
                Height = SystemInformation.VirtualScreen.Height,
                FPS = 60
            };

            Window.SetupGraphics += _window_SetupGraphics;
            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.PreDrawGraphics += _window_PreDrawGraphics;
            Window.DrawGraphics += _window_DrawGraphics;
        }

        public OverlayWrapper(string p, AttachTargetEnum target = AttachTargetEnum.Process)
        {
            _currentProcessId = Process.GetCurrentProcess().Id;
            _attachTargetName = p;

            var graphics = new Graphics
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true,
                UseMultiThreadedFactories = false,
                VSync = false,
                WindowHandle = IntPtr.Zero,
                
            };

            Window = new GraphicsWindow(graphics)
            {
                IsTopmost = true,
                IsVisible = true,
                X         = SystemInformation.VirtualScreen.X,
                Y         = SystemInformation.VirtualScreen.Y,
                Width     = SystemInformation.VirtualScreen.Width,
                Height    = SystemInformation.VirtualScreen.Height,
            };

            _attachTargetType = target;

            Window.SetupGraphics   += _window_SetupGraphics;
            Window.DestroyGraphics += _window_DestroyGraphics;
            Window.PreDrawGraphics += _window_PreDrawGraphics;
            Window.DrawGraphics    += _window_DrawGraphics;
            Window.SizeChanged     += _window_SizeChanged;
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

            if (e.KeyCode == Keys.Insert)
            {
                if (Window.IsActivated)
                    Window.Deactivate();
                else
                    Window.Activate();
                Window.BringToFront();
                
            }
            

            OnKeyDown?.Invoke(sender, e);
        }

        internal override void GHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (!Loaded) return;
            

            OnKeyUp?.Invoke(sender, e);
        }

        internal override void GHook_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - Window.X;
            var my = e.Y - Window.Y;

            //if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            //{
            //    mx = (int)(mx / Scale.X);
            //    my = (int)(my / Scale.Y);
            //}

            DxWindow.OnMouseDown(DxWindow, DxWindow, e, new SharpDX.Point(mx, my));

            OnMouseDown?.Invoke(sender, e);
        }

        internal override void GHook_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - Window.X;
            var my = e.Y - Window.Y;

            //if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            //{
            //    mx = (int)(mx / Scale.X);
            //    my = (int)(my / Scale.Y);
            //}
            DxWindow.OnMouseUp(DxWindow, DxWindow, e, new SharpDX.Point(mx, my));

            OnMouseUp?.Invoke(sender, e);
        }

        internal override void GHook_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - Window.X;
            var my = e.Y - Window.Y;

            //if (ScaleMode == DrawingAreaScaleMode.ScaleAll)
            //{
            //    mx = (int)(mx / Scale.X);
            //    my = (int)(my / Scale.Y);
            //}
            DxWindow.OnMouseMove(DxWindow, DxWindow, e, new SharpDX.Point(mx, my));

            OnMouseMove?.Invoke(sender, e);
        }

        internal override void GHook_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;

            var mx = e.X - Window.X;
            var my = e.Y - Window.Y;
            
            DxWindow.OnMouseWheel(DxWindow, DxWindow, e, new SharpDX.Point(mx, my));
            
            OnMouseWheel?.Invoke(sender, e);
        }

        internal override void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            while (!Loaded)
                Thread.Sleep(1);
            
            OnGraphicsSetup?.Invoke(sender, e);
            
            Window.Hide();

            _graphicLoaded = true;
        }

        internal override void _window_PreDrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            if (!Loaded) return;
            if (!_windowLoaded) return;

            #region Attach

            try
            {
                if (_attachTargetName != "")
                {
                    if (_attachTargetType == AttachTargetEnum.Process)
                    {
                        var foregroundWindowHandle = User32.GetForegroundWindow();

                        if (foregroundWindowHandle != _foregroundWindowHandle)
                        {
                            _foregroundWindowHandle = foregroundWindowHandle;
                            User32.GetWindowThreadProcessId(_foregroundWindowHandle, ref _foregroundProcessId);
                            var processes = Process.GetProcessesByName(_attachTargetName);

                            if (processes.Length != 0)
                            {
                                _targetProcessId = processes.First().Id; 
                                if ( _targetProcessId == _foregroundProcessId || _foregroundProcessId == _currentProcessId)
                                {
                                    _targetWindowHandle = processes.First().MainWindowHandle;

                                    if (AttachToTargetMode == AttachToTargetModeEnum.Manual)
                                    {
                                        if (AttachEventsRaiseType == AttachEventsRaiseTypeEnum.ChangeTargetState && _targetState != TargetStateEnum.Foreground)
                                            AttachEvent?.Invoke(TargetStateEnum.Foreground);
                                        else if (AttachEventsRaiseType == AttachEventsRaiseTypeEnum.Always)
                                            AttachEvent?.Invoke(TargetStateEnum.Foreground);
                                    }

                                    _targetState = TargetStateEnum.Foreground;

                                    if (AttachToTargetMode == AttachToTargetModeEnum.Automatic)
                                    {
                                        if (ActionWhenTargetStateForeground == ActionWhenTargetStateChangeEnum.Exit)
                                            Process.GetCurrentProcess().Kill(); //Поменять

                                        if (ActionWhenTargetStateForeground == ActionWhenTargetStateChangeEnum.Show)
                                        {
                                            Window.Show();
                                            Window.PlaceAbove(_targetWindowHandle);
                                            Window.FitTo(_targetWindowHandle, true);
                                        }

                                        if (ActionWhenTargetStateForeground == ActionWhenTargetStateChangeEnum.Hide)
                                            Window.Hide();

                                        if (ActionWhenTargetStateForeground == ActionWhenTargetStateChangeEnum.OpacityChange)
                                        {
                                            Window.Show();
                                            CurrentOpacity = OpacityWhenTargetStateForeground;
                                            Window.PlaceAbove(_targetWindowHandle);
                                            Window.FitTo(_targetWindowHandle, true);
                                        }
                                        else
                                            CurrentOpacity = 1f;
                                    }
                                }
                                else
                                {
                                    if (AttachToTargetMode == AttachToTargetModeEnum.Manual)
                                    {
                                        if (AttachEventsRaiseType == AttachEventsRaiseTypeEnum.ChangeTargetState && _targetState != TargetStateEnum.Background)
                                            AttachEvent?.Invoke(TargetStateEnum.Background);
                                        else if (AttachEventsRaiseType == AttachEventsRaiseTypeEnum.Always)
                                            AttachEvent?.Invoke(TargetStateEnum.Background);
                                    }

                                    _targetState = TargetStateEnum.Background;

                                    if (AttachToTargetMode == AttachToTargetModeEnum.Automatic)
                                    {
                                        if (ActionWhenTargetStateBackground == ActionWhenTargetStateChangeEnum.Exit)
                                            Process.GetCurrentProcess().Kill(); //Поменять

                                        if (ActionWhenTargetStateBackground == ActionWhenTargetStateChangeEnum.Show)
                                        {
                                            Window.Show();
                                            Window.PlaceAbove(_targetWindowHandle);
                                            Window.FitTo(_targetWindowHandle, true);
                                        }

                                        if (ActionWhenTargetStateBackground == ActionWhenTargetStateChangeEnum.Hide)
                                            Window.Hide();

                                        if (ActionWhenTargetStateBackground == ActionWhenTargetStateChangeEnum.OpacityChange)
                                        {
                                            Window.Show();
                                            CurrentOpacity = OpacityWhenTargetStateBackground;
                                            Window.PlaceAbove(_targetWindowHandle);
                                            Window.FitTo(_targetWindowHandle, true);
                                        }
                                        else
                                            CurrentOpacity = 1f;
                                    }
                                }
                            }
                            else
                            {
                                if (AttachToTargetMode == AttachToTargetModeEnum.Manual)
                                {
                                    if (AttachEventsRaiseType == AttachEventsRaiseTypeEnum.ChangeTargetState && _targetState != TargetStateEnum.None)
                                        AttachEvent?.Invoke(TargetStateEnum.None);
                                    else if (AttachEventsRaiseType == AttachEventsRaiseTypeEnum.Always)
                                        AttachEvent?.Invoke(TargetStateEnum.None);
                                }

                                _targetState = TargetStateEnum.None;

                                if (AttachToTargetMode == AttachToTargetModeEnum.Manual)
                                {
                                    if (ActionWhenTargetStateNone == ActionWhenTargetStateChangeEnum.Exit)
                                        Process.GetCurrentProcess().Kill(); //Поменять

                                    if (ActionWhenTargetStateNone == ActionWhenTargetStateChangeEnum.Show)
                                    {
                                        Window.Show();
                                        Window.PlaceAbove(_targetWindowHandle);
                                        Window.FitTo(_targetWindowHandle, true);
                                    }

                                    if (ActionWhenTargetStateNone == ActionWhenTargetStateChangeEnum.Hide)
                                        Window.Hide();

                                    if (ActionWhenTargetStateNone == ActionWhenTargetStateChangeEnum.OpacityChange)
                                    {
                                        Window.Show();
                                        CurrentOpacity = OpacityWhenTargetStateNone;
                                        Window.PlaceAbove(_targetWindowHandle);
                                        Window.FitTo(_targetWindowHandle, true);
                                    }
                                    else
                                        CurrentOpacity = 1f;
                                } //none
                            }

                            
                        }
                        else
                        {
                            //not change
                            if (AttachToTargetMode == AttachToTargetModeEnum.Manual)
                                if (AttachEventsRaiseType == AttachEventsRaiseTypeEnum.Always)
                                    AttachEvent?.Invoke(TargetStateEnum.None);

                            if (AttachToTargetMode == AttachToTargetModeEnum.Automatic)
                            {
                                if (Window.IsVisible)
                                {
                                    if (_targetWindowHandle != IntPtr.Zero)
                                    {
                                        Window.PlaceAbove(_targetWindowHandle);

                                        Window.FitTo(_targetWindowHandle, true);
                                    }
                                }
                            }
                        }
                    }

                    if (_attachTargetType == AttachTargetEnum.Window)
                    {
                        
                    }
                    
                }
            }
            catch (Exception exception) { }
            //if (_attachTargetName != "")
            //{
            //    if (_attachTargetMode == AttachTarget.Process)
            //    {
            //        _foregroundWindowHandle = User32.GetForegroundWindow();
            //        User32.GetWindowThreadProcessId(_foregroundWindowHandle, out _foregroundProcessId);
            //        var processes = Process.GetProcessesByName(_attachTargetName);

            //        if (processes.Length != 0)
            //        {
            //            _targetProcessId = processes.First().Id;

            //            if (_targetProcessId == _foregroundProcessId || _foregroundProcessId == _currentProcessId)
            //            {
            //                //if (HideOverlayWhenTargetWindowNoForeground)
            //                //{
            //                    if (!g.Window.IsVisible)
            //                        g.Window.Show();
            //                //}
            //                //else
            //                //{
            //                    if (!g.Window.IsTopmost)
            //                        g.Window.MakeTopmost();
            //                //}

            //                var targetProcessMainWindowHandle = processes.First().MainWindowHandle;
            //                g.Window.PlaceAboveWindow(targetProcessMainWindowHandle);

            //                if (_aspectRatio.CloseTo(0))
            //                    g.Window.FitToWindow(targetProcessMainWindowHandle);
            //                else
            //                    g.Window.FitToWindow(targetProcessMainWindowHandle, _aspectRatio);
            //            }
            //            else
            //            {
            //                if (HideOverlayWhenTargetWindowNoForeground)
            //                {
            //                    if (g.Window.IsVisible)
            //                        g.Window.Hide();
            //                }
            //                else
            //                {
            //                    if (g.Window.IsTopmost)
            //                        g.Window.RemoveTopmost();
            //                }
            //            }
            //        }
            //        else
            //        {
            //            _targetProcessId = -1;

            //            if (WorkWithoutProcess)
            //            {
            //                if (g.Window.IsVisible)
            //                    g.Window.Hide();
            //            }
            //            else
            //            {
            //                OnAppExit();
            //                g.Overlay.StopHook();
            //                g.Overlay.Window.Graphics.Dispose();
            //                Process.GetCurrentProcess().Kill();
            //            }
            //        }
            //    }
            //    else if(_attachTargetMode == AttachTarget.ActiveWindowTitle)
            //    {
            //        _foregroundWindowTitle = User32.GetActiveWindowTitle();

            //        if (_foregroundWindowTitle != null)
            //        {
            //            if (_foregroundWindowTitle.Contains(_attachTargetName))
            //            {
            //                if (!g.Window.IsVisible)
            //                    g.Window.Show();

            //                var targetProcessMainWindowHandle = User32.GetForegroundWindow();
            //                g.Window.PlaceAboveWindow(targetProcessMainWindowHandle);

            //                g.Window.FitToWindow(targetProcessMainWindowHandle, _aspectRatio);
            //            }
            //            else
            //            {
            //                if (g.Window.IsVisible)
            //                    g.Window.Hide();
            //            }
            //        }
            //    }
            //}

            #endregion

            OnPreDraw?.Invoke(sender, e);
        }

        internal override void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            if (!Loaded) return;
            if (!_windowLoaded) return;

            e.Graphics.ClearScene();

            var layerUsed = false;
            if (!CurrentOpacity.CloseTo(1f))
            {
                var lp = new LayerParameters() { ContentBounds = new RawRectangleF(0, 0, Window.Width, Window.Height), Opacity = CurrentOpacity };
                e.Graphics.LayerHelper.AddOpacity(lp);
                layerUsed = true;
            }
            
            DxWindow.Draw(e.Graphics);

            OnDraw?.Invoke(sender, e);

            if (layerUsed) 
                e.Graphics.LayerHelper.RemoveOpacity();
        }

        internal override void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            if (!Loaded) return;

            OnGraphicsDestroy?.Invoke(sender, e);
        }

        public override void Dispose()
        {
            Window.Dispose();
            DxWindow          = null;
            OnDraw            = null;
            OnPreDraw         = null;
            OnGraphicsSetup   = null;
            OnGraphicsDestroy = null;
            OnKeyDown         = null;
            OnKeyUp           = null;
            OnMouseDown       = null;
            OnMouseUp         = null;
            OnMouseMove       = null;
            OnMouseWheel      = null;
            base.Dispose();
        }

        private void _window_SizeChanged(object sender, OverlaySizeEventArgs e)
        {
            DxWindow.RefreshRect(Window.Width, Window.Height);
        }
    }

    public enum AttachTargetEnum
    {
        Process,
        Window
    }

    public enum TargetStateEnum
    {
        None,
        Foreground,
        Background
    }

    public enum AttachToTargetModeEnum
    {
        Automatic,
        Manual
    }

    public enum AttachEventsRaiseTypeEnum
    {
        Always,
        ChangeTargetState
    }

    public enum ActionWhenTargetStateChangeEnum
    {
        Exit,
        Hide,
        Show,
        OpacityChange
    }
}