using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
            Console.SetWindowSize(150,50);

            // int testValLow = 0;
            // int testValHigh = 1000;

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            var stockX = Window.OpenBox("stockX", 30, 50, 75, 50, new BoxStyle());

            var winListWindow = Window.OpenBox("Vine på lager", 5, 2, 50, 30, new BoxStyle()
            {
                ThickNess = LineThickNess.Double,
                Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
            });

            var winCurrTempCurrHum = Window.OpenBox("Temperatur og Luftfugtighed",5,32,50,5, new BoxStyle()
            {
                ThickNess = LineThickNess.Double,
                Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
            });


            DVI_Climate climate = new DVI_Climate("http://docker.data.techcollege.dk:5051");
            DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

            foreach (Wine wine in stock.WinesOnStock())
                stockX.WriteLine(wine.WineName + " " + wine.BaseGrape);

            while (true)
            {

            winListWindow.Clear();

                foreach (Wine wine in stock.WinesOnStock())
                    stockX.WriteLine(wine.WineName + "\n" + wine.BaseGrape + "\n");

                foreach (Wine wine in stock.WinesOnStock())
                winListWindow.WriteLine(wine.WineName + " " + wine.NumInStock);
            
                winCurrTempCurrHum.Clear(); 
                winCurrTempCurrHum.WriteLine(climate.CurrTemp().ToString() + "°" + "                                " + climate.CurrHum().ToString() + "%");

                if (climate.CurrTemp() > climate.MaxTemp())
                {
                    winCurrTempCurrHum.WriteLine("HIGH TEMP!");
                    Console.Beep();
                }
                
                    

                if (climate.CurrTemp() < climate.MinTemp())
                {   
                    winCurrTempCurrHum.WriteLine("LOW TEMP!");
                    Console.Beep();
                }

                if (climate.CurrTemp() > climate.MaxTemp())
                {
                    winCurrTempCurrHum.WriteLine("HIGH TEMP!");
                    Console.Beep();
                }

                if (climate.CurrHum() < climate.MinHum())
                {
                    winCurrTempCurrHum.WriteLine(                                      "LOW HUM!");
                    Console.Beep();
                }

                if (climate.CurrTemp() > climate.MaxHum())
                {
                    winCurrTempCurrHum.WriteLine("                                     HIGH HUM!");
                    Console.Beep();
                }


                Thread.Sleep(10000);

            }
        }
    }
}