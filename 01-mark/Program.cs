using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;


namespace _01_mark
{
    class Program
    {
        static void Main(string[] args)
        {
            var defaultInputFileName =  "../../../";
            var inputFileName = "";
            if (args.Length == 0)
            {
                Console.Write("Enter input file name: ");
                inputFileName = Console.ReadLine();
            }
            else
                inputFileName = args[0];
            if (!File.Exists(inputFileName))
            {
                inputFileName = defaultInputFileName + inputFileName;
                if (!File.Exists(inputFileName))
                    throw new FileNotFoundException("Файл не найден.");
            }
            var htmlCodeString = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">";
            var data = File.ReadAllText(inputFileName);
            var listOfLines = MarkdownProcessor.Parse(data).ToList<string>();
            listOfLines.Insert(0, htmlCodeString);
            File.WriteAllLines("output.html", listOfLines.ToArray());
        }
    }
}
