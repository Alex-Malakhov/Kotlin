using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CityDirectoryApp
{

        class DisplayFloors
        {
            public void DisplayFloorsStatistics(List<City> cities)
            {
                var statistics = cities.GroupBy(x => x.Name)
                    .ToDictionary(g => g.Key, g => g.GroupBy(x => x.Floors)
                        .ToDictionary(g2 => g2.Key, g2 => g2.Count()));

                Console.WriteLine("\nСтатистика по этажности зданий:");

                foreach (var city in statistics)
                {
                    Console.WriteLine(city.Key);

                    for (int i = 1; i <= 5; i++)
                    {
                        int count = city.Value.ContainsKey(i) ? city.Value[i] : 0;
                        Console.WriteLine(i + " этажных зданий: " + count);
                    }

                    Console.WriteLine();
                }
            }
        }
    }

