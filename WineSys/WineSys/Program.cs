using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DVI_Access_Lib;
using Konsole;

namespace DVI_Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var winListWindow = Window.OpenBox("Vine på lager", 30, 30, 20, 20);
            var winCurrTempCurrHum = Window.OpenBox("CurrTemp", 50, 50, 10, 10);

            DVI_Climate climate = new DVI_Climate("http://docker.data.techcollege.dk:5051");
            DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");
  

            winListWindow.WriteLine("Vine på lager:");

            foreach (Wine wine in stock.WinesOnStock())
                winListWindow.WriteLine(wine.WineName + " " + wine.NumInStock);

            winCurrTempCurrHum.WriteLine(climate.CurrTemp().ToString() + "\n" + climate.CurrHum().ToString());
        }
    }
}