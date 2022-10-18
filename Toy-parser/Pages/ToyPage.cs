using AngleSharp.Dom;
using System.Text;

namespace Toy_Parser.Pages;

public class ToyPage
{
    private const string RegionSelector = "[data-src='#region']";
    private const string PriceSelector = "[class='price']";
    private const string OldPriceSelector = "[class='old-price']";
    private const string AvailabilitySelector = "[class='col-6 py-2'] + div > span ";
    private const string PictureLinkSelector = "[class='col-12 col-md-10 col-lg-7'] a";
    private const string BreadCrumbsSelector = "[class='breadcrumb']";
    private const string ProductNameSelector = "[class='detail-name']";
    private IDocument doc;

    public ToyPage(IDocument doc)
    {
        this.doc = doc;
    }

    public async Task<string> GetBreadCrumbsAsync()
    {
        StringBuilder sb = new();
        var crumbs = doc.QuerySelector(BreadCrumbsSelector).Children;
        string prefix = string.Empty;

        for (int i = 0; i < crumbs.Count() - 1; i++)
        {
            var crumb = crumbs.ElementAt(i);
            sb.Append(prefix);
            sb.Append(crumb.TextContent is null ? crumb.QuerySelector("span")?.TextContent : crumb.TextContent);
            prefix = "->";
        }

        return sb.ToString();
    }

    public async Task<string> GetRegionNameAsync()
    {
        return doc.QuerySelector(RegionSelector)?.TextContent.Replace('\t', ' ').Replace('\n', ' ').Trim();
    }

    public async Task<string> GetPriceAsync()
    {
        return doc.QuerySelector(PriceSelector)?.TextContent;
    }

    public async Task<string> GetOldPriceAsync()
    {
        return doc.QuerySelector(OldPriceSelector)?.TextContent;
    }

    public async Task<string> GetAvailabilityAsync()
    {
        return doc.QuerySelector(AvailabilitySelector)?.TextContent;
    }

    public async Task<string> GetPictureLinkAsync()
    {
        return doc.QuerySelector(PictureLinkSelector)?.GetAttribute("href");
    }

    public async Task<string> GetProductNameAsync()
    {
        return doc.QuerySelector(ProductNameSelector)?.TextContent;
    }
}
