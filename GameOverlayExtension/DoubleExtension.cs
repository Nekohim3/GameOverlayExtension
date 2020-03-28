using System;

namespace GameOverlayExtension
{
    public static class DoubleExtension
    {
        public static bool CloseTo(this double value1, double value2)
        {
            return Math.Abs(value1 - value2) < 0.0001;
        }
    }
}