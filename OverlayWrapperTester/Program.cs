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

using SharpDX;
using SharpDX.DirectWrite;
using SharpDX.IO;
using SharpDX.WIC;

namespace OverlayWrapperTester
{
    static class Program
    {
        
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            g.Overlay = new OverlayWrapper("sublime_text")
            {
                UseHook = false
            };

            g.Overlay.OnGraphicsSetup   += Overlay_OnGraphicsSetup;
            g.Overlay.OnGraphicsDestroy += Overlay_OnGraphicsDestroy;

            g.Overlay.OnPreDraw += Overlay_OnBeforeDraw;
            g.Overlay.OnDraw       += Overlay_OnDraw;

            g.Overlay.OnKeyDown += Overlay_OnKeyDown;
            g.Overlay.OnKeyUp   += Overlay_OnKeyUp;

            g.Overlay.OnMouseDown  += Overlay_OnMouseDown;
            g.Overlay.OnMouseUp    += Overlay_OnMouseUp;
            g.Overlay.OnMouseMove  += Overlay_OnMouseMove;
            g.Overlay.OnMouseWheel += Overlay_OnMouseWheel;
            g.Graphics             =  g.Overlay.Window.Graphics;
            g.Overlay.Run();

            Application.Run();
        }

        #region overlay events

        private static void Overlay_OnGraphicsSetup(object sender, SetupGraphicsEventArgs e)
        {

        }

        private static void Overlay_OnGraphicsDestroy(object sender, DestroyGraphicsEventArgs e)
        {

        }

        private static void Overlay_OnBeforeDraw(object sender, DrawGraphicsEventArgs e)
        {

        }

        private static void Overlay_OnDraw(object sender, DrawGraphicsEventArgs e)
        {


        }

        private static void Overlay_OnKeyDown(object sender, KeyEventArgs e)
        {

        }

        private static void Overlay_OnKeyUp(object sender, KeyEventArgs e)
        {

        }

        private static void Overlay_OnMouseDown(object sender, MouseEventArgs e)
        {

        }

        private static void Overlay_OnMouseUp(object sender, MouseEventArgs e)
        {

        }

        private static void Overlay_OnMouseMove(object sender, MouseEventArgs e)
        {

        }

        private static void Overlay_OnMouseWheel(object sender, MouseEventArgs e)
        {

        }

        #endregion
    }
}