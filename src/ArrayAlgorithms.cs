using System.Collections.Generic;

namespace PichaLib
{
    public static class ArrayAlgorithms
    {
        // TODO move all this logic to an array algorithms class?
        public static bool NeighborIs(this string[,] array, int x, int y, string value) 
        {
            var _testVals = array.GatherNeighbors(x, y);
            foreach(string _v in _testVals) { if(_v == value) { return true; } }
            return false;
        }

        public static bool NeighborIsNot(this string[,] array, int x, int y, string value) 
        {
            var _testVals = array.GatherNeighbors(x, y);
            foreach(string _v in _testVals) { if(_v != value) { return true; } }
            return false;
        }

        public static List<string> GatherNeighbors(this string[,] array, int x, int y)
        {
            List<string> _output = new List<string>();
            List<(int xT, int yT)> _addresses = new List<(int xT, int yT)>() {
                (x - 1, y),
                (x + 1, y),
                (x, y - 1),
                (x, y + 1),
            };

            // TODO Fix this kind of wonky implementation?
            foreach((int xT, int yT) _a in _addresses)
            {
                try { _output.Add(array[_a.yT, _a.xT]); }
                catch {}
            }

            return _output;
        }
    }
}