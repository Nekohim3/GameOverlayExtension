using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using GameOverlay.Drawing;

using SharpDX.DirectWrite;

using Font = GameOverlay.Drawing.Font;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;

namespace GameOverlayExtension.UI
{
    public class DxComboBox : DxControl
    {
        public new List<DxControl> Childs => null;

        #region Variables

        private int _maxItemsVisible = 7;
        private int _currentPosition;
        private int _currentHighlightItem = -1;

        public SolidBrush Border { get; set; }
        public SolidBrush HoverBorder { get; set; }
        public SolidBrush ActiveBorder { get; set; }
        public SolidBrush Fill { get; set; }
        public SolidBrush HoverFill { get; set; }
        public SolidBrush ActiveFill { get; set; }
        public SolidBrush FontBrush { get; set; }
        public SolidBrush ActiveFontBrush { get; set; }
        public SolidBrush Separator { get; set; }
        public SolidBrush ScrollBarActive { get; set; }
        public SolidBrush ScrollBarFill { get; set; }
        public SolidBrush ItemOverHighlight { get; set; }
        public Font Font { get; set; }

        public List<object> Items { get; set; }
        public List<int> SelectedIndexes { get; set; }

        public List<object> SelectedItems
        {
            get
            {
                var lst = new List<object>();
                for (var i = 0; i < Items.Count; i++)
                    if (SelectedIndexes.Contains(i))
                        lst.Add(Items[i]);

                return lst;
            }
        }

        public object SelectedItem => SelectedItems.FirstOrDefault();

        public int SelectedIndex => SelectedIndexes.Count == 0 ? -1 : SelectedIndexes.First();

        public bool Active { get; set; }
        public bool MultiSelect { get; set; }

        #endregion

        #region Functions

        public override bool OnMouseMove(DxControl ctl, MouseEventArgs args, SharpDX.Point pt)
        {
            if (!IntersectTest(pt.X, pt.Y))
            {
                OnMouseLeave(ctl, args, pt);
                _currentHighlightItem = -1;

                return false;
            }

            OnMouseEnter(ctl, args, pt);

            var f = false;

            for (var i = 0; i < (Items.Count > _maxItemsVisible ? _maxItemsVisible : Items.Count) && !f; i++)
            {
                if (pt.X < Rect.X + 4 || pt.X > Rect.X + Rect.Width - 17 || pt.Y < Rect.Y + 21 + i * 21 || pt.Y > Rect.Y + 21 + i * 21 + 21) continue;

                _currentHighlightItem = i;
                f = true;
            }

            if (!f) _currentHighlightItem = -1;

            return true;
        }

        public override bool OnMouseDown(DxControl ctl, MouseEventArgs args, SharpDX.Point pt)
        {
            if (!IntersectTest(pt.X, pt.Y) || !IsMouseOver)
            {
                if (Active)
                {
                    TopMost = false;
                    g.Overlay.TopList.Remove(this);
                }

                Active = false;
                return false;
            }


            var oldActive = Active;

            if (pt.X >= Rect.X && pt.X <= Rect.X + Rect.Width && pt.Y >= Rect.Y && pt.Y <= Rect.Y + 21)
            {
                Active = !Active;
            }


            if (Active)
            {
                if (oldActive == false)
                    _currentPosition = 0;

                if (_currentHighlightItem != -1)
                {
                    if (MultiSelect)
                    {
                        if (SelectedIndexes.Contains(_currentHighlightItem + _currentPosition))
                            SelectedIndexes.Remove(_currentHighlightItem + _currentPosition);
                        else
                            SelectedIndexes.Add(_currentHighlightItem + _currentPosition);
                    }
                    else
                    {
                        SelectedIndexes.Clear();
                        SelectedIndexes.Add(_currentHighlightItem + _currentPosition);

                    }
                }

                if (!MultiSelect)
                    if (oldActive)
                        Active = false;
            }

            TopMost = Active;

            if (Active && !oldActive)
                g.Overlay.TopList.Add(this);
            else if (!Active && oldActive)
                g.Overlay.TopList.Remove(this);


            return true;
        }

        public override bool OnMouseWheel(DxControl ctl, MouseEventArgs args, SharpDX.Point pt)
        {
            if (!IntersectTest(pt.X, pt.Y)) return false;
            if (!IsMouseOver) return false;

            if (args.Delta > 0) // up
            {
                if (_currentPosition > 0)
                    _currentPosition--;
            }
            else
            {
                if (_currentPosition < Items.Count - _maxItemsVisible)
                    _currentPosition++;
            }

            return false;
        }

        public DxComboBox(string name) : base(name)
        {
            Width = 150;
            Height = 21;
            Fill = g.Graphics.CreateSolidBrush(3, 5, 13);
            HoverFill = g.Graphics.CreateSolidBrush(0, 11, 22);
            ActiveFill = g.Graphics.CreateSolidBrush(3, 18, 33);
            Border = g.Graphics.CreateSolidBrush(3, 23, 37);
            HoverBorder = g.Graphics.CreateSolidBrush(3, 32, 51);
            ActiveBorder = g.Graphics.CreateSolidBrush(3, 32, 51);
            ActiveFontBrush = g.Graphics.CreateSolidBrush(242, 242, 242);
            FontBrush = g.Graphics.CreateSolidBrush(153, 176, 189);
            Separator = g.Graphics.CreateSolidBrush(3, 50, 70);
            ScrollBarActive = g.Graphics.CreateSolidBrush(3, 168, 245);
            ScrollBarFill = g.Graphics.CreateSolidBrush(23, 36, 46);
            ItemOverHighlight = g.Graphics.CreateSolidBrush(5, 50, 70);

            Font = g.Graphics.CreateFont("museosanscyrl-500", 13);

            Brushes.Add(Fill);
            Brushes.Add(HoverFill);
            Brushes.Add(ActiveFill);
            Brushes.Add(Border);
            Brushes.Add(HoverBorder);
            Brushes.Add(ActiveBorder);
            Brushes.Add(FontBrush);
            Brushes.Add(ActiveFontBrush);
            Brushes.Add(Separator);
            Brushes.Add(ScrollBarActive);
            Brushes.Add(ScrollBarFill);
            Brushes.Add(ItemOverHighlight);

            Items = new List<object>();
            SelectedIndexes = new List<int>() { };
        }

        public override void Draw()
        {
            if (Active)
            {
                if (Items.Count < _maxItemsVisible)
                    Rect.Height = (Items.Count + 1) * 21 + 5;
                else
                    Rect.Height = (_maxItemsVisible + 1) * 21 + 5;

                g.Graphics.OutlineFillRectangle(ActiveBorder, ActiveFill, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);

                g.Graphics.DrawText(new TextHelper(SelectedItems.Cast<string>().Aggregate("", (current, item) => current + $"{item},").TrimEnd(','))
                {
                    ParagraphAlignment = ParagraphAlignment.Center,
                    TextAntialiasMode = TextAntialiasMode.Grayscale,
                    FontWeight = FontWeight.SemiBold
                },
                                    Font, ActiveFontBrush, null, Rect.X + 4, Rect.Y - 1, Rect.Width - 17, 21);

                g.Graphics.DrawLine(ActiveFontBrush, new Point(Rect.X + Rect.Width - 5, Rect.Y + 8), new Point(Rect.X + Rect.Width - 9, Rect.Y + 12), 2); //arrow
                g.Graphics.DrawLine(ActiveFontBrush, new Point(Rect.X + Rect.Width - 12, Rect.Y + 8), new Point(Rect.X + Rect.Width - 8, Rect.Y + 12), 2); //arrow

                g.Graphics.FillRectangle(Separator, Rect.X, Rect.Y + 21, Rect.Width, 1); //separator

                for (var i = 0; i < (Items.Count > _maxItemsVisible ? _maxItemsVisible : Items.Count); i++)
                {
                    if (_currentHighlightItem == i)
                    {
                        g.Graphics.FillRectangle(ItemOverHighlight, Rect.X + 4, Rect.Y + 21 + i * 21, Rect.Width - 8 - (Items.Count <= _maxItemsVisible ? 0 : 9), 21);
                    }
                    g.Graphics.DrawText(new TextHelper(Items[i + _currentPosition].ToString())
                    {
                        ParagraphAlignment = ParagraphAlignment.Center,
                        TextAntialiasMode = TextAntialiasMode.Grayscale,
                        FontWeight = FontWeight.SemiBold
                    },
                                        Font, SelectedIndexes.Contains(i + _currentPosition) ? ActiveFontBrush : FontBrush, null, Rect.X + 8, Rect.Y + 21 + i * 21, Rect.Width - 17, 21);
                }

                if (Items.Count > _maxItemsVisible)
                {
                    g.Graphics.FillRectangle(ScrollBarFill, Rect.X + Rect.Width - 10, Rect.Y + 25, 3, Rect.Height - 29);

                    var scrollHeight = Rect.Height - 29;
                    var scrollOnePieceHeight = scrollHeight / (float)Items.Count;
                    var scrollActiveHeight = scrollOnePieceHeight * 7;
                    g.Graphics.FillRectangle(ScrollBarActive, Rect.X + Rect.Width - 10, Rect.Y + 25 + scrollOnePieceHeight * _currentPosition, 3, scrollActiveHeight);
                }
            }
            else
            {
                Rect.Height = Height;

                g.Graphics.OutlineFillRectangle(Border, Fill, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);

                g.Graphics.DrawText(new TextHelper(SelectedItems.Cast<string>().Aggregate("", (current, item) => current + $"{item},").TrimEnd(','))
                {
                    ParagraphAlignment = ParagraphAlignment.Center,
                    TextAntialiasMode = TextAntialiasMode.Grayscale,
                    FontWeight = FontWeight.SemiBold
                },
                                    Font, ActiveFontBrush, null, Rect.X + 4, Rect.Y - 1, Rect.Width, 21);

                g.Graphics.DrawLine(ActiveFontBrush, new Point(Rect.X + Rect.Width - 5, Rect.Y + 8), new Point(Rect.X + Rect.Width - 9, Rect.Y + 12), 2); //arrow
                g.Graphics.DrawLine(ActiveFontBrush, new Point(Rect.X + Rect.Width - 12, Rect.Y + 8), new Point(Rect.X + Rect.Width - 8, Rect.Y + 12), 2); //arrow
            }

            //base.Draw();
        }

        public override void AddChild(DxControl ctl)
        {

        }

        public override void RemoveChild(DxControl ctl)
        {

        }

        public override void RemoveChildAt(int i)
        {

        }

        #endregion
    }
}
