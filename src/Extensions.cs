using SkiaSharp;

namespace PichaLib
{
    public static class BitmapExtensions
    {
        // public static SKBitmap[] ToSkia(this Bitmap[] bmps)
        // {
        //     var output = new SKBitmap[bmps.Length];
        //     for(int i = 0; i < bmps.Length; i++)
        //     {
        //         output[i] = bmps[i].ToSkia();
        //     }
        //     return output;
        // }

        // public static SKBitmap ToSkia(this Bitmap bmp)
        // {
        //     var output = new SKBitmap(bmp.Width, bmp.Height);
        //     for(int x = 0; x < bmp.Width; x++)
        //     {
        //         for(int y = 0; y < bmp.Height; y++)
        //         {
        //             var c = bmp.GetPixel(x, y);
        //             output.SetPixel(x, y, c.ToSkia());
        //         }
        //     }
        //     return output;
        // }

        // public static SKColor ToSkia(this Color c)
        // {
        //     return new SKColor(c.R, c.G, c.B, c.A);
        // }

        // public static Bitmap[] ToSystem(this SKBitmap[] bmps)
        // {
        //     var output = new Bitmap[bmps.Length];
        //     for(int i = 0; i < bmps.Length; i++)
        //     {
        //         output[i] = bmps[i].ToSystem();
        //     }
        //     return output;
        // }

        // public static Bitmap ToSystem(this SKBitmap bmp)
        // {
        //     var output = new Bitmap(bmp.Width, bmp.Height);            
        //     for(int x = 0; x < bmp.Width; x++)
        //     {
        //         for(int y = 0; y < bmp.Height; y++)
        //         {
        //             var c = bmp.GetPixel(x, y);
        //             output.SetPixel(x, y, c.ToSystem());
        //         }
        //     }
        //     return output;
        // }

        // public static Color ToSystem(this SKColor c)
        // {
        //     return Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
        // }
    }
}