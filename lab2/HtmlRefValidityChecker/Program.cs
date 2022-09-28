using HtmlRefValidityChecker.HtmlParser;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

HtmlLinkParser htmlLinkParser = new HtmlLinkParser(new Uri("http://links.qatl.ru/"));
ISet<string> links = new HashSet<string>();
links = htmlLinkParser.GetAllPageLinks();
foreach (var link in links)
{
    Console.WriteLine(link);
    Console.WriteLine(htmlLinkParser.ConcatUriWithLink(link).ToString());
    int result = await htmlLinkParser.CheckUriAccessibility(htmlLinkParser.ConcatUriWithLink(link));
    Console.WriteLine(result);
}
