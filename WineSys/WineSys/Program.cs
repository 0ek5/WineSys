using CodeHollow.FeedReader;
using DVI_Access_Lib;
using Konsole;

Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);

string asciiart = @"
                                                                                                      
     ***** *    **   ***                                         *******                              
  ******  *  *****    ***     *                                *       ***                            
 **   *  *     *****   ***   ***                              *         **                            
*    *  **     * **      **   *                               **        *                             
    *  ***     *         **                                    ***          **   ****         ****    
   **   **     *         ** ***     ***  ****       ***       ** ***         **    ***  *    * **** * 
   **   **     *         **  ***     **** **** *   * ***       *** ***       **     ****    **  ****  
   **   **     *         **   **      **   ****   *   ***        *** ***     **      **    ****       
   **   **     *         **   **      **    **   **    ***         *** ***   **      **      ***      
   **   **     *         **   **      **    **   ********            ** ***  **      **        ***    
    **  **     *         **   **      **    **   *******              ** **  **      **          ***  
     ** *      *         *    **      **    **   **                    * *   **      **     ****  **  
      ***      ***      *     **      **    **   ****    *   ***        *     *********    * **** *   
       ******** ********      *** *   ***   ***   *******   *  *********        **** ***      ****    
         ****     ****         ***     ***   ***   *****   *     *****                ***             
                                                           *                   *****   ***            
                                                            **               ********  **             
                                                                            *      ****               
                                                                                         GRUPPE I ©          
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

var Vin = Window.OpenBox("Lagerbeholdning", 55, 2, 100, 30, new BoxStyle()
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

    winListWindow.Clear();
    foreach (Wine wine in stock.StockOverThreshold())
        winListWindow.WriteLine(wine.WineName);


    winListWindow2.Clear();
    foreach (Wine wine in stock.StockUnderThreshold())
        winListWindow2.WriteLine(wine.WineName);


    Box3.Clear();
    Box3.WriteLine("Danmark " + "    " + dkTimeNow + "\n" + "Californien " + caliTimeNow + "\n" + "Australien " + " " + ausTimeNow);


    var feed = await FeedReader.ReadAsync("https://www.dr.dk/nyheder/service/feeds/senestenyt");
    Box4.Clear();
    foreach (var item in feed.Items)
    
        Box4.WriteLine(item.Title + "\n");

    winCurrTempCurrHum.Clear();
    winCurrTempCurrHum.WriteLine(climate.CurrTemp().ToString("0.00") + "°");

    winCurrTempCurrHum2.Clear();
    winCurrTempCurrHum2.WriteLine(climate.CurrHum().ToString("0.00") + "%");

    //Equal or greater then on warnings.

    if (climate.CurrTemp() > climate.MaxTemp())
    {
        winCurrTempCurrHum.WriteLine("HIGH TEMP!");
        Console.Beep();
    }

    if (climate.CurrTemp() <= climate.MinTemp())
    {
        Console.ForegroundColor = ConsoleColor.Red;
        winCurrTempCurrHum.WriteLine("LOW TEMP!");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Beep();
    }

    if (climate.CurrTemp() >= climate.MaxTemp())
    {
        Console.ForegroundColor = ConsoleColor.Red;
        winCurrTempCurrHum.WriteLine("HIGH TEMP!");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Beep();
    }

    if (climate.CurrHum() < climate.MinHum())
    {
        winCurrTempCurrHum2.WriteLine("LOW HUM!");
        Console.Beep();
    }

    if (climate.CurrTemp() > climate.MaxHum())
    {
        winCurrTempCurrHum2.WriteLine("HIGH HUM!");
        Console.Beep();
    }

    if (climate.CurrTemp() < climate.MaxTemp() && climate.CurrTemp() > climate.MinTemp())
    {
        winCurrTempCurrHum.WriteLine("WORKING");
    }

    if (climate.CurrHum() < climate.MaxHum() && climate.CurrHum() > climate.MinHum())
    {
        winCurrTempCurrHum2.WriteLine("WORKING");
    }

    await Task.Delay(5000);
}
