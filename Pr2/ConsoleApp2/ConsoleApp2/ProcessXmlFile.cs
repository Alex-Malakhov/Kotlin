using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CityDirectoryApp
{

        class ProcessXmlFile
        {
            public void ProcessXml(string filepath)
            {
                XDocument xmlDocument = XDocument.Load(filepath);
                var cities = xmlDocument.Descendants("item")
                    .Select(x => new City
                    {
                        Name = x.Attribute("city").Value,
                        Floors = int.Parse(x.Attribute("floor").Value),
                        Street = x.Attribute("street").Value,
                        House = x.Attribute("house").Value
                    })
                    .ToList();

                DisplayDuplicates displayduplicates = new DisplayDuplicates();
                displayduplicates.Display(cities);
                DisplayFloors displayfloors = new DisplayFloors();
                displayfloors.DisplayFloorsStatistics(cities);
            }
        }
    }

