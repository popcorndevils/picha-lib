using System;
using System.Collections.Generic;
using SkiaSharp;
using OctavianLib;

namespace PichaLib
{
    public class Frame
    {
        public string[,] Data;

        public int GetWidth() { return this.Data.GetWidth(); }
        public int GetHeight() { return this.Data.GetHeight(); }
        
        public string RunPolicy(Policy p, int x, int y)
        {
            if(PFactory.Random.NextDouble() <= p.Rate)
            {
                if(p.ConditionA != ConditionTarget.NONE && p.ConditionLogic != ConditionExpression.NONE)
                {
                    switch(p.ConditionA)
                    {
                        case ConditionTarget.NEIGHBOR:
                            if(p.ConditionLogic == ConditionExpression.IS &&
                               this.Data.NeighborIs(x, y, p.ConditionB))
                                { return p.Output; }
                            else if(p.ConditionLogic == ConditionExpression.IS_NOT &&
                                    this.Data.NeighborIsNot(x, y, p.ConditionB))
                                { return p.Output; }
                            else
                                { return this.Data[y, x]; }
                        default:
                            return this.Data[y, x];
                    }
                }
                else 
                    { return p.Output; }
            }
            else
                { return this.Data[y, x]; }
        }

        public Frame RunPolicies(List<Policy> cycle)
        {
            (int W, int H) _size = (this.GetWidth(), this.GetHeight());

            var _output = this.Copy();
            _output.Data = new string[_size.H, _size.W];

            for(int _x = 0; _x < _size.W; _x++)
            {
                for(int _y = 0; _y < _size.H; _y++)
                {
                    string _cType = this.Data[_y, _x];
                    var _policies = cycle.FindAll(p => p.Input == _cType);
                    if(_policies.Count > 0)
                    { 
                        foreach(Policy _p in _policies)
                        {
                            _output.Data[_y, _x] = this.RunPolicy(_p, _x, _y);
                        }
                    }
                    else
                        { _output.Data[_y, _x] = _cType; }
                }
            }
            
            return _output;
        }

        public Frame RunCycles(List<Cycle> cycles)
        {
            var _output = new Frame();
            _output.Data = this.Data.Copy();

            foreach(Cycle _cycle in cycles)
            {
                _output = _output.RunPolicies(_cycle.Policies);
            }

            return _output;
        }

        public SKBitmap Generate(CellData cd)
        {
            int _w = this.GetWidth();
            int _h = this.GetHeight();
            var _color = new SKColor[_h, _w];

            for(int y = 0; y < _h; y++)
            {
                for(int x = 0; x < _w; x++)
                {
                    var _cell = this.Data[y, x];
                    var _cSet = cd.Pixels[_cell];
                    if(_cell != Pixel.NULL)
                    {
                        float _grade = 0f;

                        switch(cd.Pixels[_cell].FadeDirection)
                        {
                            case FadeDirection.NORTH:
                                _grade = (float)((y + 1f) / _h);
                                break;
                            case FadeDirection.WEST:
                                _grade = (float)((x + 1f) / _w);
                                break;
                            case FadeDirection.SOUTH:
                                _grade = 1f - (float)((y + 1f) / _h);
                                break;
                            case FadeDirection.EAST:
                                _grade = 1f - (float)((x + 1f) / _w);
                                break;
                            case FadeDirection.NONE:
                                _grade = 1f;
                                break;
                        }

                        float u_sin = (float)Math.Cos(_grade * Math.PI);
                        float _l = (float)(PFactory.Random.RandfRange(0f, cd.Pixels[_cell].BrightNoise) * u_sin) + _cSet.HSL.l;

                        _color[y, x] = Chroma.CreateFromHSL(_cSet.HSL.h, _cSet.Sat, _l, _cSet.HSL.a).ToSkia();
                    }
                    else
                    {
                        // is the cell is null just fill with transparent pixel.
                        _color[y, x] = Chroma.CreateFromBytes(0, 0, 0, 0).ToSkia();
                    }
                }
            }

            if(cd.MirrorX) { _color = _color.MirrorX(); }
            if(cd.MirrorY) { _color = _color.MirrorY(); }

            var _output = new SKBitmap(_color.GetWidth(), _color.GetHeight());

            for(int x = 0; x < _color.GetWidth(); x++)
            {
                for(int y = 0; y < _color.GetHeight(); y++)
                {
                    _output.SetPixel(x, y, _color[y, x]);
                }
            }

            return _output;
        }
    }
}