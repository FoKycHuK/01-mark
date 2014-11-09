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
            var inputFileName =  "../../../";
            var outputFileName = "../../../output.html";
            if (args.Length == 0)
            {
                Console.Write("Enter input file name: ");
                inputFileName += Console.ReadLine();
            }
            else
                inputFileName += args[0];
            if (args.Length >= 2)
                outputFileName = args[1];
            var htmlCodeString = "<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">";
            var data = File.ReadAllText(inputFileName);
            var listOfLines = MarkdownProcessor.Parse(data).ToList<string>();
            listOfLines.Insert(0, htmlCodeString);
            File.WriteAllLines(outputFileName, listOfLines.ToArray());
            Console.WriteLine("complete!");
        }
    }
}
