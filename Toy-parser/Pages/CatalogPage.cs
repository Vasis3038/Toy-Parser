using AngleSharp.Dom;

namespace Toy_Parser.Pages;

public class CatalogPage
{
    private const string ToysListLocator = "[class='row mt-2']";
    private const string PagesListLocator = "[class='pagination justify-content-between']";

    public string GetNextCotalogPageLink(IDocument doc)
    {
        return doc.QuerySelector(PagesListLocator)?.Children.Last().Children.First().GetAttribute("href");
    }

    public List<string> GetToysPagesLinksList(IDocument doc)
    {
        List<string> toysPagesLinksList = new();
        var e = doc.QuerySelector(ToysListLocator);
        IHtmlCollection<IElement> toysPages = doc.QuerySelector(ToysListLocator)?.Children;

        if (toysPages is null)
        {
            return null;
        }

        foreach (var element in toysPages)
        {
            var metaElements = element.GetElementsByTagName("meta");

            if (metaElements.Count() == 0)
            {
                continue;
            }

            var href = metaElements.First().GetAttribute("content");

            if (href != null)
            {
                toysPagesLinksList.Add(href);
            }
        }

        return toysPagesLinksList;
    }
}
