using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_mark
{
    public class ParserOutputData
    {
        public readonly string[] parsedData;
        public readonly List<Tuple<int, int>> parsedParts;
        public ParserOutputData(string[] parsedData, List<Tuple<int, int>> parsedParts)
        {
            this.parsedData = parsedData;
            this.parsedParts = parsedParts;
        }
    }
}
