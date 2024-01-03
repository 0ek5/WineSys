using DVI_Access_Lib;
using Konsole;

internal class Program
{

    static void Main(string[] args)
    {
        var winListWindow = Window.OpenBox("Vine på lager", 60, 25);

        DVI_Stock stock = new DVI_Stock("http://docker.data.techcollege.dk:5051");

        List<Wine> winesInStock = stock.WinesOnStock();

        foreach (Wine wine in winesInStock)
        {
            winListWindow.WriteLine(wine.WineName + " " + wine.NumInStock);
        }
    }
}