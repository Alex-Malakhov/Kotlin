using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace CityDirectoryApp
{
    class City
    {
        public string Name { get; set; }
        public int Floors { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
    }
}
