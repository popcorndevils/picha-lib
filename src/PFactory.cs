using System;
using System.Drawing;
using System.Collections.Generic;
using SkiaSharp;
using CompositingMode = System.Drawing.Drawing2D.CompositingMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

using OctavianLib;

namespace PichaLib
{
    public static class PFactory
    {
        public static Random Random = new Random();
        private static SolidBrush _transparent = new SolidBrush(Color.FromArgb(0, 255, 255, 255));

        public static Bitmap GenSpriteSheet(Canvas canvas, int cols, int rows, int scale, bool clip_content, bool random_start = false)
        {
            var _sprites = new Bitmap[rows, cols];

            for(int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    _sprites[y, x] = canvas.GenerateSprite(clip_content, scale, random_start);
                }
            }

            (int W, int H) _size = (_sprites[0, 0].Width, _sprites[0, 0].Height);

            var _output = new Bitmap(_size.W * cols, _size.H * rows, PixelFormat.Format32bppArgb);

            using(Graphics _gfx = Graphics.FromImage(_output))
            {
                _gfx.FillRectangle(PFactory._transparent, 0, 0, _size.W * cols, _size.H * rows);
                _gfx.CompositingMode = CompositingMode.SourceOver;
                for(int x = 0; x < cols; x++)
                {
                    for(int y = 0; y < rows; y++)
                    {
                        _gfx.DrawImageUnscaled(_sprites[y, x], x * _size.W, y * _size.H);
                    }
                }
            }

            return _output;
        }

        public static Bitmap[] GenSpriteFrames(Canvas canvas, int cols, int rows, int scale, bool clip_content, bool random_start = false)
        {
            int _sheets = ExMath.LCD(canvas.FrameCounts);

            var _output = new Bitmap[_sheets];

            Bitmap[,][] _frames = new Bitmap[cols, rows][];

            for(int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    _frames[y, x] = canvas.GenerateFrames_OLD(clip_content, scale, random_start);
                }
            }

            (int W, int H) _size = (_frames[0, 0][0].Width, _frames[0, 0][0].Height);

            for(int s = 0; s < _sheets; s++)
            {
                _output[s] = new Bitmap(_size.W * cols, _size.H * rows, PixelFormat.Format32bppArgb);

                using(Graphics _gfx = Graphics.FromImage(_output[s]))
                {
                    _gfx.FillRectangle(PFactory._transparent, 0, 0, _size.W, _size.H);
                    _gfx.CompositingMode = CompositingMode.SourceOver;
                    for(int x = 0; x < cols; x++)
                    {
                        for(int y = 0; y < rows; y++)
                        {
                            _gfx.DrawImageUnscaled(_frames[y, x][s], x * _size.W, y * _size.H);
                        }
                    }
                }
            }

            return _output;
        }

        public static Dictionary<string, PixelColors> PickColors(Dictionary<string, Pixel> pixels)
        {
            var _output = new Dictionary<string, PixelColors>();

            foreach(Pixel _type in pixels.Values)
            {
                var _dat = new PixelColors() {
                    FadeDirection = pixels[_type.Name].FadeDirection,
                    BrightNoise = pixels[_type.Name].BrightNoise,
                };
                
                if(_type.RandomCol)
                {
                    _dat.RGB = new Chroma(
                        (float)PFactory.Random.NextDouble(),
                        (float)PFactory.Random.NextDouble(),
                        (float)PFactory.Random.NextDouble());
                }
                else
                    { _dat.RGB = _type.Color; }

                _dat.Sat = (float)PFactory.Random.RandfRange(_type.MinSaturation * _dat.HSL.s, _dat.HSL.s);

                _output.Add(_type.Name, _dat);
            }

            return _output;
        }
    }
}