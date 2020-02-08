using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameOverlay.Drawing;

namespace GameOverlayExtension
{
    public static class FontCollection
    {
        private static readonly List<FontItem> Fonts = new List<FontItem>();

        public static void Init()
        {
#if DEBUG
            g.Config.RemoveAll("Fonts");
            Fonts.Clear();
#endif
            try
            {
                foreach (var q in g.Config.GetKeys("Fonts"))
                    if (!q.EndsWith(".Size"))
                        Fonts.Add(new FontItem(q));
            }
            catch (Exception e)
            {
                g.Config.RemoveAll("Fonts");
                Fonts.Clear();
            }
        }

        public static FontItem Get(string name)
        {
            return Fonts.First(x => x.Name == name);
        }

        public static void Add(string name, string font, int size)
        {
            if (Fonts.Count(x => x.Name == name) == 0)
                Fonts.Add(new FontItem(name, font, size));
        }
    }

    public class FontItem
    {
        public Font Font { get; set; }
        public string Name { get; set; }

        public FontItem(string name)
        {
            Name = name;
            Font = new Font(g.Config.GetString("Fonts", Name), g.Config.GetInt("Fonts", $"{Name}.Size"));
        }

        public FontItem(string name, string fontName, int fontSize)
        {
            Name = name;
            Font = new Font(fontName, fontSize);
            SaveFont();
        }

        public void SetFont(string fontName, int fontSize)
        {
            Font.FontFamilyName = fontName;
            Font.FontSize = fontSize;
            g.Config.Set("Fonts", Name, fontName);
            g.Config.Set("Fonts", $"{Name}.Size", fontSize);
        }

        public void SetFontName(string fontName)
        {
            Font.FontFamilyName = fontName;
            g.Config.Set("Fonts", Name, fontName);
        }

        public void SetFontSize(int fontSize)
        {
            Font.FontSize = fontSize;
            g.Config.Set("Fonts", $"{Name}.Size", fontSize);
        }

        public void LoadFont()
        {
            Font.FontFamilyName = g.Config.GetString("Fonts", Name);
            Font.FontSize = g.Config.GetInt("Fonts", $"{Name}.Size");
        }

        public void LoadDefaultFont()
        {
            Font.FontFamilyName = g.Config.GetString("Fonts", Name, true);
            Font.FontSize = g.Config.GetInt("Fonts", $"{Name}.Size", true);
        }

        public void LoadDefaultFontSize() => Font.FontSize = g.Config.GetInt("Fonts", $"{Name}.Size", true);

        public void LoadDefaultFontName() => Font.FontFamilyName = g.Config.GetString("Fonts", Name, true);

        public void SaveFont()
        {
            g.Config.Set("Fonts", Name, Font.FontFamilyName);
            g.Config.Set("Fonts", $"{Name}.Size", Font.FontSize);
        }
    }
}
