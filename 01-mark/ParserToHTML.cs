using System;
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
            ParseDoubleUnderlines(lines);
            ParseUnderlines(lines);
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
                    if (symbol != ' ' && symbol != (char)13) //??? wtf
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
                    if (i == splited.Length - 1 && splited[i].Length == 0 || splited[i].Length > 0 && !Regex.IsMatch(splited[i].First().ToString(), "^[a-zA-Zа-яА-Я0-9_]")) //splited[i].First() == ' ')
                    {
                        codedParts.Add(Tuple.Create(codedFrom, i - 1));
                        nowCoded = false;
                    }
                    else
                        splited[i] = symbols + splited[i];
                }
                else
                {
                    if (i == 1 && splited[0].Length == 0 || splited[i - 1].Length > 0 && !Regex.IsMatch(splited[i - 1].Last().ToString(), "^[a-zA-Zа-яА-Я0-9_]"))//splited[i - 1].Last() == ' ')
                    {
                        nowCoded = true;
                        codedFrom = i;
                    }
                    else
                        splited[i - 1] += symbols;
                }
            }
            if (nowCoded)
                splited[codedFrom - 1] += symbols;
            return new ParserOutputData(splited, codedParts);
        }

        public static string AddTags(ParserOutputData data, string line, string tag)
        {
            var parsed = data.parsedData;
            var startTag = "<" + tag + ">";
            var endTag = "</" + tag + ">";
            foreach (var selectedPart in data.parsedParts) // парсер все сделал за нас. просто добавим тэги.
            {
                parsed[selectedPart.Item1] = startTag + parsed[selectedPart.Item1];
                parsed[selectedPart.Item2] += endTag;
            }
            return String.Join("", parsed);
        }
        public static void ParseBackticks(string[] lines)
        {
            
            for (var lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var data = ParseOnParts(lines[lineNum], "`");
                var parsed = data.parsedData;
                foreach (var codedPart in data.parsedParts) // нейтрализуем все спецсимволы в распаршеном коде.
                    for (var i = codedPart.Item1; i <= codedPart.Item2; i++)
                    {
                        var newPart = "";
                        foreach (var symbol in parsed[i])
                            if (symbol == '_')
                                newPart += "\\_";
                            else
                                newPart += symbol;
                        parsed[i] = newPart;
                    }
                lines[lineNum] = AddTags(data, lines[lineNum], "code");
            }
        }
        public static void ParseUnderlines(string[] lines)
        {
            for (var lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var data = ParseOnParts(lines[lineNum], "_");
                lines[lineNum] = AddTags(data, lines[lineNum], "em");
            }
        }
        public static void ParseDoubleUnderlines(string[] lines)
        {
            for (var lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var data = ParseOnParts(lines[lineNum], "__");
                lines[lineNum] = AddTags(data, lines[lineNum], "strong");
            }
        }
        public static void RemoveEscapeChars(string[] lines)
        {
            for (var lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var oldLine = lines[lineNum];
                var newLine = "";
                for (var i = 0; i < oldLine.Length; i++)
                {
                    if (oldLine[i] == '\\')
                    {
                        if (i != oldLine.Length - 1 && oldLine[i + 1] == '\\')
                        {
                            newLine += '\\';
                            i++;
                        }
                    }
                    else
                        newLine += oldLine[i];
                }
                lines[lineNum] = newLine;
            }
        }
    }
}
