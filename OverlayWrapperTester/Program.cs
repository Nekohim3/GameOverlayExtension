using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GameOverlay.Drawing;
using GameOverlay.PInvoke;
using GameOverlay.Windows;

using GameOverlayExtension;
using GameOverlayExtension.UI;

using SharpDX;
using SharpDX.DirectWrite;
using SharpDX.IO;
using SharpDX.WIC;

using FontCollection = GameOverlayExtension.FontCollection;
using HorizontalAlignment = GameOverlayExtension.UI.HorizontalAlignment;

namespace OverlayWrapperTester
{
    static class Program
    {
        public static bool Loaded;

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //g.Overlay = new OverlayWrapper(100, 100, 1280, 720)
            //{
            //    UseHook = true,
            //    Window = { Title = "Test" },
            //};
            //g.Overlay = new OverlayWrapper(100, 100, 1000, 500)
            //{
            //    UseHook = true,
            //    Window = { Title = "Test" },

            //};
            g.Overlay = new OverlayWrapper("sublime_text")
            {
                UseHook = false,
                Window  = {Title = "Test"},
            };

            //g.Overlay.Window.Offsets = new GameOverlay.Drawing.Rectangle(2, 32, 2, 0);
            //g.Overlay.Window.MaximizedWindowOffsets = new GameOverlay.Drawing.Rectangle(2, 32, 61, 3);

            g.Overlay.OnGraphicsSetup   += Overlay_OnGraphicsSetup;
            g.Overlay.OnGraphicsDestroy += Overlay_OnGraphicsDestroy;

            g.Overlay.OnBeforeDraw += Overlay_OnBeforeDraw;
            g.Overlay.OnDraw       += Overlay_OnDraw;

            g.Overlay.OnKeyDown += Overlay_OnKeyDown;
            g.Overlay.OnKeyUp   += Overlay_OnKeyUp;

            g.Overlay.OnMouseDown  += Overlay_OnMouseDown;
            g.Overlay.OnMouseUp    += Overlay_OnMouseUp;
            g.Overlay.OnMouseMove  += Overlay_OnMouseMove;
            g.Overlay.OnMouseWheel += Overlay_OnMouseWheel;
            g.Graphics             =  g.Overlay.Window.Graphics;
            g.Overlay.Run();

            Loaded = true;
            //var tt = new Timer {Interval = 3000};
            //tt.Tick += (sender, args) => throw new Exception();
            //tt.Start();
            Application.Run();
        }

        #region overlay events

        private static void Overlay_OnGraphicsSetup(object sender, SetupGraphicsEventArgs e)
        {
            //textbox = new DxTextBox("2", "qwertyuiop[]asdfghjkl;'zxcvbnm,./1234567890") { Margin = new Thickness(650, 10, 0, 0), Width = 550 };
            //textbox1 = new DxTextBox("2", "qwertyuiop[]asdfghjkl;'zxcvbnm,./1234567890".ToUpperInvariant()) { Margin = new Thickness(650, 35, 0, 0), Width = 550 };
            //Controls.Add(textbox);
            //Controls.Add(textbox1);

            //l1 = new DxLabel("1", $"{g.Graphics.MeasureText(textbox.Text.Substring(0, textbox.CaretPos + 1), FontCollection.Get("Control.Font").Font).Width}") { Margin = new Thickness(10, 35, 0, 0), Width = 200 };
            //Controls.Add(l1);

            //tb = new DxTrackBar("3", "DxTrackBar")
            //{
            //    Margin = new Thickness(200,500,0,0),
            //    Min = 0,
            //    Max = 255,
            //    TickRate = 1,
            //    Value = 0,
            //    IsSnapToTick = true
            //};
            //Controls.Add(tb);
            ////l = new DxLabel("1", $"")
            ////{
            ////    Margin = new Thickness(10, 10, 0, 0), Width = 800, Height = 500
            ////};
            ////Controls.Add(l);

            //bmp = new Image(g.Graphics.GetRenderTarget(), "q.png");
            //var q = new DxImage("", bmp){Margin = new Thickness(10,10,0,0), Width = 200, Height = 200};
            //Controls.Add(q);

            //var button = new DxButton("button", "Test")
            //{
            //    Margin = new Thickness(10, 10, 0, 0)
            //};

            //button.Click += btn =>
            //{
            //    //MsgBox();
            //};

            //Controls.Add(button);

            //var image = new DxImage("image", new Image(g.Graphics.GetRenderTarget(), "q.png")) {Width = 50, Height = 75, Margin = new Thickness(100, 10, 0, 0)};
            //Controls.Add(image);

            //var panel = new DxPanel("panel")
            //{
            //    Width       = 50,
            //    Height      = 75,
            //    Margin      = new Thickness(100, 10, 0, 0),
            //    StrokeBrush = BrushCollection.Get("Control.Stroke").Brush
            //};
            //Controls.Add(panel);

            //var imagebutton = new DxImageButton("imagebutton", new Image(g.Graphics.GetRenderTarget(), "q.png")) { Width = 50, Height = 75, Margin = new Thickness(200, 10, 0, 0) };
            //imagebutton.Click += btn =>
            //{
            //    //MsgBox();
            //};
            //Controls.Add(imagebutton);

            //var label = new DxLabel("label", "text"){Margin = new Thickness(300,10,0,0)};
            //Controls.Add(label);

            //var textbox = new DxTextBox("textbox", "test")
            //{
            //    Width = 100,
            //    Margin = new Thickness(100,150,0,0)
            //};
            //Controls.Add(textbox);

            //var toggle = new DxToggle("toggle", "test toggle")
            //{
            //    Width = 200,
            //    Margin = new Thickness(100,200,0,0),
            //    IsActive = true
            //};
            //Controls.Add(toggle);

            var trackbar = new DxTrackBar("trackbar", "trackbar test")
            {
                Width        = 200,
                Max          = 250,
                Min          = 0,
                IsSnapToTick = true,
                TickRate     = 1,
                Margin       = new Thickness(100, 250, 0, 0)
            };

            var label = new DxLabel("", "Vulkan")
            {
                Width     = 200,
                Height    = 50,
                FillBrush = BrushCollection.Get("Window.Fill").Brush,
                Font      = FontCollection.Get("TestFont").Font,
                FontBrush = BrushCollection.Get("Test").Brush,
                Margin    = new Thickness(400, 400, 0, 0),
                Text = new TextHelper("Vlukan")
                {
                    TextAlignment      = TextAlignment.Center,
                    ParagraphAlignment = ParagraphAlignment.Center,
                }
            };

            Controls.Add(trackbar);

            Controls.Add(label);
        }

        private static void Overlay_OnGraphicsDestroy(object sender, DestroyGraphicsEventArgs e)
        {
            if (!Loaded) return;
        }

        private static void Overlay_OnBeforeDraw(object sender, DrawGraphicsEventArgs e)
        {
            if (!Loaded) return;

            //l.Text = $"{g.Graphics.MeasureText(textbox.Text.Substring(0, textbox.CaretPos), FontCollection.Get("Control.Font").Font).Width}";
            //l1.Text = $"{g.Graphics.MeasureText(textbox.Text.Substring(0, textbox.CaretPos + 1), FontCollection.Get("Control.Font").Font).Width}";
        }

        private static void Overlay_OnDraw(object sender, DrawGraphicsEventArgs e)
        {
            if (!Loaded) return;

            g.Graphics.OutlineFillRectangle(BrushCollection.Get("Test").Brush, BrushCollection.Get("Test2").Brush, 0, 0, g.Graphics.Width, g.Graphics.Height, 1, 0);
            //g.Graphics.DrawText($"{User32.GetWindowLong(Process.GetProcessesByName("nox").First().MainWindowHandle, -16)}", FontCollection.Get("Window.Title.Font").Font, BrushCollection.Get("Test").Brush, BrushCollection.Get("Window.Fill").Brush, 50,50);
            //g.Graphics.FillRectangle(BrushCollection.Get("Test").Brush, 50,50,50,50);
            //g.Graphics.DrawTextWithBackground(FontCollection.Get("Window.Title.Font").Font, BrushCollection.Get("Window.Font").Brush, BrushCollection.Get("Control.Fill.Pressed").Brush, 50, 50, 300, 300, "qweasdzxc\nqweqweqweasdasdasdasdasd", TextAlignment.Trailing);
        }

        private static void Overlay_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!Loaded) return;

            if (e.KeyCode == Keys.Insert)
            {
                g.Overlay.Window.Show();
            }

            //if (e.KeyCode == Keys.Home)
            //{
            //    g.Overlay.ScaleMode = DrawingAreaScaleMode.ScaleOnlyDrawingArea;
            //}
            //if (e.KeyCode == Keys.PageUp)
            //    g.Overlay.ScaleMode = DrawingAreaScaleMode.ScaleAll;
        }

        private static void Overlay_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!Loaded) return;
        }

        private static void Overlay_OnMouseDown(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;
        }

        private static void Overlay_OnMouseUp(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;
        }

        private static void Overlay_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;
        }

        private static void Overlay_OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (!Loaded) return;
        }

        #endregion
    }
}