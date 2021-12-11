using System.Collections.Generic;

namespace PichaLib
{
    // TODO move to types class?  Why force it to be internal?  Eventually move code here from godot that handles gen rules for frames.
    internal struct CellData
    {
        public bool MirrorX;
        public bool MirrorY;
        public Dictionary<string, PixelColors> Pixels;
    }

    internal struct PixelColors
    {
        public Chroma RGB;
        public (float h, float s, float l, float a) HSL => this.RGB.HSL;
        public float Sat;
        public FadeDirection FadeDirection;
        public float BrightNoise;
    }
}
