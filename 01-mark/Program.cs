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
            //TODO. Странное умолчание, что для входного файла путь относительно корня солюшна, для аутпута - от текущей 
            var inputFileName =  "../../../";
            var outputFileName = "output.html";
            if (args.Length == 0)
            {
                Console.Write("Enter input file name: ");
                inputFileName += Console.ReadLine();
            }
            else
                inputFileName += args[0];
            if (args.Length >= 2)
                outputFileName = args[1];
            var data = ReadDataFromFile(inputFileName);
            var htmlData = ParserToHTML.Parse(data);
            WriteDataFromFile(htmlData, outputFileName);
            Console.WriteLine("complete!");
        }

        //TODO. Можно проще. File.ReadAllText() или File.ReadAllLines()
        static string ReadDataFromFile(string fileName)
        {
            var data = "";
            using (var reader = new StreamReader(fileName))
            {
                data = reader.ReadToEnd();
            }
            return data;
        }

        //TODO. тоже можно проще. см. System.IO.File.
        static void WriteDataFromFile(string[] data, string fileName)
        {
            using (var writer = new StreamWriter(fileName))
            {
                //TODO. Если хотим генерировать валидный HTML - можно заодно добавить doctype, и тэг html.
                writer.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                foreach (var line in data)
                    writer.Write(line);
            }
        }
    }
}
