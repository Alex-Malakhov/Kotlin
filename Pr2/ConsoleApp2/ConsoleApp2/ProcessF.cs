using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CityDirectoryApp
{
    
        class ProcessF
        {
            public void ProcessFile(string filepath)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                if (Path.GetExtension(filepath) == ".xml")
                {
                    ProcessXmlFile processXmlFile = new ProcessXmlFile();
                    processXmlFile.ProcessXml(filepath);
                }
                else if (Path.GetExtension(filepath) == ".csv")
                {
                    ProcessCsvFile processCsvFile = new ProcessCsvFile();
                    processCsvFile.ProcessCsv(filepath);
                }
                else
                {
                    Console.WriteLine("Неподдерживаемый формат файла.");
                    return;
                }

                stopwatch.Stop();
                Console.WriteLine("Время обработки файла: " + stopwatch.Elapsed.TotalSeconds + " сек.");
            }
        }
    }
