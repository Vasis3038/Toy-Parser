using AngleSharp;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;
using Toy_Parser.Pages;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

public class Program
{
    private const string GetBodyXPathSelector = @"/html/body";
    private const string Url = @"https://www.toy.ru/catalog/boy_transport/";
    private const string PathToDataFile = @"D:\findjob\notissimus\testparser\Repo\Toy-parser\ToysData.csv";
    private const string RegionSelector = "[rel='61000001000']";
    private const string OpenRegionsSelector = "[data-src='#region']";
    private const string NextButtonSelector = "[rel='nofollow'][class='page-link']";
    private const string Region = "Ростов-на-Дону";

    private static async Task Main(string[] args)
    {

        new DriverManager().SetUpDriver(new ChromeConfig());
        var webDriver = new ChromeDriver();
        webDriver.Navigate().GoToUrl(Url);
        var openRegionsButton = webDriver.FindElement(By.CssSelector(OpenRegionsSelector));
        var regoinButton = webDriver.FindElement(By.CssSelector(RegionSelector));
        openRegionsButton.Click();
        regoinButton.Click();

        var config = Configuration.Default.WithDefaultLoader();
        using var context = BrowsingContext.New(config);
        CatalogPage catalogPage = new();

        while (true)
        {
            StringBuilder result = new();
            Thread.Sleep(2000);

            var allHtmlElement = webDriver.FindElement(By.XPath(GetBodyXPathSelector));
            Thread.Sleep(2000);
            var allHtml = allHtmlElement.GetAttribute("innerHTML");

            using var doc = await context.OpenAsync(req => req.Content(allHtml));


            List<string> toysPagesLinks = catalogPage.GetToysPagesLinksList(doc);

            foreach (string toyPageLink in toysPagesLinks)
            {
                using var subDoc = await context.OpenAsync(toyPageLink);
                var toyPage = new ToyPage(subDoc);

                Task<string> priceTask = toyPage.GetPriceAsync();
                Task<string> oldPriceTask = toyPage.GetOldPriceAsync();
                Task<string> avaliableTask = toyPage.GetAvailabilityAsync();
                Task<string> pictureLinkTask = toyPage.GetPictureLinkAsync();
                Task<string> nameTask = toyPage.GetProductNameAsync();
                Task<string> breadCrumbsTask = toyPage.GetBreadCrumbsAsync();

                result.AppendJoin(";", new List<string> {Region, await breadCrumbsTask, await nameTask, await priceTask,
                         await oldPriceTask, await avaliableTask , toyPageLink, await pictureLinkTask})
                      .Append("\n");
            }

            File.AppendAllTextAsync(PathToDataFile, result.ToString());
            var nextButtons = webDriver.FindElements(By.CssSelector(NextButtonSelector));

            if (nextButtons.Count == 3)
            {
                webDriver.Close();
                return;
            }

            nextButtons.Last().Click();
        }
    }
}
