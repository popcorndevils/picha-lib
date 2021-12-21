using System;
using System.Collections.Generic;

using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using SolidBrush = System.Drawing.SolidBrush;
using Color = System.Drawing.Color;
using CompositingMode = System.Drawing.Drawing2D.CompositingMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

using OctavianLib;

namespace PichaLib
{
    public delegate void LayerChangeHandler(Layer layer, bool major);

    public class Layer
    {
        public event LayerChangeHandler LayerChanged;
        private CellData _ColorData;
        // helper for creating sprites
        private SolidBrush _Brush = new SolidBrush(Color.FromArgb(0, 255, 255, 255));

        private string _Name;
        public string Name {
            get => this._Name;
            set {
                this._Name = value;
                this.LayerChanged?.Invoke(this, false);
            }
        }

        private bool _MirrorX, _MirrorY;

        public bool MirrorX {
            get => this._MirrorX;
            set {
                this._MirrorX = value;
                this.LayerChanged?.Invoke(this, true);
            }
        }

        public bool MirrorY {
            get => this._MirrorY;
            set {
                this._MirrorY = value;
                this.LayerChanged?.Invoke(this, true);
            }
        }

        private int _X, _Y;
        public int X {
            get => this._X;
            set {
                this._X = value;
                this.LayerChanged?.Invoke(this, false);
            }
        }
        public int Y {
            get => this._Y;
            set {
                this._Y = value;
                this.LayerChanged?.Invoke(this, false);
            }
        }

        public (int W, int H) Size {
            get {
                foreach(Frame f in this._Frames)
                {
                    return (f.Data.GetWidth() * (this.MirrorX ? 2 : 1) ,
                            f.Data.GetHeight() * (this.MirrorY ? 2 : 1));
                }
                return (0, 0);
            }
        }
        
        public (int X, int Y) Position {
            get => (this.X, this.Y);
            set {
                if(this.X != value.X || this.Y != value.Y)
                {
                    this._X = value.X;
                    this._Y = value.Y;
                    this.LayerChanged?.Invoke(this, false);
                }
            }
        }

        public int FramesCount {
            get {
                // NOTE: don't return values based on timing anymore
                return this.Frames.Count;
            }
        }

        private List<Frame> _Frames = new List<Frame>();
        public List<Frame> Frames {
            get => this._Frames;
            set {
                this._Frames = value;
                this.LayerChanged?.Invoke(this, true);
            }
        }
        
        private List<Cycle> _Cycles = new List<Cycle>();
        public List<Cycle> Cycles {
            get => this._Cycles;
            set {
                this._Cycles = value;
                this.LayerChanged?.Invoke(this, true);
            }
        }

        public Bitmap[] Generate(Dictionary<string, PixelColors> colors)
        {
            return this.GenShape().GenColor(colors);
        }

        public Bitmap GenerateSheet(Dictionary<string, PixelColors> colors)
        {
            var _frames = this.Generate(colors);
            (int W, int H) _s = (_frames[0].Width, _frames[0].Height);
            var _output = new Bitmap(_s.W * _frames.Length, _s.H, PixelFormat.Format32bppArgb);

            using (var _gfx = Graphics.FromImage(_output))
            {
                _gfx.FillRectangle(this._Brush, 0, 0, _s.W, _s.H);
                _gfx.CompositingMode = CompositingMode.SourceOver;

                for(int i = 0; i < _frames.Length; i++)
                {
                    var _f = _frames[i];
                    _gfx.DrawImageUnscaled(_f, (_s.W * i), 0);
                }
            }

            return _output;
        }

        public Layer GenShape()
        {
            var _output = this.Copy();
            _output.Frames = new List<Frame>();
            foreach(Frame _f in this.Frames)
            {
                _output.Frames.Add(_f.RunCycles(this.Cycles));
            }
            return _output;
        }

        public Bitmap[] GenColor(Dictionary<string, PixelColors> colors)
        {
            var _output = new Bitmap[this.FramesCount];

            var _colors = new CellData() {
                MirrorX = this.MirrorX,
                MirrorY = this.MirrorY,
                Pixels = colors,
            };

            for(int i = 0; i < this.FramesCount; i++)
            {
                _output[i] = this.Frames[i].Generate(_colors);
            }

            return _output;
        }
    }

    public enum FadeDirection
    {
        NONE,
        NORTH,
        SOUTH,
        EAST,
        WEST,
        RANDOM
    }
}