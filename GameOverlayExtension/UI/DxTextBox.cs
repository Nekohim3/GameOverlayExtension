using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Windows.Forms;
using GameOverlay.Drawing;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;
using Font = GameOverlay.Drawing.Font;

namespace GameOverlayExtension.UI
{
    public class DxTextBox : DxControl
    {
        #region Events

        //public delegate void ButtonEventHandler(DxButton btn);

        //public event ButtonEventHandler Click;

        //public virtual void OnClick(DxButton btn)
        //{
        //    Click?.Invoke(btn);
        //}

        #endregion

        public bool IsFocused { get; set; }

        private int _caretPos;

        public int CaretPos
        {
            get => _caretPos;
            set
            {
                _caretPos = value;
                _drawCaret = true;
                CaretChangeTime = DateTime.Now;
            }
        }

        public SolidBrush FontBrush { get; set; }
        public Font Font { get; set; }

        public SolidBrush FillBrush { get; set; }
        public SolidBrush StrokeBrush { get; set; }
        public SolidBrush FocusedFillBrush { get; set; }
        public SolidBrush FocusedStrokeBrush { get; set; }

        public TextHelper Text { get; set; }

        private DateTime CaretChangeTime { get; set; }
        private bool _drawCaret;

        public DxTextBox(string name, string text) : base(name)
        {
            Width = 100;
            Height = 22;
            Margin = new Thickness(11, 11, 1, 1);
            CaretChangeTime = DateTime.Now;

            Text = new TextHelper(text)
            {
                TextAlignment      = TextAlignment.Leading,
                ParagraphAlignment = ParagraphAlignment.Center,
                WordWrapping       = WordWrapping.NoWrap
            };


            Font = FontCollection.Get("Control.Font").Font;
            FontBrush = BrushCollection.Get("Control.Font").Brush;

            FillBrush = BrushCollection.Get("Control.Fill").Brush;
            StrokeBrush = BrushCollection.Get("Control.Stroke").Brush;
            FocusedFillBrush = BrushCollection.Get("TextBox.Focused.Fill").Brush;
            FocusedStrokeBrush = BrushCollection.Get("TextBox.Focused.Stroke").Brush;

        }

        public override void Draw()
        {
            if (IsFocused)
                g.Graphics.OutlineFillRectangle(FocusedStrokeBrush, FocusedFillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);
            else
                g.Graphics.OutlineFillRectangle(StrokeBrush, FillBrush, Rect.X, Rect.Y, Rect.Width, Rect.Height, 1, 0);

            //var layout = g.Graphics.GetLayout(Text, Font);
            //RawBool q11 = false;
            //RawBool q12 = false;
            //float q21 = 0;
            //float q22 = 0;
            //var q1 = layout.HitTestPoint(100, 0, out q11, out q12);//click text position
            //var q2 = layout.HitTestTextPosition(0, false, out q21, out q22);
            //var q3 = layout.HitTestTextRange(0, Text.Length, 0, 0);



            //var textMetrics = g.Graphics.MeasureText(Text, Font);
            //g.Graphics.FillRectangle(BrushCollection.Get("Test").Brush, Rect.X + 3, Rect.Y, textMetrics.Width, textMetrics.Height);
            
            //TODO: multiline, выделение (клавой, мышкой), ctrl + left-right?, home-end,ctrl + a-c-v, запилить норм drawtext с bold и тд 


            g.Graphics.DrawText(Text, Font, FontBrush, Rect.X + 2, Rect.Y, Rect.Width - 2, Rect.Height, true);

            if (IsFocused)
            {
                if ((DateTime.Now - CaretChangeTime).TotalMilliseconds > 750)
                {
                    _drawCaret = !_drawCaret;
                    CaretChangeTime = DateTime.Now;
                }

                if (_drawCaret)
                {

                    var textSize = g.Graphics.MeasureText(Text.Text.Substring(0, CaretPos), Font);
                    var caretX   = (float) Math.Round(textSize.Width,  MidpointRounding.ToEven);
                    var caretH   = (float) Math.Round(textSize.Height, MidpointRounding.ToEven);

                    caretH = Rect.Height / 2 - caretH / 2;
                    g.Graphics.DrawLine(StrokeBrush, Rect.X + caretX + 3, Rect.Y + caretH, Rect.X + caretX + 3, Rect.Y + Rect.Height - caretH, 1);
                }
            }
        }

        public override void OnMouseDown(DxControl sender, MouseEventArgs e, Point pt)
        {
            IsFocused = true;
            base.OnMouseDown(sender, e, pt);
        }

        public override void OnKeyDown(DxControl sender, KeyEventArgs e)
        {
            if (IsFocused)
            {
                if (e.KeyCode == Keys.Back)
                {
                    if (CaretPos > 0)
                    {
                        Text.Text = $"{Text.Text.Substring(0, CaretPos - 1)}{Text.Text.Substring(CaretPos)}";
                        CaretPos--;
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    if (CaretPos < Text.Text.Length)
                    {
                        Text.Text = $"{Text.Text.Substring(0, CaretPos)}{Text.Text.Substring(CaretPos + 1)}";
                    }
                }
                else if (e.KeyCode == Keys.Left)
                {
                    if (CaretPos > 0)
                        CaretPos--;
                }
                else if (e.KeyCode == Keys.Right)
                {
                    if (CaretPos < Text.Text.Length)
                        CaretPos++;
                }
                else
                {
                    var insert = KeyboardHelper.CodeToString(e.KeyValue);
                    if (insert != "")
                    {
                        Text.Text = $"{Text.Text.Substring(0, CaretPos)}{insert}{Text.Text.Substring(CaretPos)}";
                        CaretPos++;
                    }
                }
            }
        }
    }
}
