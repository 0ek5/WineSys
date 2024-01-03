using DVI_Access_Lib;

internal class Program
{
    static async Task Main(string[] args)
    {
        DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

        Console.WriteLine("Vine på lager:");

        List<Wine> winesInStock = stock.WinesOnStock();

        foreach (Wine wine in winesInStock)
        {
            Console.WriteLine(wine.WineName + " " + wine.NumInStock);
        }

        Console.ReadLine();
    }
}