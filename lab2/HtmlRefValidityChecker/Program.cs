using HtmlRefValidityChecker.HtmlParser;
using HtmlRefValidityChecker.Exceptions;

// See https://aka.ms/new-console-template for more information

Uri GetUriFromArgs(string[] args, uint argsCount = 1)
{
    if (args.Length == 0)
    {
        throw new NotEnoughArgumentsException();
    }

    if (args.Length > argsCount)
    {
        Console.WriteLine("Extra parameters will be ignored");
    }

    if (!Uri.TryCreate(args[0], UriKind.Absolute, out Uri? uri))
    {
        throw new NotValidStringUriValue();
    }

    return uri;
}

async Task RunHtmlPageLinkScraping(Uri pageUri)
{
    HtmlLinkParser htmlLinkParser = new HtmlLinkParser(pageUri);
    ISet<string> links = htmlLinkParser.GetAllPageLinks();

    using StreamWriter successLinksFile = new("SUCCESS.TXT");
    using StreamWriter errorLinksFile = new("ERROR.TXT");

    foreach (var link in links)
    {
        Uri absoluteUri = htmlLinkParser.GetUriWithLocalPath(link);
        int statusCode = await htmlLinkParser.GetUriStatusCode(absoluteUri);


        if (HtmlLinkParser.IsSuccessStatusCode(statusCode))
        {
            successLinksFile.WriteLine(String.Format("{0} - {1}", absoluteUri.ToString(), statusCode));
        }
        else
        {
            errorLinksFile.WriteLine(String.Format("{0} - {1}", absoluteUri.ToString(), statusCode));
        }
    }
}
Uri uri = GetUriFromArgs(args);
await RunHtmlPageLinkScraping(uri);
