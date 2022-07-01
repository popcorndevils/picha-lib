using System;
using System.Collections.Generic;
using SkiaSharp;

namespace PichaLib
{
    public class Canvas
    {
        // main gen vars
        public List<Layer> Layers = new List<Layer>();
        public (int W, int H) Size = (16, 16);

        // used in app
        public bool AutoGen = true;
        public float TimeToGen = 2f;
        public float AnimTime = 1f;
        public Chroma TransparencyFG = Chroma.CreateFromHex("#298c8c8c");
        public Chroma TransparencyBG = new Chroma(.1f, .1f, .1f, 0f);

        private Dictionary<string, Pixel> _Pixels = new Dictionary<string, Pixel>();
        public Dictionary<string, Pixel> Pixels {
            get => this._Pixels;
            set {
                this._Pixels = value;
            }
        }

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

        // public SKBitmap[] GenerateFrames_OLD(bool clip_content = true, int scale = 1, bool random_start = false)
        // {
        //     var _totalFrames = ExMath.LCD(this.FrameCounts);
        //     var _output = new SKBitmap[_totalFrames];
        //     var _size = clip_content ? this.Size : this.TrueSize;

        //     var _colors = PFactory.PickColors(this.Pixels);

        //     (int X, int Y) _offset = clip_content ? (0, 0) : (Math.Abs(this.Extents.MinX), Math.Abs(this.Extents.MinY));

        //     // do as two separate loops for now, can merge loops for efficiency if needed.

        //     for(int i = 0; i < _totalFrames; i++)
        //     {
        //         _output[i] = new SKBitmap(_size.W, _size.H);
        //         using(var canvas = new SKCanvas(_output[i]))
        //         {
        //             canvas.Clear(SKColor.Empty);
        //         }
        //     }
            
        //     foreach(Layer l in this.Layers)
        //     {
        //         var _imgs = l.Generate(_colors);
        //         int _copies_per_frame = _totalFrames / _imgs.Length;
        //         int _times_copied = 0;

        //         foreach(SKBitmap _f in _imgs)
        //         {
        //             for(int i = 0; i < _copies_per_frame; i++)
        //             {   
        //                 using(var canvas = new SKCanvas(_output[_times_copied]))
        //                 {
        //                     canvas.DrawBitmap(_f, new SKPoint(l.X + _offset.X, l.Y + _offset.Y));
        //                 }
        //                 _times_copied++;
        //             }
        //         }
        //     }

        //     if(random_start)
        //     {
        //         var _rand = new Random();
        //         var _start = _rand.Next(_output.Length);
        //         var _new = new SKBitmap[_output.Length];

        //         for(int i = 0; i < _output.Length; i++)
        //         {
        //             var _index = i + _start;
        //             if(_index >= _output.Length)
        //             {
        //                 _index -= _output.Length;
        //             }
        //             _new[i] = _output[_index];
        //         }
        //         _output = _new;
        //     }

        //     if(scale != 1)
        //     {
        //         for(int i = 0; i < _output.Length; i++)
        //         {
        //             var _f = _output[i];
        //             var _new_img = new SKBitmap(_f.Width * scale, _f.Height * scale);

        //             using(var canvas = new SKCanvas(_new_img))
        //             {
        //                 canvas.Clear(SKColor.Empty);
        //             }
        //             _f.ScalePixels(_new_img, SKFilterQuality.None);

        //             _output[i] = _new_img;
        //         }
        //     }

        //     return _output;
        // }
        
        public SKBitmap[] GenerateFrames(bool clip_content = true, int scale = 1, bool random_start = false)
        {
            var _totalFrames = ExMath.LCD(this.FrameCounts);
            var _output = new SKBitmap[_totalFrames];
            var _size = clip_content ? this.Size : this.TrueSize;

            var _colors = PFactory.PickColors(this.Pixels);

            (int X, int Y) _offset = clip_content ? (0, 0) : (Math.Abs(this.Extents.MinX), Math.Abs(this.Extents.MinY));

            // do as two separate loops for now, can merge loops for efficiency if needed.

            for(int i = 0; i < _totalFrames; i++)
            {
                _output[i] = new SKBitmap(_size.W, _size.H);
                using(var canvas = new SKCanvas(_output[i]))
                {
                    canvas.Clear(SKColor.Empty);
                }
            }
            
            foreach(Layer l in this.Layers)
            {
                var _imgs = l.Generate(_colors);
                int _copies_per_frame = _totalFrames / _imgs.Length;
                int _times_copied = 0;

                foreach(SKBitmap _f in _imgs)
                {
                    for(int i = 0; i < _copies_per_frame; i++)
                    {   
                        using(var canvas = new SKCanvas(_output[_times_copied]))
                        {
                            canvas.DrawBitmap(_f,
                                new SKPoint(l.X + _offset.X, l.Y + _offset.Y));
                        }
                        _times_copied++;
                    }
                }
            }

            if(random_start)
            {
                var _rand = new Random();
                var _start = _rand.Next(_output.Length);
                var _new = new SKBitmap[_output.Length];

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
                    var _new_img = new SKBitmap(_f.Width * scale, _f.Height * scale);
                    using(var canvas = new SKCanvas(_new_img))
                    {
                        canvas.Clear(SKColor.Empty);
                    }
                    _f.ScalePixels(_new_img, SKFilterQuality.None);

                    _output[i] = _new_img;
                }
            }

            return _output;
        }

        public SKBitmap GenerateSprite(bool clip_content = true, int scale = 1, bool random_start = false)
        {
            var _frames = this.GenerateFrames(clip_content, scale, random_start);
            (int W, int H) _s = (_frames[0].Width, _frames[0].Height);
            
            var _output = new SKBitmap(_s.W * _frames.Length, _s.H);

            using(var canvas = new SKCanvas(_output))
            {
                canvas.Clear(SKColor.Empty);
                for(int i = 0; i < _frames.Length; i++)
                {
                    var _f = _frames[i];
                    canvas.DrawBitmap(_f, new SKPoint((_s.W * i), 0));
                }
            }

            return _output;
        }
    }
}