using System;
using System.Drawing;

namespace Utility
{
    public class Colors
    {
        public string FromHexToRgba(string backgroundColor, double backgroundOpacity)
        {
            var converter = new ColorConverter();
            var color = FromHex(backgroundColor);
            var r = Convert.ToInt16(color.R);
            var g = Convert.ToInt16(color.G);
            var b = Convert.ToInt16(color.B);
            return string.Format("rgba({0}, {1}, {2}, {3});", r, g, b, backgroundOpacity);
        }

        public Color FromHex(string hex)
        {
            var converter = new ColorConverter();
            return (Color)converter.ConvertFromString((hex.IndexOf('#') < 0 ? "#" : "") + hex);
        }

        public string FromColorToHex(Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = color.R;
            float green = color.G;
            float blue = color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public string ChangeHexBrightness(string hex, float correctionFactor)
        {
            return FromColorToHex(ChangeColorBrightness(FromHex(hex), correctionFactor));
        }
    }
}
