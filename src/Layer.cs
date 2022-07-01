using System;
using System.Collections.Generic;

using SkiaSharp;

using Bitmap = System.Drawing.Bitmap;
using Graphics = System.Drawing.Graphics;
using SolidBrush = System.Drawing.SolidBrush;
using Color = System.Drawing.Color;
using CompositingMode = System.Drawing.Drawing2D.CompositingMode;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

using OctavianLib;

namespace PichaLib
{
    public delegate void LayerChangeHandler(LayerChangeEvent e);

    public struct LayerChangeEvent
    {
        public LayerChangeType Type;
        public Object OldValue;
        public Object NewValue;
        public Layer Sender;
        public bool Major;
    }

    public enum LayerChangeType
    {
        NULL, NAME, POSITION, FRAME, MIRROR_X, MIRROR_Y, CYCLES, NEW_LAYER
    }

    public class Layer
    {
        public event LayerChangeHandler LayerChanged;

        private string _Name;
        public string Name {
            get => this._Name;
            set {

                var _e = new LayerChangeEvent() {
                    Type = LayerChangeType.NAME,
                    OldValue = this._Name,
                    NewValue = value,
                    Sender = this,
                    Major = false,
                };

                this._Name = value;
                this.LayerChanged?.Invoke(_e);
            }
        }

        private bool _MirrorX, _MirrorY;

        public bool MirrorX {
            get => this._MirrorX;
            set {
                var _e = new LayerChangeEvent() {
                    Type = LayerChangeType.MIRROR_X,
                    OldValue = this._MirrorX,
                    NewValue = value,
                    Sender = this,
                    Major = true,
                };

                this._MirrorX = value;
                this.LayerChanged?.Invoke(_e);
            }
        }

        public bool MirrorY {
            get => this._MirrorY;
            set {
                var _e = new LayerChangeEvent() {
                    Type = LayerChangeType.MIRROR_Y,
                    OldValue = this._MirrorY,
                    NewValue = value,
                    Sender = this,
                    Major = true,
                };

                this._MirrorY = value;
                this.LayerChanged?.Invoke(_e);
            }
        }

        private int _X, _Y;
        public int X {
            get => this._X;
            set {
                var _e = new LayerChangeEvent() {
                    Type = LayerChangeType.POSITION,
                    OldValue = (this._X, this._Y),
                    NewValue = (value, this._Y),
                    Sender = this,
                    Major = false,
                };

                this._X = value;
                this.LayerChanged?.Invoke(_e);
            }
        }
        public int Y {
            get => this._Y;
            set {
                var _e = new LayerChangeEvent() {
                    Type = LayerChangeType.POSITION,
                    OldValue = (this._X, this._Y),
                    NewValue =  (this._X, value),
                    Sender = this,
                    Major = false,
                };

                this._Y = value;
                this.LayerChanged?.Invoke(_e);
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
                    var _e = new LayerChangeEvent() {
                        Type = LayerChangeType.POSITION,
                        OldValue = (this._X, this._Y),
                        NewValue =  (value.X, value.Y),
                        Sender = this,
                        Major = false,
                    };
                    
                    this._X = value.X;
                    this._Y = value.Y;
                    this.LayerChanged?.Invoke(_e);
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
                var _e = new LayerChangeEvent() {
                    Type = LayerChangeType.FRAME,
                    OldValue = this._Frames,
                    NewValue =  value,
                    Sender = this,
                    Major = true,
                };

                this._Frames = value;
                this.LayerChanged?.Invoke(_e);
            }
        }
        
        private List<Cycle> _Cycles = new List<Cycle>();
        public List<Cycle> Cycles {
            get => this._Cycles;
            set {
                var _e = new LayerChangeEvent() {
                    Type = LayerChangeType.CYCLES,
                    OldValue = this._Cycles,
                    NewValue =  value,
                    Sender = this,
                    Major = true,
                };

                this._Cycles = value;
                this.LayerChanged?.Invoke(_e);
            }
        }

        public SKBitmap[] Generate(Dictionary<string, PixelColors> colors)
        {
            return this.GenShape().GenColor(colors);
        }

        public SKBitmap GenerateSheet(Dictionary<string, PixelColors> colors)
        {
            var _frames = this.Generate(colors);
            (int W, int H) _s = (_frames[0].Width, _frames[0].Height);
            var _output = new SKBitmap(_s.W * _frames.Length, _s.H);

            using(var canvas = new SKCanvas(_output))
            {
                canvas.Clear(SKColor.Empty);
                for(int i = 0; i < _frames.Length; i++)
                {
                    canvas.DrawBitmap(_frames[i], new SKPoint(_s.W * i, 0));
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

        public SKBitmap[] GenColor(Dictionary<string, PixelColors> colors)
        {
            var _output = new SKBitmap[this.FramesCount];

            var _colors = new CellData() {
                MirrorX = this.MirrorX,
                MirrorY = this.MirrorY,
                Pixels = colors,
            };

            for(int i = 0; i < this.FramesCount; i++)
            {
                _output[i] = this.Frames[i].Generate(_colors).ToSkia();
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