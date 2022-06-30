using System.Drawing;
using SkiaSharp;

namespace PichaLib
{
    public static class BitmapExtensions
    {
        public static SKBitmap[] ToSkia(this Bitmap[] array)
        {
            var output = new SKBitmap[array.Length];
            for(int i = 0; i < array.Length; i++)
            {
                output[i] = array[i].ToSkia();
            }
            return output;
        }

        public static SKBitmap ToSkia(this Bitmap img)
        {
            var output = new SKBitmap(img.Width, img.Height);
            for(int x = 0; x < img.Width; x++)
            {
                for(int y = 0; y < img.Height; y++)
                {
                    var c = img.GetPixel(x, y);
                    output.SetPixel(x, y, c.ToSkia());
                }
            }
            return output;
        }

        public static SKColor ToSkia(this Color c)
        {
            return new SKColor(c.R, c.G, c.B, c.A);
        }
    }
}