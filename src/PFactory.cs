using System;
using System.Drawing;

using CompositingMode = System.Drawing.Drawing2D.CompositingMode;

namespace PichaLib
{
    public static class PFactory
    {
        public static Random Random = new Random();
        private static SolidBrush _transparent = new SolidBrush(Color.FromArgb(0, 255, 255, 255));

        public static Bitmap GenerateSpriteSheet(Canvas canvas, int cols, int rows, int scale, bool clip_content)
        {
            var _sprites = new Bitmap[rows, cols];

            for(int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    _sprites[y, x] = canvas.GenerateSprite(clip_content, scale);
                }
            }

            (int W, int H) _size = (_sprites[0, 0].Width, _sprites[0, 0].Height);

            var _output = new Bitmap(_size.W * cols, _size.H * rows);

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

        public static Bitmap[] GenerateFrameSheets(Canvas canvas, int cols, int rows, int scale, bool clip_content)
        {
            int _sheets = ExMath.LCD(canvas.FrameCounts);

            var _output = new Bitmap[_sheets];

            Bitmap[,][] _frames = new Bitmap[cols, rows][];

            for(int x = 0; x < cols; x++)
            {
                for(int y = 0; y < rows; y++)
                {
                    _frames[y, x] = canvas.GenerateFrames(clip_content, scale);
                }
            }

            (int W, int H) _size = (_frames[0, 0][0].Width, _frames[0, 0][0].Height);

            for(int s = 0; s < _sheets; s++)
            {
                _output[s] = new Bitmap(_size.W * cols, _size.H * rows);

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
    }
}