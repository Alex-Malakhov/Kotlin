using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CityDirectoryApp
{
        class ProcessCsvFile
        {
            public void ProcessCsv(string filepath)
            {
                var cities = File.ReadLines(filepath)
                    .Skip(1)
                    .Select(x =>
                    {
                        var values = x.Split(';');
                        return new City
                        {
                            Name = values[0].Trim('\"'),
                            Floors = int.Parse(values[3]),
                            Street = values[1].Trim('\"'),
                            House = values[2].Trim('\"')
                        };
                    })
                    .ToList();

                DisplayDuplicates displayduplicates = new DisplayDuplicates();
                displayduplicates.Display(cities);
                DisplayFloors displayfloors = new DisplayFloors();
                displayfloors.DisplayFloorsStatistics(cities);
            }
        }
    }

