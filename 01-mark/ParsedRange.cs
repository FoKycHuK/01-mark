using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_mark
{
    public class ParsedRange
    {
        public readonly int start;
        public readonly int end;
        public ParsedRange(int start, int end)
        {
            this.start = start;
            this.end = end;
        }
    }
}
