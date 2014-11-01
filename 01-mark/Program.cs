using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace _01_mark
{
    class Program
    {
        static void Main(string[] args)
        {
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

        }
        static string ReadDataFromFile(string fileName)
        {
            throw new NotImplementedException();
        }
        static void WriteDataFromFile(string[] data, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
