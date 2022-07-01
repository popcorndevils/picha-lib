using System;
using System.Collections.Generic;
using SkiaSharp;

using OctavianLib;

namespace PichaLib
{
    public static class PFactory
    {
        public static Random Random = new Random();

        public static SKBitmap GenSpriteSheet(
            Canvas canvas, int cols, int rows, int scale, bool clip_content,
            bool random_start = false)
        {
            var _sprites = new SKBitmap[rows, cols];

            for(int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    _sprites[y, x] = canvas.GenerateSprite(clip_content, scale, random_start);
                }
            }

            (int W, int H) _size = (_sprites[0, 0].Width, _sprites[0, 0].Height);

            var _output = new SKBitmap(_size.W * cols, _size.H * rows);

            using(var gfx = new SKCanvas(_output))
            {
                gfx.Clear(SKColor.Empty);
                for(int x = 0; x < cols; x++)
                {
                    for(int y = 0; y < rows; y++)
                    {
                        gfx.DrawBitmap(_sprites[y, x], new SKPoint(x * _size.W, y * _size.H));
                    }
                }
            }

            return _output;
        }

        public static SKBitmap[] GenSpriteFrames(
            Canvas canvas, int cols, int rows, int scale, bool clip_content,
            bool random_start = false)
        {
            int _sheets = ExMath.LCD(canvas.FrameCounts);

            var _output = new SKBitmap[_sheets];

            SKBitmap[,][] _frames = new SKBitmap[cols, rows][];

            for(int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    _frames[y, x] = canvas.GenerateFrames(clip_content, scale, random_start);
                }
            }

            (int W, int H) _size = (_frames[0, 0][0].Width, _frames[0, 0][0].Height);

            for(int s = 0; s < _sheets; s++)
            {
                _output[s] = new SKBitmap(_size.W * cols, _size.H * rows);

                using(var gfx = new SKCanvas(_output[s]))
                {
                    gfx.Clear(SKColor.Empty);
                    for(int x = 0; x < cols; x++)
                    {
                        for(int y = 0; y < rows; y++)
                        {
                            gfx.DrawBitmap(_frames[y, x][s], new SKPoint(x * _size.W, y * _size.H));
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