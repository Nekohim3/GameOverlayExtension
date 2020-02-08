using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameOverlay.Drawing;

namespace GameOverlayExtension
{
    public static class BrushCollection
    {
        private static readonly List<BrushItem> Brushes = new List<BrushItem>();

        public static void Init()
        {
#if DEBUG
            g.Config.RemoveAll("Brushes");
            Brushes.Clear();
#endif
            try
            {
                foreach (var q in g.Config.GetKeys("Brushes"))
                    Brushes.Add(new BrushItem(q));
            }
            catch
            {
                g.Config.RemoveAll("Brushes");
                Brushes.Clear();
            }
        }

        public static BrushItem Get(string name)
        {
            return Brushes.First(x => x.Name == name);
        }

        public static void Add(string name, uint def)
        {
            if (Brushes.Count(x => x.Name == name) == 0)
                Brushes.Add(new BrushItem(name, def));
        }
    }

    public class BrushItem
    {
        public SolidBrush Brush { get; set; }
        public string Name { get; set; }

        public BrushItem(string name)
        {
            Name = name;
            Brush = g.Graphics.CreateSolidBrush(g.Config.GetUInt("Brushes", Name));
        }

        public BrushItem(string name, uint def)
        {
            Name = name;
            Brush = g.Graphics.CreateSolidBrush(def);
            Save();
        }

        public void Set(int a, int r, int g, int b) => Set(new Color(r, g, b, a));

        public void Set(Color color) => Set(color.ToARGB());

        public void Set(uint color)
        {
            g.Config.Set("Brushes", Name, color);
            Brush.Color = Color.FromARGB(color);
        }

        public void LoadDefault() => Brush.Color = Color.FromARGB(g.Config.GetUInt("Brushes", Name, true));

        public void Load() => Brush.Color = Color.FromARGB(g.Config.GetUInt("Brushes", Name));

        public void Save() => g.Config.Set("Brushes", Name, Brush.Color.ToARGB());

    }
}
