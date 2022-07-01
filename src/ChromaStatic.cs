using System;
using SkiaSharp;

namespace PichaLib
{
    public partial class Chroma
    {
        // CONSTRUCTORS FOR CHROMA OBJECT
        public static Chroma CreateFromHSV(float h, float s, float v, float a)
            { return new Chroma(Chroma.HSVtoRGB(h, s, v, a)); }
        public static Chroma CreateFromHSL(float h, float s, float l, float a)
            { return new Chroma(Chroma.HSLtoRGB(h, s, l, a)); }
        public static Chroma CreateFromHex(string hex)
            { return Chroma.CreateFromSkia(SKColor.Parse(hex)); }
        public static Chroma CreateFromSkia(SKColor col)
        { 
            return new Chroma(
                (float)(col.Red / 255f),
                (float)(col.Green / 255f),
                (float)(col.Blue / 255f),
                (float)(col.Alpha / 255f)
            );
        }
        public static Chroma CreateFromBytes(byte r, byte g, byte b, byte a)
            { return Chroma.CreateFromBytes((r, g, b, a)); }
        public static Chroma CreateFromBytes((byte r, byte g, byte b, byte a) col)
        {
            return new Chroma(
                (float)(col.r / 255f),
                (float)(col.g / 255f),
                (float)(col.b / 255f),
                (float)(col.a / 255f)
            );
        }

        // CONVERSION METHODS
        // convenience methods for tuple handling
        public static (float h, float s, float v, float a) RGBtoHSV((float r, float g, float b, float a) col)
            { return Chroma.RGBtoHSV(col.r, col.g, col.b, col.a); }
        public static float RGBtoH((float r, float g, float b) col)
            { return Chroma.RGBtoH(col.r, col.g, col.b); }
        public static (float r, float g, float b, float a) HSVtoRGB((float h, float s, float v, float a) col)
            { return Chroma.HSVtoRGB(col.h, col.s, col.v, col.a); }
        public static (float h, float s, float l, float a) HSVtoHSL((float h, float s, float v, float a) col)
            { return Chroma.HSVtoHSL(col.h, col.s, col.v, col.a); }
        public static (float h, float s, float v, float a) HSLtoHSV((float h, float s, float l, float a) col)
            { return Chroma.HSLtoHSV(col.h, col.s, col.l, col.a); }
        public static (float h, float s, float l, float a) RGBtoHSL((float r, float g, float b, float a) col)
            { return Chroma.RGBtoHSL(col.r, col.g, col.b, col.a); }

        // multistage conversions for convenience
        public static (float h, float s, float l, float a) RGBtoHSL(float r, float g, float b, float a)
            { return Chroma.HSVtoHSL(Chroma.RGBtoHSV(r, g, b, a)); }
        public static (float r, float g, float b, float a) HSLtoRGB(float h, float s, float l, float a)
            { return Chroma.HSVtoRGB(Chroma.HSLtoHSV(h, s, l, a)); }

        // actual conversion methods
        public static float RGBtoH(float r, float g, float b)
        {
            float _out = 0f;
            float _max = Math.Max(r, Math.Max(g, b));
            float _min = Math.Min(r, Math.Min(g, b));
            float _delta = _max - _min;

            if(_delta == 0)
                { return 0f; }

            if(_max == r)
                { _out = (g - b) / (_delta); }
            else if(_max == g)
                { _out = 2f + (b - r) / (_delta); }
            else
                { _out = 4f + (r - g) / (_max / _min); } 

            _out *= 60;

            if(_out < 0) { _out += 360; }

            return (_out / 360);
        }

        public static (float h, float s, float v, float a) RGBtoHSV(float r, float g, float b, float a)
        {
            float _max = Math.Max(r, Math.Max(g, b));
            float _min = Math.Min(r, Math.Min(g, b));
            float _delta = _max - _min;

            float _h = Chroma.RGBtoH(r, g, b);
            float _s = 0f;
            float _v = _max;

            if(_max != 0) { _s = _delta / _max; }

            return (_h, _s, _v, a);
        }

        public static (float r, float g, float b, float a) HSVtoRGB(float h, float s, float v, float a)
        {
            float _h = h * 360;
            float _s = s;
            float _v = v;

            if(_s == 0)
                { return (v, v, v, a); }
            else if(_v <= 0)
                { return (0f, 0f, 0f, a); }

            // wrap the value around 360°
            while (_h < 0) 
                { _h += 360; };
            while (_h >= 360) 
                { _h -= 360; };

            // calculate possible values
            float _hf = _h / 60.0f;
            int _i = (int)Math.Floor(_hf);
            float _f = _hf - _i;
            float _pv = _v * (1 - _s);
            float _qv = _v * (1 - _s * _f);
            float _tv = _v * (1 - _s * (1 - _f));

            switch (_i) 
            {
                case 0:
                    return (_v, _tv, _pv, a);
                case 1:
                    return (_qv, _v, _pv, a);
                case 2:
                    return (_pv, _v, _tv, a);
                case 3:
                    return (_pv, _qv, _v, a);
                case 4:
                    return (_tv, _pv, _v, a);
                case 5:
                    return (_v, _pv, _qv, a);
                case 6:
                    return (_v, _tv, _pv, a);
                case -1:
                    return (_v, _tv, _qv, a);
                default:
                    // TODO throw error because color doesn't exist
                    // for now convert to black and white
                    return (_v, _v, _v, a);
            }
        }

        public static (float h, float s, float l, float a) HSVtoHSL(float h, float s, float v, float a)
        {
            float _h = h;
            float _l = (v * (1-(s / 2)));
            float _s = (_l == 0 | _l == 1) ? 0f : (v - _l) / Math.Min(_l, 1 - _l);   
            return (_h, _s, _l, a);
        }
        public static (float h, float s, float v, float a) HSLtoHSV(float h, float s, float l, float a)
        {
            float _h = h;
            float _v = l + (s * Math.Min(l, 1 - l));
            float _s = _v == 0f ? 0f : 2 * (1 - (l / _v));
            return (_h, _s, _v, a);
        }
    }
}