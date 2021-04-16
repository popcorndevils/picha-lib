using System.Collections.Generic;

using OctavianLib;

namespace PichaLib
{
    public delegate void LayerEditedHandler(Layer layer);

    public class Layer
    {
        public event LayerEditedHandler OnLayerChanged;

        private string _Name;
        public string Name {
            get => this._Name;
            set {
                this._Name = value;
                this.OnLayerChanged?.Invoke(this);
            }
        }

        private float _AnimTime;
        public float AnimTime {
            get => this._AnimTime;
            set {
                this._AnimTime = value;
                this.OnLayerChanged?.Invoke(this);
            }
        }

        private bool _MirrorX, _MirrorY;
        public bool MirrorX {
            get => this._MirrorX;
            set {
                this._MirrorX = value;
                this.OnLayerChanged?.Invoke(this);
            }
        }
        public bool MirrorY {
            get => this._MirrorY;
            set {
                this._MirrorY = value;
                this.OnLayerChanged?.Invoke(this);
            }
        }

        private int _X, _Y;
        public int X {
            get => this._X;
            set {
                this._X = value;
                this.OnLayerChanged?.Invoke(this);
            }
        }
        public int Y {
            get => this._Y;
            set {
                this._Y = value;
                this.OnLayerChanged?.Invoke(this);
            }
        }

        public (int w, int h) Size => (this._Frames[0].GetWidth(), this._Frames[0].GetHeight());
        public (int w, int h) Position => (this.X, this.Y);

        private SortedList<int, string[,]> _Frames = new SortedList<int, string[,]>();
        public SortedList<int, string[,]> Frames {
            get => this._Frames;
            set {
                this._Frames = value;
                this.OnLayerChanged?.Invoke(this);
            }
        }

        private Dictionary<string, Pixel> _Pixels = new Dictionary<string, Pixel>();
        public Dictionary<string, Pixel> Pixels {
            get => this._Pixels;
            set => this._Pixels = value;
        }
        
        private SortedList<int, Cycle> _Cycles = new SortedList<int, Cycle>();
        public SortedList<int, Cycle> Cycles {
            get => this._Cycles;
            set => this._Cycles = value;
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