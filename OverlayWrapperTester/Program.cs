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
using Color = GameOverlay.Drawing.Color;
using HorizontalAlignment = GameOverlayExtension.UI.HorizontalAlignment;

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
                UseHook = true
            };

            // g.Overlay.OnGraphicsSetup   += Overlay_OnGraphicsSetup;
            // g.Overlay.OnGraphicsDestroy += Overlay_OnGraphicsDestroy;
            //
            // g.Overlay.OnPreDraw += Overlay_OnBeforeDraw;
            // g.Overlay.OnDraw       += Overlay_OnDraw;
            //
            // g.Overlay.OnKeyDown += Overlay_OnKeyDown;
            // g.Overlay.OnKeyUp   += Overlay_OnKeyUp;
            //
            // g.Overlay.OnMouseDown  += Overlay_OnMouseDown;
            // g.Overlay.OnMouseUp    += Overlay_OnMouseUp;
            // g.Overlay.OnMouseMove  += Overlay_OnMouseMove;
            // g.Overlay.OnMouseWheel += Overlay_OnMouseWheel;
            g.Graphics = g.Overlay.Window.Graphics;
            g.Window   = g.Overlay.Window;
            g.DxWindow = g.Overlay.DxWindow;
            g.Overlay.Run();

            g.Overlay.AttachToTargetMode               = AttachToTargetModeEnum.Automatic;
            g.Overlay.AttachEventsRaiseType            = AttachEventsRaiseTypeEnum.ChangeTargetState;
            g.Overlay.ActionWhenTargetStateBackground  = ActionWhenTargetStateChangeEnum.OpacityChange;
            g.Overlay.ActionWhenTargetStateForeground  = ActionWhenTargetStateChangeEnum.Show;
            g.Overlay.ActionWhenTargetStateNone        = ActionWhenTargetStateChangeEnum.Exit;
            g.Overlay.OpacityWhenTargetStateBackground = 0.5f;

            var dxComboBoxSingle = new DxComboBox(g.Overlay, "dxComboBoxSingle") { MultiSelect = false, Margin = new Thickness(50, 50, 0, 0) };
            dxComboBoxSingle.Items.Add("Test0");
            dxComboBoxSingle.Items.Add("Test1");
            dxComboBoxSingle.Items.Add("Test2");
            dxComboBoxSingle.Items.Add("Test3");
            dxComboBoxSingle.Items.Add("Test4");
            dxComboBoxSingle.Items.Add("Test5");
            dxComboBoxSingle.Items.Add("Test6");
            dxComboBoxSingle.Items.Add("Test7");
            dxComboBoxSingle.Items.Add("Test8");
            dxComboBoxSingle.Items.Add("Test9");
            g.Overlay.DxWindow.AddChild(dxComboBoxSingle);

            var dxComboBoxMulti = new DxComboBox(g.Overlay, "dxComboBoxSingle") { MultiSelect = true, Margin = new Thickness(250, 50, 0, 0) };
            dxComboBoxMulti.Items.Add("Test0");
            dxComboBoxMulti.Items.Add("Test1");
            dxComboBoxMulti.Items.Add("Test2");
            dxComboBoxMulti.Items.Add("Test3");
            dxComboBoxMulti.Items.Add("Test4");
            dxComboBoxMulti.Items.Add("Test5");
            dxComboBoxMulti.Items.Add("Test6");
            dxComboBoxMulti.Items.Add("Test7");
            dxComboBoxMulti.Items.Add("Test8");
            dxComboBoxMulti.Items.Add("Test9");
            g.Overlay.DxWindow.AddChild(dxComboBoxMulti);

            var dxButton = new DxButton(g.Overlay, "dxButton", "Test button") { Margin = new Thickness(0, 50, 50, 0), HorizontalAlignment = HorizontalAlignment.Right };
            var counter  = 0;
            dxButton.MouseDown += (ctl, args, pt) =>
            {
                dxButton.Text.Text = counter++.ToString();
            };

            var dxGroupBox = new DxGroupBox(g.Overlay, "dxGroupBox", "Test groupBox") { Width = 300, Height = 150, Margin = new Thickness(50, 200, 0, 0) };
            dxGroupBox.AddChild(dxButton);
            g.DxWindow.AddChild(dxGroupBox);

            g.DxWindow.Fill.Color = new Color(8, 8, 13);
            
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