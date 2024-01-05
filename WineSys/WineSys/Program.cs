using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DVI_Access_Lib;
using Konsole;

// BETA 05.01.24

namespace DVI_Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            // Remember to add time and date instead of only timezone!!!

            TimeZoneInfo dktime = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
            TimeZoneInfo calitime = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            TimeZoneInfo austime = TimeZoneInfo.FindSystemTimeZoneById("W. Australia Standard Time");




            // Window.OpenBox(string title, int sx, int sy, int width, int height, BoxStyle style)            

            var winListWindow = Window.OpenBox("Lagerbeholdning for høj", 5, 2, 50,16, new BoxStyle()
            {
                ThickNess = LineThickNess.Double,
                Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
            });

            //Resize - Height; placement
            
            var winListWindow2 = Window.OpenBox("Lagerbeholdning lav", 5, 17, 50, 15, new BoxStyle()
            {
                ThickNess = LineThickNess.Double,
                Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
            });

            var Vin = Window.OpenBox("Vin", 55, 2, 100, 30, new BoxStyle()
            {
                ThickNess = LineThickNess.Double,
                Title = new Colors(ConsoleColor.White, ConsoleColor.Red)

            });

            // Split this box

            var winCurrTempCurrHum = Window.OpenBox("Temperatur og Luftfugtighed",5,32,50,6, new BoxStyle()
            {
                ThickNess = LineThickNess.Double,
                Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
        });

            var Box3 = Window.OpenBox("Box3", 55, 32, 50, 6, new BoxStyle()
            {
                ThickNess = LineThickNess.Double,
                Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
            });

            var Box4 = Window.OpenBox("Box4", 105, 32, 50, 6, new BoxStyle()
            {
                ThickNess = LineThickNess.Double,
                Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
            });


            DVI_Climate climate = new DVI_Climate("http://docker.data.techcollege.dk:5051");
            DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

            while (true)
            {

                Vin.Clear();
                foreach (Wine wine in stock.StockOverThreshold())
                    Vin.WriteLine(wine.WineName + " " + "[" + wine.NumInStock + "]");

                winListWindow.Clear();
                foreach (Wine wine in stock.StockOverThreshold())
                winListWindow.WriteLine(wine.WineName + " " + "[" +wine.NumInStock + "]");

                
                winListWindow2.Clear();
                foreach (Wine wine in stock.StockUnderThreshold())
                    winListWindow2.WriteLine(wine.WineName + "[" + wine.NumInStock + "]");


                Box3.Clear();
                Box3.WriteLine(dktime + "\n" + calitime + "\n" + austime);

                //Split
                
                winCurrTempCurrHum.Clear(); 
                winCurrTempCurrHum.WriteLine(climate.CurrTemp().ToString() + "°" + "                                " + climate.CurrHum().ToString() + "%");

                //Equal or greater then on warnings.
                
                if (climate.CurrTemp() > climate.MaxTemp())
                {
                    winCurrTempCurrHum.WriteLine("HIGH TEMP!");
                    Console.Beep();
                }
                
                if (climate.CurrTemp() <= climate.MinTemp())
                {   
                    winCurrTempCurrHum.WriteLine("LOW TEMP!");
                    Console.Beep();
                }

                if (climate.CurrTemp() >= climate.MaxTemp())
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

                if (climate.CurrTemp() < climate.MaxTemp() && climate.CurrTemp() > climate.MinTemp())
                {
                    winCurrTempCurrHum.WriteLine("WORKING");
                }

                if (climate.CurrHum() < climate.MaxHum() && climate.CurrHum() > climate.MinHum())
                {
                    winCurrTempCurrHum.WriteLine("                                     WORKING");
                }

                Thread.Sleep(10000);

            }
        }
    }
}