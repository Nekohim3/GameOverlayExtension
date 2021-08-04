using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameOverlay.Drawing;

using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

using Font = GameOverlay.Drawing.Font;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace GameOverlayExtension.UI
{
    public class DxGroupBox : DxControl
    {
        public new List<DxControl> Childs
        {
            get => base.Childs[0].Childs;
            set => base.Childs[0].Childs = value;
        }

        #region Variables

        public SolidBrush Border { get; set; }
        public SolidBrush Fill { get; set; }
        public SolidBrush Separator { get; set; }
        public SolidBrush Header { get; set; }
        public Font HeaderFont { get; set; }
        public TextHelper Text { get; set; }

        #endregion

        #region Functions

        public DxGroupBox(string name, string text) : base(name)
        {
            BorderThickness = 0;
            Width = 100;
            Height = 100;
            Text = new TextHelper(text) { ParagraphAlignment = ParagraphAlignment.Center, FontWeight = FontWeight.SemiBold, TextAntialiasMode = TextAntialiasMode.Grayscale };
            Fill = g.Graphics.CreateSolidBrush(0, 11, 22);
            Border = g.Graphics.CreateSolidBrush(6, 25, 37);
            Separator = g.Graphics.CreateSolidBrush(1, 27, 43);
            Header = g.Graphics.CreateSolidBrush(242, 242, 242);

            HeaderFont = g.Graphics.CreateFont("museosanscyrl-900", 15);

            Brushes.Add(Fill);
            Brushes.Add(Border);
            Brushes.Add(Separator);
            Brushes.Add(Header);

            var grid = new DxGrid($"{name}>Grid")
            {
                Parent = this,
                BorderThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(5, 40, 5, 10)
            };

            base.AddChild(grid);
        }

        public override void Draw()
        {
            g.Graphics.OutlineFillRectangle(Border, Fill, Rect.X, Rect.Y, Rect.Width, Rect.Height, BorderThickness, 0);
            g.Graphics.FillRectangle(Separator, Rect.X + 5, Rect.Y + 30, Rect.Width - 10, 2);
            g.Graphics.DrawText(Text, HeaderFont, Header, null, Rect.X + 10, Rect.Y, Rect.Width - 10, 30);
            base.Draw();
        }

        public override void AddChild(DxControl ctl)
        {
            base.Childs[0].AddChild(ctl);
        }

        public override void RemoveChild(DxControl ctl)
        {
            base.Childs[0].RemoveChild(ctl);
        }

        public override void RemoveChildAt(int i)
        {
            base.Childs[0].RemoveChildAt(i);
        }

        #endregion
    }
}
