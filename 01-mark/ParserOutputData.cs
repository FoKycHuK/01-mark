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
        public readonly List<ParsedRange> parsedParts;
        public ParserOutputData(string[] parsedData, List<ParsedRange> parsedParts)
        {
            this.parsedData = parsedData;
            this.parsedParts = parsedParts;
        }
    }
}
