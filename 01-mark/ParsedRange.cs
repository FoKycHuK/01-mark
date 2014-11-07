using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_mark
{
    public class ParsedRange
    {
        public readonly int Item1;
        public readonly int Item2;
        public ParsedRange(int start, int end)
        {
            Item1 = start;
            Item2 = end;
        }
    }
}
