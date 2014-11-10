using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace _01_mark
{
    public class MarkdownProcessor
    {
        public static string[] Parse(string text)
        {
            var replacedSpecial = ParseSpecialSymbols(text);
            var lines = ParseLines(replacedSpecial);
            ParseBackticks(lines);
            ParseUnderlines(lines, "__", "strong");
            ParseUnderlines(lines, "_", "em");
            RemoveEscapeChars(lines);
            return lines;
        }

        public static string ParseSpecialSymbols(string text)
        {
            return HttpUtility.HtmlEncode(text);
        }

        public static string[] ParseLines(string text)
        {
            var lines = text.Split('\n');
            lines[0] = "<p>" + lines[0];
            lines[lines.Length - 1] += "</p>";
            return lines
                .Select(x =>
                {
                    if (String.IsNullOrWhiteSpace(x))
                        return x + "</p><p>";
                    else
                        return x;
                })
                .ToArray();
        }

        public static ParserOutputData ParseOnParts(string line, string symbols)
        {
            var splited = Regex.Split(line, symbols);
            var nowCoded = false;
            var codedFrom = 0;
            var codedParts = new List<ParsedRange>(); // элементы массива, которые будут подвергнуты обработке и тегам. включая оба.
            for (var i = 1; i < splited.Length; i++) // генерим все части, которые нужно закодить.
            {
                if (splited[i - 1].Length > 0 && splited[i - 1].Last() == '\\')
                {
                    splited[i - 1] += symbols;
                    continue;
                }


                var isChanged = IsChangedState(
                    nowCoded, 
                    nowCoded ? splited[i] : splited[i - 1], 
                    i == 1 || i == splited.Length - 1);
                if (isChanged)
                {
                    if (nowCoded)
                        codedParts.Add(new ParsedRange(codedFrom, i - 1));
                    else
                        codedFrom = i;
                    nowCoded = !nowCoded;
                }
                else
                    splited[i - 1] += symbols;
            }
            if (nowCoded)
                splited[codedFrom - 1] += symbols;
            return new ParserOutputData(splited, codedParts);
        }

        private static bool IsChangedState(bool nowCoded, string line, bool startOrEnd) //получилось супер-кратко, но, возможно, немного запутанно.
        {
            if (line.Length == 0)
                return startOrEnd;
            return nowCoded ? IsGoodChar(line.First()) : IsGoodChar(line.Last());
        }
        private static bool IsGoodChar(char symbol)
        {
            return !Char.IsLetterOrDigit(symbol) && symbol != '_';
        }
        public static string AddTags(ParserOutputData data, string line, string tag)
        {
            var parsed = data.parsedData;
            var startTag = "<" + tag + ">";
            var endTag = "</" + tag + ">";
            foreach (var selectedPart in data.parsedParts) // парсер все сделал за нас. просто добавим тэги.
            {
                parsed[selectedPart.start] = startTag + parsed[selectedPart.start];
                parsed[selectedPart.end] += endTag;
            }

            return String.Join("", parsed);
        }
        public static void ParseBackticks(string[] lines)
        {
            for (var lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var data = ParseOnParts(lines[lineNum], "`");
                var parsedLine = ReplaceAllUnderlines(lines[lineNum], data);
                lines[lineNum] = AddTags(data, parsedLine, "code");
            }
        }
        public static string ReplaceAllUnderlines(string line, ParserOutputData data)
        {
            var parsed = data.parsedData;
            foreach (var codedPart in data.parsedParts)
                for (var i = codedPart.start; i <= codedPart.end; i++)
                {
                    var newPart = "";
                    foreach (var symbol in parsed[i])
                        if (symbol == '_')
                            newPart += "\\_";
                        else
                            newPart += symbol;
                    parsed[i] = newPart;
                }
            return String.Join("", parsed);
        }
        public static void ParseUnderlines(string[] lines, string symbols, string code)
        {
            for (var lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var data = ParseOnParts(lines[lineNum], symbols);
                var parsedLine = ReplaceAllUnderlines(lines[lineNum], data);
                lines[lineNum] = AddTags(data, parsedLine, code);
            }
        }

        public static void RemoveEscapeChars(string[] lines)
        {
            for (var lineNum = 0; lineNum < lines.Length; lineNum++)
            {
                var oldLine = lines[lineNum];
                var newLine = "";
                for (var i = 0; i < oldLine.Length; i++)
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
                lines[lineNum] = newLine;
            }
        }
    }
}
