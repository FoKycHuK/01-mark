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

        //TODO. очень очень сложно. см комментарии в почте.
        public static ParserOutputData ParseOnParts(string line, string symbols)
        {
            var splited = Regex.Split(line, symbols);
            if (splited.Length < 3)
                return new ParserOutputData(new string[] { line }, new List<ParsedRange>());
            var nowCoded = false;
            int codedFrom = -1;
            var codedParts = new List<ParsedRange>(); // элементы массива, которые будут подвергнуты обработке и тегам. включая оба.
            for (var i = 1; i < splited.Length; i++) // генерим все части, которые нужно закодить.
            {
                if (splited[i - 1].Length > 0 && splited[i - 1].Last() == '\\')
                {
                    splited[i - 1] += symbols;
                    continue;
                }
                if (nowCoded)
                {
                    if (i == splited.Length - 1 &&
                        splited[i].Length == 0 ||
                        splited[i].Length > 0 &&
                        !Char.IsLetterOrDigit(splited[i].First()) &&
                        splited[i].First() != '_')
                        //!Regex.IsMatch(splited[i].First().ToString(), "^[a-zA-Zа-яА-Я0-9_]"))
                    {
                        codedParts.Add(new ParsedRange(codedFrom, i - 1));
                        nowCoded = false;
                    }
                    else
                        splited[i] = symbols + splited[i];
                }
                else
                {
                    if (i == 1 &&
                        splited[0].Length == 0 ||
                        splited[i - 1].Length > 0 &&
                        !Char.IsLetterOrDigit(splited[i - 1].Last()) &&
                        splited[i - 1].Last() != '_') // разве так лучше? ну ладно :)
                        //!Regex.IsMatch(splited[i - 1].Last().ToString(), "^[a-zA-Zа-яА-Я0-9_]"))
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
