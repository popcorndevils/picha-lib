using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace PichaLib
{
    public class Canvas
    {
        // helper for creating sprites
        private SolidBrush _Brush = new SolidBrush(Color.FromArgb(0, 255, 255, 255));

        public List<Layer> Layers = new List<Layer>();
        public (int W, int H) Size = (16, 16);
        public int[] FrameCounts {
            get {
                var _val = new int[this.Layers.Count];
                for(int i = 0; i < this.Layers.Count; i++)
                {
                    _val[i] = this.Layers[i].Frames.Count;
                }
                return _val;
            }
        }

        public (int MinX, int MinY, int MaxX, int MaxY) Extents {
            get {
                var _minX = 0;
                var _minY = 0;
                var _maxX = this.Size.W;
                var _maxY = this.Size.H;
                foreach(Layer l in this.Layers)
                {
                    _minX = Math.Min(_minX, l.X);
                    _minY = Math.Min(_minY, l.Y);
                    _maxX = Math.Max(_maxX, l.X + l.Size.W);
                    _maxY = Math.Max(_maxY, l.Y + l.Size.H);
                }
                return (_minX, _minY, _maxX, _maxY);
            }
        }

        public (int W, int H) TrueSize {
            get {
                var _ext = this.Extents;
                return (_ext.MaxX - _ext.MinX, _ext.MaxY - _ext.MinY);
            }
        }

        public Bitmap Generate()
        {
            // TODO: move sprite generation to this function.
            var _output = new List<Bitmap>();
            var _frameNums = this.FrameCounts;
            var _totalFrames = ExMath.LCD(_frameNums);

            var _spriteFrame = new Bitmap(this.Size.W, this.Size.H, PixelFormat.Format32bppArgb);

            using (var _gfx = Graphics.FromImage(_spriteFrame))
            {
                _gfx.FillRectangle(this._Brush, 0, 0, this.Size.W, this.Size.H);
                _gfx.CompositingMode = CompositingMode.SourceOver;

                foreach(Layer l in this.Layers)
                {
                    var _imgs = l.Generate();
                    _gfx.DrawImageUnscaled(_imgs[0], new Point(l.X, l.Y));
                }
            }

            return _spriteFrame;
        }

        // useful for the app only.
        public bool AutoGen = false;
        public float TimeToGen = 1f;
        public float AnimTime = 3f;
        public Chroma TransparencyFG = Chroma.CreateFromHex("#298c8c8c");
        public Chroma TransparencyBG = new Chroma(.1f, .1f, .1f, 0f);
    }
}