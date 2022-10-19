using AngleSharp;
using System.Text;
using Toy_Parser.Pages;

public class Program
{
    private const string Host = @"https://www.toy.ru";
    private static string url = @"https://www.toy.ru/catalog/boy_transport/";
    private const string PathToDataFile = @"";

    private static async Task Main(string[] args)
    {
        var config = Configuration.Default.WithDefaultLoader();
        using var context = BrowsingContext.New(config);
        CatalogPage catalogPage = new();

        do
        {
            StringBuilder result = new();
            using var doc = await context.OpenAsync(url);

            List<string> toysPagesLinks = catalogPage.GetToysPagesLinksList(doc);

            foreach (string toyPageLink in toysPagesLinks)
            {

                using var subDoc = await context.OpenAsync(toyPageLink);
                var toyPage = new ToyPage(subDoc);

                Task<string> regionTask = toyPage.GetRegionNameAsync();
                Task<string> priceTask = toyPage.GetPriceAsync();
                Task<string> oldPriceTask = toyPage.GetOldPriceAsync();
                Task<string> avaliableTask = toyPage.GetAvailabilityAsync();
                Task<string> pictureLinkTask = toyPage.GetPictureLinkAsync();
                Task<string> nameTask = toyPage.GetProductNameAsync();
                Task<string> breadCrumbsTask = toyPage.GetBreadCrumbsAsync();

                result.AppendJoin(";", new List<string> {await regionTask, await breadCrumbsTask, await nameTask, await priceTask,
                         await oldPriceTask, await avaliableTask , toyPageLink, await pictureLinkTask})
                      .Append("\n");
            }

            File.AppendAllTextAsync(PathToDataFile, result.ToString());
            url = Host + catalogPage.GetNextCotalogPageLink(doc);
        }
        while (!url.Equals(Host + "#"));
    }
}
