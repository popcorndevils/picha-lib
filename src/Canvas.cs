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

        // main gen vars
        public List<Layer> Layers = new List<Layer>();
        public (int W, int H) Size = (16, 16);

        // used in app
        public bool AutoGen = true;
        public float TimeToGen = 2f;
        public float AnimTime = 1f;
        public Chroma TransparencyFG = Chroma.CreateFromHex("#298c8c8c");
        public Chroma TransparencyBG = new Chroma(.1f, .1f, .1f, 0f);

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

        public Bitmap[] GenerateFrames(bool clip_content = true, int scale = 1, bool random_start = false)
        {
            var _totalFrames = ExMath.LCD(this.FrameCounts);
            var _output = new Bitmap[_totalFrames];
            var _size = clip_content ? this.Size : this.TrueSize;
            (int X, int Y) _offset = clip_content ? (0, 0) : (Math.Abs(this.Extents.MinX), Math.Abs(this.Extents.MinY));

            // do as two separate loops for now, can merge loops for efficiency if needed.

            for(int i = 0; i < _totalFrames; i++)
            {
                _output[i] = new Bitmap(_size.W, _size.H, PixelFormat.Format32bppArgb);
                using(var _gfx = Graphics.FromImage(_output[i]))
                {
                    _gfx.FillRectangle(this._Brush, 0, 0, _size.W, _size.H);
                }
            }
            
            foreach(Layer l in this.Layers)
            {
                var _imgs = l.Generate();
                int _copies_per_frame = _totalFrames / _imgs.Length;
                int _times_copied = 0;

                foreach(Bitmap _f in _imgs)
                {
                    for(int i = 0; i < _copies_per_frame; i++)
                    {   
                        using(var _gfx = Graphics.FromImage(_output[_times_copied]))
                        {
                            _gfx.CompositingMode = CompositingMode.SourceOver;

                            _gfx.DrawImageUnscaled(_f, (l.X + _offset.X), (l.Y + _offset.Y));

                            _times_copied++;
                        }
                    }
                }
            }

            if(random_start)
            {
                var _rand = new Random();
                var _start = _rand.Next(_output.Length);
                var _new = new Bitmap[_output.Length];

                for(int i = 0; i < _output.Length; i++)
                {
                    var _index = i + _start;
                    if(_index >= _output.Length)
                    {
                        _index -= _output.Length;
                    }
                    _new[i] = _output[_index];
                }
                _output = _new;
            }

            if(scale != 1)
            {
                for(int i = 0; i < _output.Length; i++)
                {
                    var _f = _output[i];
                    var _new_img = new Bitmap(_f.Width * scale, _f.Height * scale);

                    using(Graphics _gfx = Graphics.FromImage(_new_img))
                    {
                        _gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
                        _gfx.PixelOffsetMode = PixelOffsetMode.Half;
                        _gfx.DrawImage(_f, 0, 0, _new_img.Width, _new_img.Height);
                    }

                    _output[i] = _new_img;
                }
            }

            return _output;
        }

        public Bitmap GenerateSprite(bool clip_content = true, int scale = 1, bool random_start = false)
        {
            var _frames = this.GenerateFrames(clip_content, scale, random_start);
            (int W, int H) _s = (_frames[0].Width, _frames[0].Height);
            
            var _output = new Bitmap(_s.W * _frames.Length, _s.H, PixelFormat.Format32bppArgb);

            using (var _gfx = Graphics.FromImage(_output))
            {
                _gfx.FillRectangle(this._Brush, 0, 0, _s.W, _s.H);
                _gfx.CompositingMode = CompositingMode.SourceOver;

                for(int i = 0; i < _frames.Length; i++)
                {
                    var _f = _frames[i];
                    var _pos = new Point((_s.W * i), 0);
                    _gfx.DrawImageUnscaled(_f, _pos);
                }
            }

            return _output;
        }
    }
}