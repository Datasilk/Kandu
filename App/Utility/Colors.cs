using System;
using System.Drawing;

namespace Utility
{
    public class Colors
    {
        public string FromHexToRgba(string backgroundColor, double backgroundOpacity)
        {
            var converter = new ColorConverter();
            var color = (Color)converter.ConvertFromString(backgroundColor);
            var r = Convert.ToInt16(color.R);
            var g = Convert.ToInt16(color.G);
            var b = Convert.ToInt16(color.B);
            return string.Format("rgba({0}, {1}, {2}, {3});", r, g, b, backgroundOpacity);
        }
    }
}
