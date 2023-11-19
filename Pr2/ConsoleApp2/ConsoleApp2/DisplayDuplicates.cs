using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CityDirectoryApp
{
        class DisplayDuplicates
        {
            public void Display(List<City> cities)
            {
                var duplicates = cities.GroupBy(x => new { x.Name, x.Street, x.House, x.Floors })
                    .Where(g => g.Count() > 1)
                    .ToDictionary(g => g.Key, g => g.Count());

                Console.WriteLine("\nДублирующиеся записи:");
                foreach (var duplicate in duplicates)
                {
                    Console.WriteLine("Город: " + duplicate.Key.Name + ", Улица: " + duplicate.Key.Street + ", Дом: " + duplicate.Key.House + ", Количество повторений: " + duplicate.Value);
                    var duplicateCities = cities.Where(c => c.Name == duplicate.Key.Name && c.Street == duplicate.Key.Street && c.House == duplicate.Key.House && c.Floors == duplicate.Key.Floors).ToList();
                    foreach (var city in duplicateCities)
                    {
                        Console.WriteLine(" Повторяющаяся запись: " + city.Name + ", Улица: " + city.Street + ", Дом: " + city.House + ", Этажность здания: " + city.Floors);
                    }
                }
            }
        }
    }

