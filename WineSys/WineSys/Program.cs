using System;
using CodeHollow.FeedReader;
using DVI_Access_Lib;
using Konsole;
using System.Runtime.InteropServices;       // hentet fra nettet for at maximize et console vindue

#region maxmimize kode fra nettet
[DllImport("kernel32.dll", ExactSpelling = true)]
static extern IntPtr GetConsoleWindow();

[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
const int MAXIMIZE = 3;
#endregion

//NB, vi bruger ikke access modifiers (private, public, protected) da vi kører "som powershell", hedder "top level statements" er nyt i 2020

// opretter en string variable hvor der er indsat noget ascii art
string asciiart = @"
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

// vi gemmer de forskellige timers her for nem adgang
int showAsciiTime = 3000;
int rssWaitTime = 3500;

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

// her bruger vi den API fra den "using DVI_Access_Lib" til at hente de data ned fra cloud
DVI_Climate climate = new DVI_Climate("http://docker.data.techcollege.dk:5051");
DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

// start af program, åben console vindue og maximize det (maximize er taget fra nettet)
ShowWindow(GetConsoleWindow(), MAXIMIZE);

// skriver vores intro, sætter farven til grøn for epic hackerman look
Console.ForegroundColor = ConsoleColor.Green;

Console.WriteLine(asciiart);

// venter "showAsciiTime" millisekunder og derefter så clear vi console så de er klar igen
await Task.Delay(showAsciiTime);
Console.Clear();

// vi sætter farverne på console tilbage til alm sort/hvid
Console.ForegroundColor = ConsoleColor.White;
Console.BackgroundColor = ConsoleColor.Black;

// definer de forskellige bokse fra Konsole, tallene er bredde og højde
// modsat andre variable, defineres de her fordi de gør brug af størrelsen af console window
// hvis de blev sat op først er vinduet for lille og den crasher
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

// her begynder vi et uendeligt loop (den er aldrig "ikke true") for at programmer kører for "evigt"
while (true)
{
    // vi rydder indholdet i boxen og tilføjer indholdet, gøres flere steder herunder
    boxLager.Clear();
    foreach (Wine wine in stock.WinesOnStock())
    {
        boxLager.WriteLine("\n" + wine.WineName);
    }

    boxAntal.Clear();
    foreach (Wine wine in stock.WinesOnStock())
    {
        boxAntal.WriteLine("\n" + wine.NumInStock.ToString());
    }

    boxIndkøbspris.Clear();
    foreach (Wine wine in stock.WinesOnStock())
    {
        // bemærk at alle steder vi bruger "ToString("00.00")" er for at formatere tallet til et bestemt antal decimaler
        // "0" betyder der _skal_ være et ciffer, mens at "#" kan bruges til at sige der kan være et
        boxIndkøbspris.WriteLine("\n" + wine.PurchasePrice.ToString("00.00") + " DKK");
    }

    boxAvance.Clear();
    foreach (Wine wine in stock.WinesOnStock())
    {
        boxAvance.WriteLine("\n" + (wine.PurchasePrice * wine.MarkUp).ToString("0.00") + " DKK" + " | " + (wine.MarkUp * 100) + "%");
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

    // vi bruger "ToString("g") for at formatere det korrekt, https://learn.microsoft.com/en-us/dotnet/api/system.datetime.tostring?view=net-8.0
    boxClock.Clear();
    boxClock.WriteLine("Danmark " + "    " + dkTimeNow.ToString("g") + "\n" + "Californien " + caliTimeNow.ToString("g") + "\n" + "Australien " + " " + ausTimeNow.ToString("g"));

    boxTemperature.Clear();
    boxTemperature.WriteLine(climate.CurrTemp().ToString("0.0") + "°");

    boxHumidity.Clear();
    boxHumidity.WriteLine(climate.CurrHum().ToString("0.0") + "%");

    // nedenfor er 2 større conditional statements der tjekker temperatur, conditionals >, <, <=, >= 
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

    // vi henter et RSS feed med vores library "CodeHollow.FeedReader"
    var feed = await FeedReader.ReadAsync("https://www.dr.dk/nyheder/service/feeds/senestenyt");
    boxRSS.Clear();

    // for loop der kører igennem alle nyhederne i feed
    // for i stedet for "for each" fordi vi bruger iterator i til at numerere nyheden
    for (int i = 0; i < feed.Items.Count; i++)
    {
        await Task.Delay(rssWaitTime);  // her venter vi i "rssWaitTime" millisekunder, betyder at WineSys opdateres feed.Items.Count * rssWaitTime ms
        string rssCountText = (i + 1) + "/" + feed.Items.Count; // for at gøre linjen under lidt simplere laver vi "antal rss" her og gemmer det
        boxRSS.Clear(); // clear box ellers, teknisk set, indeholder den meget mere tekst og kan bugg, her slettes indhold og der skrives kun den ene linje fra rss
        boxRSS.WriteLine(feed.Items[i].Title + "\n" + rssCountText);    // "\n" går til næste linje, og så skriver vi hvilken nyhed det er
    }
}

// dette stykke kode blev repeteret ofte og er derfor lavet til sin egen metode
// metoden laver bare 10 beeps
void DoBeeps()
{
    for (int i = 0; i < 10; i++)
    {
        Console.Beep(300, 500);
    }
}

// dette er noget gammelt test kode, til at lave en metode for at gøre det simplere at fylde boksene med tekst
// fordi den netop opfører sig meget ens
// kan ignoreres
void UpdateWineText(IConsole console, string text, DVI_Stock stock)
{
    console.Clear();
    foreach (Wine wine in stock.WinesOnStock())
    {
        console.WriteLine("\n" + wine.NumInStock.ToString());
    }
}

// noget test code til at skrive intro logo ascii langsommere og løbende, bruges pt ikke
void ShowAsciiSlow()
{
    string test = "";
    int index = 0;
    int oldIndex = 0;

    while (index < asciiart.Length - 1)
    {
        oldIndex = index;
        index += 5;
        if (index >= asciiart.Length)
        {
            index = asciiart.Length - 1;
        }
        test = asciiart.Substring(oldIndex, index);
        //await Task.Delay(10);
        //Console.Clear();
        Console.Write(test);
    }
}