using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_mark
{
    class ParserToHTML
    {
        public static string[] Parse(string text)
        {
            var replacedSpecial = ParseSpecialSymbols(text);
            var lines = ParseLines(replacedSpecial);
            lines = ParseBackticks(lines);
            lines = ParseUnderlines(lines);
            lines = ParseDoubleUnderlines(lines);
            lines = RemoveEscapeChars(lines);
            return lines;
        }
        public static string ParseSpecialSymbols(string text)
        {
            throw new NotImplementedException();
        }
        public static string[] ParseLines(string text)
        {
            throw new NotImplementedException();
        }
        public static string[] ParseBackticks(string[] lines) //временный коммент. хочу тут добавлять к каждому спец символу экранизацию (/)
        {
            throw new NotImplementedException();
        }
        public static string[] ParseUnderlines(string[] lines) //временный коммент. хочу тут реализовывать неэкранированные подчеркивания.
        {
            throw new NotImplementedException();
        }
        public static string[] ParseDoubleUnderlines(string[] lines) //временный коммент. хочу тут реализовывать неэкранированные двойные подчеркивания.
        {
            throw new NotImplementedException();
        }
        public static string[] RemoveEscapeChars(string[] lines)
        {
            throw new NotImplementedException();
        }
    }
}
