using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOverlayExtension
{
    public static class FloatUtils
    {
        public static bool CloseTo(this float value1, float value2) => Math.Abs(value1 - value2) < 0.000001;
    }
}
