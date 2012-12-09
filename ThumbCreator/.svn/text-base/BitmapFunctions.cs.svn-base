using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace ThumbCreator
{
    class BitmapFunctions
    {

        public static Bitmap Resize(Bitmap b, int nWidth, int nHeight, bool bBilinear)
        {
            Bitmap bTemp = (Bitmap)b.Clone();
            b = new Bitmap(nWidth, nHeight, bTemp.PixelFormat);

            double nXFactor = (double)bTemp.Width / (double)nWidth;
            double nYFactor = (double)bTemp.Height / (double)nHeight;

            if (bBilinear)
            {
                double fraction_x, fraction_y, one_minus_x, one_minus_y;
                int ceil_x, ceil_y, floor_x, floor_y;
                Color c1 = new Color();
                Color c2 = new Color();
                Color c3 = new Color();
                Color c4 = new Color();
                byte red, green, blue;

                byte b1, b2;

                for (int x = 0; x < b.Width; ++x)
                    for (int y = 0; y < b.Height; ++y)
                    {
                        // Setup

                        floor_x = (int)Math.Floor(x * nXFactor);
                        floor_y = (int)Math.Floor(y * nYFactor);
                        ceil_x = floor_x + 1;
                        if (ceil_x >= bTemp.Width) ceil_x = floor_x;
                        ceil_y = floor_y + 1;
                        if (ceil_y >= bTemp.Height) ceil_y = floor_y;
                        fraction_x = x * nXFactor - floor_x;
                        fraction_y = y * nYFactor - floor_y;
                        one_minus_x = 1.0 - fraction_x;
                        one_minus_y = 1.0 - fraction_y;

                        c1 = bTemp.GetPixel(floor_x, floor_y);
                        c2 = bTemp.GetPixel(ceil_x, floor_y);
                        c3 = bTemp.GetPixel(floor_x, ceil_y);
                        c4 = bTemp.GetPixel(ceil_x, ceil_y);

                        // Blue
                        b1 = (byte)(one_minus_x * c1.B + fraction_x * c2.B);
                        b2 = (byte)(one_minus_x * c3.B + fraction_x * c4.B);
                        blue = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                        // Green
                        b1 = (byte)(one_minus_x * c1.G + fraction_x * c2.G);
                        b2 = (byte)(one_minus_x * c3.G + fraction_x * c4.G);
                        green = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                        // Red
                        b1 = (byte)(one_minus_x * c1.R + fraction_x * c2.R);
                        b2 = (byte)(one_minus_x * c3.R + fraction_x * c4.R);
                        red = (byte)(one_minus_y * (double)(b1) + fraction_y * (double)(b2));

                        b.SetPixel(x, y, System.Drawing.Color.FromArgb(255, red, green, blue));
                    }
            }
            else
            {
                for (int x = 0; x < b.Width; ++x)
                    for (int y = 0; y < b.Height; ++y)
                        b.SetPixel(x, y, bTemp.GetPixel((int)(Math.Floor(x * nXFactor)), (int)(Math.Floor(y * nYFactor))));
            }

            return b;
        }

        public static Bitmap FastResize(Bitmap b, int nWidth, int nHeight)
        {
            Bitmap result = new Bitmap(nWidth, nHeight);
            using (Graphics g = Graphics.FromImage((Image)result))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(b, 0, 0, nWidth, nHeight);
            }
            return result;
        }

    }
}
