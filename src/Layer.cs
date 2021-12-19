using System;
using System.Collections.Generic;

using Bitmap = System.Drawing.Bitmap;

using OctavianLib;

namespace PichaLib
{
    public delegate void LayerChangeHandler(Layer layer, bool major);

    public class Layer
    {
        public event LayerChangeHandler LayerChanged;
        private CellData _ColorData;

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

        private Dictionary<string, Pixel> _Pixels = new Dictionary<string, Pixel>();
        public Dictionary<string, Pixel> Pixels {
            get => this._Pixels;
            set {
                this._Pixels = value;
                this.LayerChanged?.Invoke(this, false);
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

        public Bitmap[] Generate()
        {
            return this.GenShape().GenColor();
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

        public Bitmap[] GenColor()
        {
            var _output = new Bitmap[this.FramesCount];
            var _colors = this.PickColors();

            for(int i = 0; i < this.FramesCount; i++)
            {
                _output[i] = this.Frames[i].Generate(_colors);
            }

            return _output;
        }

        public CellData PickColors()
        {
            var _output = new CellData() {
                MirrorX = this.MirrorX,
                MirrorY = this.MirrorY,
                Pixels = new Dictionary<string, PixelColors>(),
            };

            foreach(Pixel _type in this.Pixels.Values)
            {
                var _dat = new PixelColors() {
                    FadeDirection = this.Pixels[_type.Name].FadeDirection,
                    BrightNoise = this.Pixels[_type.Name].BrightNoise,
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

                _output.Pixels.Add(_type.Name, _dat);
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