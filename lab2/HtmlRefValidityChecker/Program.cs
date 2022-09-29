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

void AppendInfoWithDateTime(StreamWriter streamWriter, string info, string dateTimeFormat = "g")
{
    streamWriter.WriteLine();
    streamWriter.WriteLine(info);
    streamWriter.WriteLine(String.Format("Executed - {0}", DateTime.Now.ToString(dateTimeFormat)));
}

async Task RunHtmlPageLinkScraping(Uri pageUri)
{
    HtmlLinkParser htmlLinkParser = new HtmlLinkParser(pageUri);
    SortedSet<string> links = htmlLinkParser.GetAllPageLinks();

    using StreamWriter successLinksFile = new("SUCCESS.TXT");
    using StreamWriter errorLinksFile = new("ERROR.TXT");

    uint successCounter = 0, errorCounter = 0;
    foreach (var link in links)
    {
        
        Uri absoluteUri = htmlLinkParser.GetUriWithLocalPath(link);
        int statusCode = await htmlLinkParser.GetUriStatusCode(absoluteUri);

        if (HtmlLinkParser.IsSuccessStatusCode(statusCode))
        {
            successLinksFile.WriteLine(String.Format("{0} - {1}", absoluteUri.ToString(), statusCode));
            ++successCounter;
        }
        else
        {
            errorLinksFile.WriteLine(String.Format("{0} - {1}", absoluteUri.ToString(), statusCode));
            ++errorCounter;
        }
    }
    AppendInfoWithDateTime(successLinksFile, String.Format("Links found number - {0}", successCounter));
    AppendInfoWithDateTime(errorLinksFile, String.Format("Links found number - {0}", errorCounter));

}

try
{
    Uri uri = GetUriFromArgs(args);
    await RunHtmlPageLinkScraping(uri);

    Console.WriteLine("Done!");
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}
