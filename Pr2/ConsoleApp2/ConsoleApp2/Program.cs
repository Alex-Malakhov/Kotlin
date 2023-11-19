using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CityDirectoryApp
{
    class Program
    {
        static void Main(string[] args)
        {

            string filepath;
            while (true)
            {
                Console.WriteLine("Введите путь до файла-справочника или введите 'exit' для завершения работы:");
                filepath = Console.ReadLine();

                if (filepath.ToLower() == "exit")
                {
                    break;
                }
                ProcessF processF = new ProcessF();
                processF.ProcessFile(filepath);
            }

            Console.WriteLine("Работа приложения завершена.");

        }
    }
}
    
    
