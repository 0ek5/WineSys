using CodeHollow.FeedReader;
using DVI_Access_Lib;
using Konsole;

Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

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
                                                                                                      ";

Console.WriteLine(asciiart);
await Task.Delay(3000);
Console.Clear();


Console.ForegroundColor = ConsoleColor.White;
Console.BackgroundColor = ConsoleColor.Black;

// Remember to add time and date instead of only timezone!!!

TimeZoneInfo dktime = TimeZoneInfo.FindSystemTimeZoneById("Romance Standard Time");
TimeZoneInfo calitime = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
TimeZoneInfo austime = TimeZoneInfo.FindSystemTimeZoneById("W. Australia Standard Time");

DateTime utcNow = DateTime.UtcNow;

DateTime dkTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, dktime);
DateTime caliTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, calitime);
DateTime ausTimeNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, austime);


// Window.OpenBox(string title, int sx, int sy, int width, int height, BoxStyle style)            

var winListWindow = Window.OpenBox("Lagerbeholdning for høj", 5, 2, 50, 17, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
});


var winListWindow2 = Window.OpenBox("Lagerbeholdning lav", 5, 19, 50, 13, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
});

var Vin = Window.OpenBox("Lagerbeholdning", 55, 2, 50, 30, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)

});




var Vin1 = Window.OpenBox("Antal", 105, 2, 15, 30, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)

});

var Vin2 = Window.OpenBox("Indkøbspris", 120, 2, 15, 30, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)

});

var Vin3 = Window.OpenBox("Avance", 135, 2, 20, 30, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)

});


var winCurrTempCurrHum = Window.OpenBox("Temperatur", 5, 32, 25, 6, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
});

var winCurrTempCurrHum2 = Window.OpenBox("Luftfugtighed", 30, 32, 25, 6, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
});

var Box3 = Window.OpenBox("Klokken", 55, 32, 50, 6, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
});

var Box4 = Window.OpenBox("Seneste nyt fra DR.dk", 105, 32, 50, 6, new BoxStyle()
{
    ThickNess = LineThickNess.Double,
    Title = new Colors(ConsoleColor.White, ConsoleColor.Red)
});


DVI_Climate climate = new DVI_Climate("http://docker.data.techcollege.dk:5051");
DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

while (true)
{





    Vin.Clear();
    foreach (Wine wine in stock.WinesOnStock())
        Vin.WriteLine("\n" + wine.WineName);

    Vin1.Clear();
    foreach (Wine wine in stock.WinesOnStock())
        Vin1.WriteLine("\n" + wine.NumInStock.ToString());

    Vin2.Clear();
    foreach (Wine wine in stock.WinesOnStock())
        Vin2.WriteLine("\n" + wine.PurchasePrice + " DKK");

    Vin3.Clear();
    foreach (Wine wine in stock.WinesOnStock())
        Vin3.WriteLine("\n" + wine.PurchasePrice * wine.MarkUp + " DKK" + " | " + wine.MarkUp * 100 + "%");





    winListWindow.Clear();
    foreach (Wine wine in stock.StockOverThreshold())
        winListWindow.WriteLine(wine.WineName);


    winListWindow2.Clear();
    foreach (Wine wine in stock.StockUnderThreshold())
        winListWindow2.WriteLine(wine.WineName);


    Box3.Clear();
    Box3.WriteLine("Danmark " + "    " + dkTimeNow + "\n" + "Californien " + caliTimeNow + "\n" + "Australien " + " " + ausTimeNow);

    // RSS Feed

    winCurrTempCurrHum.Clear();
    winCurrTempCurrHum.WriteLine(climate.CurrTemp().ToString("0.00") + "°");

    winCurrTempCurrHum2.Clear();
    winCurrTempCurrHum2.WriteLine(climate.CurrHum().ToString("0.00") + "%");




    if (climate.CurrTemp() <= climate.MinTemp())
    {
        winCurrTempCurrHum.WriteLine(ConsoleColor.Red, "LAV TEMP!");
        Console.Beep();
    }

    if (climate.CurrTemp() >= climate.MaxTemp())
    {
        winCurrTempCurrHum.WriteLine(ConsoleColor.Red, "HØJ TEMP!");
        Console.Beep();
    }

    if (climate.CurrHum() < climate.MinHum())
    {
        winCurrTempCurrHum2.WriteLine(ConsoleColor.Red, "LAV HUM!");
        Console.Beep();
    }

    if (climate.CurrTemp() > climate.MaxHum())
    {
        winCurrTempCurrHum2.WriteLine(ConsoleColor.Red, "HØJ HUM!");
        Console.Beep();
    }

    if (climate.CurrTemp() < climate.MaxTemp() && climate.CurrTemp() > climate.MinTemp())
    {
        winCurrTempCurrHum.WriteLine(ConsoleColor.Green, "ALT I ORDEN!");
    }

    if (climate.CurrHum() < climate.MaxHum() && climate.CurrHum() > climate.MinHum())
    {
        winCurrTempCurrHum2.WriteLine(ConsoleColor.Green, "ALT I ORDEN!");
    }

    var feed = await FeedReader.ReadAsync("https://www.dr.dk/nyheder/service/feeds/senestenyt");
    Box4.Clear();
    foreach (var item in feed.Items)
    {
        await Task.Delay(3000);
        Box4.WriteLine(item.Title + "\n");
    }

    await Task.Delay(20);
}
