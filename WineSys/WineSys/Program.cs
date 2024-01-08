using System;
using CodeHollow.FeedReader;
using DVI_Access_Lib;
using Konsole;
using System.Runtime.InteropServices;       // hentet fra nettet for at maximize et console vindue

public class WineSysMain
{
    #region maxmimize kode fra nettet
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    private const int MAXIMIZE = 3;
    private const int MINIMIZE = 6;
    #endregion

    // opretter en string variable hvor der er indsat noget ascii art
    private static string asciiart = @"
        oooooo   oooooo     oooo  o8o                         .oooooo..o                      
         `888.    `888.     .8'   `""'                        d8P'    `Y8                      
          `888.   .8888.   .8'   oooo  ooo. .oo.    .ooooo.  Y88bo.      oooo    ooo  .oooo.o 
           `888  .8'`888. .8'    `888  `888P""Y88b  d88' `88b  `""Y8888o.   `88.  .8'  d88(  ""8 
            `888.8'  `888.8'      888   888   888  888ooo888      `""Y88b   `88..8'   `""Y88b.  
             `888'    `888'       888   888   888  888    .o oo     .d8P    `888'    o.  )88b 
              `8'      `8'       o888o o888o o888o `Y8bod8P' 8""""88888P'      .8'     8""""888P' 
                                                                         .o..P'               
                                                                         `Y8P'   GRUPPE I ©          
                                                                       3MK4N1 K4K4N0 0EK5                                        ";

    // constructor, bruges når man laver en ny instance af en class, denne er tom
    // kan udelades da en tom constructor alligevel fyldes med default værdier for variable
    public WineSysMain()
    {
    }

    // "Main" er udgangspunktet for al C# kode
    static void Main(string[] args)
    {
        ShowWindow(GetConsoleWindow(), MAXIMIZE);

        // vi starter vores loop som er en task, den kører sideløbende med koden
        Task t = new Task(() => WineSysLoop());
        t.Start();

        // vi starter endnu en løkke for at forhindre at programmet lukker ned 
        while(true)
        {

        }
    }

    // her er hoveddelen af al koden
    private async void WineSysLoop()
    {
        // sætter vinduet til at være det største den kan være
        
        //Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

        // skriver vores intro
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(asciiart);
        
        // venter 3 sekunder og derefter så clear vi console så de er klar igen
        await Task.Delay(3000);
        Console.Clear();

        // set color of text and background (necessary?)
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        // setup timezones, used for automatically converting between "our" time and different zones
        TimeZoneInfo dkTime = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
        TimeZoneInfo caliTime = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        TimeZoneInfo ausTime = TimeZoneInfo.FindSystemTimeZoneById("W. Australia Standard Time");

        // get the current date/time
        DateTime utcNow = DateTime.UtcNow;

        // opret nye variable som konverterer den nuværende tid til de nye tidszoner
        // dette er en lidt større metode, man kunne bare have +/- de rigtige antal timer og få samme resultat
        DateTime dkTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, dkTime);
        DateTime caliTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, caliTime);
        DateTime ausTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, ausTime);

        // da vi bruger den samme style til alle bokse, så opretter vi en variable som indeholder 
        // den samme stil, her sætter vi thickness og og title colors
        BoxStyle ourStyle = new BoxStyle();
        ourStyle.ThickNess = LineThickNess.Double;
        ourStyle.Title = new Colors(ConsoleColor.White, ConsoleColor.Red);

        // definer de forskellige bokse fra Konsole, tallene er bredde og højde, 
        var boxLager = Window.OpenBox("Lagerbeholdning", 55, 2, 50, 30, ourStyle);
        var boxLagerHigh = Window.OpenBox("Lagerbeholdning for høj", 5, 2, 50, 17, ourStyle);
        var boxLagerLow = Window.OpenBox("Lagerbeholdning for lav", 5, 19, 50, 13, ourStyle);
        var boxAntal = Window.OpenBox("Antal", 105, 2, 15, 30, ourStyle);
        var boxIndkøbspris = Window.OpenBox("Indkøbspris", 120, 2, 15, 30, ourStyle);
        var boxAvance = Window.OpenBox("Avance", 135, 2, 20, 30, ourStyle);
        var boxTemperature = Window.OpenBox("Temperatur", 5, 32, 25, 6, ourStyle);
        var boxHumidity = Window.OpenBox("Luftfugtighed", 30, 32, 25, 6, ourStyle);
        var boxClock = Window.OpenBox("Klokken", 55, 32, 50, 6, ourStyle);
        var boxRSS = Window.OpenBox("Seneste nyt fra DR.dk", 105, 32, 50, 6, ourStyle);

        // her bruger vi den API fra den "using DVI_Access_Lib" til at hente de data ned fra cloud
        DVI_Climate climate = new DVI_Climate("http://docker.data.techcollege.dk:5051");
        DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

        // her begynder vi et uendeligt loop (den er aldrig "ikke true") for at programmer kører for "evigt"
        while (true)
        {
            // vi rydder indholdet i boxen og tilføjer indholdet
            boxLager.Clear();
            foreach (Wine wine in stock.WinesOnStock())
            {
                boxLager.WriteLine("\n" + wine.WineName);
            }

            // vi rydder indholdet i boxen og tilføjer indholdet
            boxAntal.Clear();
            foreach (Wine wine in stock.WinesOnStock())
            {
                boxAntal.WriteLine("\n" + wine.NumInStock.ToString());
            }

            // vi rydder indholdet i boxen og tilføjer indholdet
            boxIndkøbspris.Clear();
            foreach (Wine wine in stock.WinesOnStock())
            {
                boxIndkøbspris.WriteLine("\n" + wine.PurchasePrice.ToString("00.00") + " DKK");
            }                

            boxAvance.Clear();
            foreach (Wine wine in stock.WinesOnStock())
            {
                boxAvance.WriteLine("\n" + (wine.PurchasePrice * wine.MarkUp).ToString("0.00") + " DKK" + " | " + wine.MarkUp * 100 + "%");
            }                

            boxLagerHigh.Clear();
            foreach (Wine wine in stock.StockOverThreshold())
            {
                boxLagerHigh.WriteLine(wine.WineName);
            }                

            boxLagerLow.Clear();
            foreach (Wine wine in stock.StockUnderThreshold())
            {
                boxLagerLow.WriteLine(wine.WineName);
            }

            boxClock.Clear();
            boxClock.WriteLine("Danmark " + "    " + dkTimeNow + "\n" + "Californien " + caliTimeNow + "\n" + "Australien " + " " + ausTimeNow);

            boxTemperature.Clear();
            boxTemperature.WriteLine(climate.CurrTemp().ToString("0.0") + "°");

            boxHumidity.Clear();
            boxHumidity.WriteLine(climate.CurrHum().ToString("0.0") + "%");

            if (climate.CurrTemp() < climate.MinTemp())
            {
                boxTemperature.WriteLine(ConsoleColor.Red, "LAV TEMP!");
                DoBeeps();
            }
            else if (climate.CurrTemp() > climate.MaxTemp())
            {
                boxTemperature.WriteLine(ConsoleColor.Red, "HØJ TEMP!");
                DoBeeps();
            }
            else
            {
                boxTemperature.WriteLine(ConsoleColor.Green, "ALT I ORDEN!");
            }

            if (climate.CurrHum() < climate.MinHum())
            {
                boxHumidity.WriteLine(ConsoleColor.Red, "LAV HUM!");
                DoBeeps();
            }
            else if (climate.CurrTemp() > climate.MaxHum())
            {
                boxHumidity.WriteLine(ConsoleColor.Red, "HØJ HUM!");
                DoBeeps();
            }
            else
            {
                boxHumidity.WriteLine(ConsoleColor.Green, "ALT I ORDEN!");
            }

            var feed = await FeedReader.ReadAsync("https://www.dr.dk/nyheder/service/feeds/senestenyt");
            boxRSS.Clear();

            for(int i = 0; i < feed.Items.Count; i++)
            {
                await Task.Delay(250);
                boxRSS.WriteLine((i+1) +"/" + feed.Items.Count + " " + feed.Items[i].Title + "\n");
            }

            //await Task.Delay(20);
        }
    }

    private void WineSysLoop_EKN()
    {
        // sætter vinduet til at være det største den kan være

        //Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

        // skriver vores intro
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(asciiart);

        // venter 3 sekunder og derefter så clear vi console så de er klar igen
        Thread.Sleep(3000);
        Console.Clear();

        // set color of text and background (necessary?)
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        // setup timezones, used for automatically converting between "our" time and different zones
        TimeZoneInfo dkTime = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
        TimeZoneInfo caliTime = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        TimeZoneInfo ausTime = TimeZoneInfo.FindSystemTimeZoneById("W. Australia Standard Time");

        // get the current date/time
        DateTime utcNow = DateTime.UtcNow;

        // opret nye variable som konverterer den nuværende tid til de nye tidszoner
        // dette er en lidt større metode, man kunne bare have +/- de rigtige antal timer og få samme resultat
        DateTime dkTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, dkTime);
        DateTime caliTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, caliTime);
        DateTime ausTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, ausTime);

        // da vi bruger den samme style til alle bokse, så opretter vi en variable som indeholder 
        // den samme stil, her sætter vi thickness og og title colors
        BoxStyle ourStyle = new BoxStyle();
        ourStyle.ThickNess = LineThickNess.Double;
        ourStyle.Title = new Colors(ConsoleColor.White, ConsoleColor.Red);

        // definer de forskellige bokse fra Konsole, tallene er bredde og højde, 
        var boxLager = Window.OpenBox("Lagerbeholdning", 55, 2, 50, 30, ourStyle);
        var boxLagerHigh = Window.OpenBox("Lagerbeholdning for høj", 5, 2, 50, 17, ourStyle);
        var boxLagerLow = Window.OpenBox("Lagerbeholdning for lav", 5, 19, 50, 13, ourStyle);
        var boxAntal = Window.OpenBox("Antal", 105, 2, 15, 30, ourStyle);
        var boxIndkøbspris = Window.OpenBox("Indkøbspris", 120, 2, 15, 30, ourStyle);
        var boxAvance = Window.OpenBox("Avance", 135, 2, 20, 30, ourStyle);
        var boxTemperature = Window.OpenBox("Temperatur", 5, 32, 25, 6, ourStyle);
        var boxHumidity = Window.OpenBox("Luftfugtighed", 30, 32, 25, 6, ourStyle);
        var boxClock = Window.OpenBox("Klokken", 55, 32, 50, 6, ourStyle);
        var boxRSS = Window.OpenBox("Seneste nyt fra DR.dk", 105, 32, 50, 6, ourStyle);

        // her bruger vi den API fra den "using DVI_Access_Lib" til at hente de data ned fra cloud
        DVI_Climate climate = new DVI_Climate("http://docker.data.techcollege.dk:5051");
        DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

        // her begynder vi et uendeligt loop (den er aldrig "ikke true") for at programmer kører for "evigt"
        while (true)
        {
            // vi rydder indholdet i boxen og tilføjer indholdet
            boxLager.Clear();
            foreach (Wine wine in stock.WinesOnStock())
            {
                boxLager.WriteLine("\n" + wine.WineName);
            }

            // vi rydder indholdet i boxen og tilføjer indholdet
            boxAntal.Clear();
            foreach (Wine wine in stock.WinesOnStock())
            {
                boxAntal.WriteLine("\n" + wine.NumInStock.ToString());
            }

            // vi rydder indholdet i boxen og tilføjer indholdet
            boxIndkøbspris.Clear();
            foreach (Wine wine in stock.WinesOnStock())
            {
                boxIndkøbspris.WriteLine("\n" + wine.PurchasePrice.ToString("00.00") + " DKK");
            }

            boxAvance.Clear();
            foreach (Wine wine in stock.WinesOnStock())
            {
                boxAvance.WriteLine("\n" + (wine.PurchasePrice * wine.MarkUp).ToString("0.00") + " DKK" + " | " + wine.MarkUp * 100 + "%");
            }

            boxLagerHigh.Clear();
            foreach (Wine wine in stock.StockOverThreshold())
            {
                boxLagerHigh.WriteLine(wine.WineName);
            }

            boxLagerLow.Clear();
            foreach (Wine wine in stock.StockUnderThreshold())
            {
                boxLagerLow.WriteLine(wine.WineName);
            }

            boxClock.Clear();
            boxClock.WriteLine("Danmark " + "    " + dkTimeNow + "\n" + "Californien " + caliTimeNow + "\n" + "Australien " + " " + ausTimeNow);

            boxTemperature.Clear();
            boxTemperature.WriteLine(climate.CurrTemp().ToString("0.0") + "°");

            boxHumidity.Clear();
            boxHumidity.WriteLine(climate.CurrHum().ToString("0.0") + "%");

            if (climate.CurrTemp() < climate.MinTemp())
            {
                boxTemperature.WriteLine(ConsoleColor.Red, "LAV TEMP!");
                DoBeeps();
            }
            else if (climate.CurrTemp() > climate.MaxTemp())
            {
                boxTemperature.WriteLine(ConsoleColor.Red, "HØJ TEMP!");
                DoBeeps();
            }
            else
            {
                boxTemperature.WriteLine(ConsoleColor.Green, "ALT I ORDEN!");
            }

            if (climate.CurrHum() < climate.MinHum())
            {
                boxHumidity.WriteLine(ConsoleColor.Red, "LAV HUM!");
                DoBeeps();
            }
            else if (climate.CurrTemp() > climate.MaxHum())
            {
                boxHumidity.WriteLine(ConsoleColor.Red, "HØJ HUM!");
                DoBeeps();
            }
            else
            {
                boxHumidity.WriteLine(ConsoleColor.Green, "ALT I ORDEN!");
            }

            var feed = FeedReader.ReadAsync("https://www.dr.dk/nyheder/service/feeds/senestenyt");
            boxRSS.Clear();

            for (int i = 0; i < feed.Items.Count; i++)
            {
                Thread.Sleep(3000);
                boxRSS.WriteLine((i + 1) + "/" + feed.Items.Count + " " + feed.Items[i].Title + "\n");
            }

            //await Task.Delay(20);
        }
    }


    // dette stykke kode blev repeteret ofte og er derfor lavet til sin egen metode
    // metoden laver bare 10 beeps
    private static void DoBeeps()
    {
        for (int i = 0; i < 10; i++)
        {
            Console.Beep(300, 500);
        }
    }

    // dette er noget gammelt test kode, til at lave en metode for at gøre det simplere at fylde boksene med tekst
    // fordi den netop opfører sig meget ens
    // kan ignoreres
    private static void UpdateWineText(IConsole console, string text, DVI_Stock stock)
    {
        console.Clear();
        foreach (Wine wine in stock.WinesOnStock())
        {
            console.WriteLine("\n" + wine.NumInStock.ToString());
        }
    }
}