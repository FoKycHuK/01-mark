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
        //TODO. проблема Tuple - невозможнро понять, что в нем хранится, глядя только на него, без контекста. Класс с двумя именоваными полями гораздо информативнее
        public readonly List<Tuple<int, int>> parsedParts;
        public ParserOutputData(string[] parsedData, List<Tuple<int, int>> parsedParts)
        {
            this.parsedData = parsedData;
            this.parsedParts = parsedParts;
        }
    }
}
