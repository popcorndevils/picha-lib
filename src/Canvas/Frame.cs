using System;
using System.Collections.Generic;

using OctavianLib;

namespace PichaLib
{
    public class Frame
    {
        public string[,] Data;
        public int Timing = 1;

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
            var _output = new Frame() { Timing = this.Timing };
            _output.Data = this.Data.Copy();

            foreach(Cycle _cycle in cycles)
            {
                _output = _output.RunPolicies(_cycle.Policies);
            }

            return _output;
        }
    }
}