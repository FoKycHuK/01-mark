﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _01_mark
{
    public class ParserToHTML
    {
        public static string[] Parse(string text)
        {
            var replacedSpecial = ParseSpecialSymbols(text);
            var lines = ParseLines(replacedSpecial);
            ParseBackticks(lines);
            ParseUnderlines(lines);
            ParseDoubleUnderlines(lines);
            RemoveEscapeChars(lines);
            return lines;

        }
        public static string ParseSpecialSymbols(string text)
        {
            var res = "";
            foreach (var symbol in text)
            {
                switch (symbol)
                {
                    case '<':
                        res += "&lt;";
                        break;
                    case '>':
                        res += "&gt;";
                        break;
                    case '"':
                        res += "&quot;";
                        break;
                    default:
                        res += symbol;
                        break;
                }
            }
            return res;
        }
        public static string[] ParseLines(string text)
        {
            var lines = text.Split('\n');
            lines[0] = "<p>" + lines[0];
            lines[lines.Length - 1] += "</p>";
            for (var i = 1; i < lines.Length; i++)
            {
                var allSpaces = true;
                foreach (var symbol in lines[i])
                    if (symbol != ' ')
                        allSpaces = false;
                if (allSpaces)
                {
                    lines[i - 1] += "</p>";
                    lines[i] += "<p>";
                }
            }
            return lines;
        }

        public static ParserOutputData ParseOnParts(string line, string symbols)
        {
            var splited = Regex.Split(line, symbols);
            if (splited.Length < 3)
                return new ParserOutputData(new string[] { line }, new List<Tuple<int, int>>());
            var nowCoded = false;
            int codedFrom = -1;
            var codedParts = new List<Tuple<int, int>>(); // элементы массива, которые будут подвергнуты обработке и тегам. включая оба.
            for (var i = 1; i < splited.Length; i++) // генерим все части, которые нужно закодить.
            {
                if (splited[i - 1].Length > 0 && splited[i - 1].Last() == '\\')
                {
                    splited[i - 1] += symbols;
                    continue;
                }
                if (nowCoded)
                {
                    codedParts.Add(Tuple.Create(codedFrom, i - 1));
                    nowCoded = false;
                }
                else
                {
                    nowCoded = true;
                    codedFrom = i;
                }
            }
            return new ParserOutputData(splited, codedParts);
        }

        public static void ParseBackticks(string[] lines) //временный коммент. хочу тут добавлять к каждому спец символу экранизацию (/)
        {
            
            for (var lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var data = ParseOnParts(lines[lineNum], "\'");
                var parsed = data.parsedData;
                foreach (var codedPart in data.parsedParts) // нейтрализуем все спецсимволы в распаршеном коде.
                {
                    parsed[codedPart.Item1] = "<code>" + parsed[codedPart.Item1];
                    parsed[codedPart.Item2] += "</code>";
                    for (var i = codedPart.Item1; i <= codedPart.Item2; i++)
                    {
                        var newPart = "";
                        foreach (var symbol in parsed[i])
                        {
                            if (symbol == '_')
                                newPart += "\\_";
                            else
                                newPart += symbol;
                        }
                        parsed[i] = newPart;
                    }
                }

                lines[lineNum] = String.Join("", parsed);
            }
        }
        public static void ParseUnderlines(string[] lines) //временный коммент. хочу тут реализовывать неэкранированные подчеркивания.
        {
            throw new NotImplementedException();
        }
        public static void ParseDoubleUnderlines(string[] lines) //временный коммент. хочу тут реализовывать неэкранированные двойные подчеркивания.
        {
            throw new NotImplementedException();
        }
        public static void RemoveEscapeChars(string[] lines)
        {
            throw new NotImplementedException();
        }
    }
}
