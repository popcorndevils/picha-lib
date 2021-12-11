using System;
using SysColor = System.Drawing.Color;
using Bitmap = System.Drawing.Bitmap;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Collections.Generic;
using OctavianLib;

namespace PichaLib
{
    public static partial class PFactory
    {
        private static Random _Random = new Random();
        public static List<Bitmap> Generate(Canvas c) {return PFactory._Generate(c);}
        public static List<Bitmap> Generate(Layer l) { return PFactory._Generate(l); }
        internal static Bitmap Generate(Frame f, CellData data) { return PFactory._Generate(f, data); }
    }
}