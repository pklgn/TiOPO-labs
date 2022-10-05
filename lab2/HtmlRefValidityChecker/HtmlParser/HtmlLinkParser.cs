using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HtmlRefValidityChecker.HtmlParser;

class HtmlLinkParser
{
    private SortedSet<string> m_paths;
    private HtmlWeb m_htmlWeb;
    private Uri m_uri;
    private HttpClient m_httpClient = new HttpClient();

    public HtmlLinkParser(Uri uri)
    {
        m_paths = new SortedSet<string>();
        m_htmlWeb = new HtmlWeb();
        m_uri = uri;
    }

    public SortedSet<string> GetAllPageLinks()
    {
        TraverseAllPageLinks(m_uri);

        return m_paths;
    }

    private static Uri? TryGetUriWithLocalPath(Uri uri, string localPath)
    {
        Uri.TryCreate(uri, localPath, out Uri? resultUri);

        return resultUri;
    }

    private static Uri? TryGetUriWithAbsolutePath(string absolutePath)
    {
        Uri.TryCreate(absolutePath, UriKind.Absolute, out Uri? resultUri);

        return resultUri;
    }

    public Uri GetUriWithLocalPath(string localPath)
    {
        return new Uri(String.Format("{0}/{1}", m_uri.ToString().TrimEnd('/'), localPath.TrimStart('/')));
    }

    public async Task<int> GetUriStatusCode(Uri uri)
    {
        HttpResponseMessage webResponse = await m_httpClient.GetAsync(uri);

        return (int)webResponse.StatusCode;
    }

    public static bool IsSuccessStatusCode(int statusCode)
    {
        return ((int)statusCode >= 200) && ((int)statusCode <= 299);
    }

    private void TraversePagePath(Uri? uri)
    {
        if (uri != null &&
            m_uri.Host == uri.Host &&
            m_paths.Add(uri.LocalPath))
        {
            TraverseAllPageLinks(uri);
        }
    }

    private void TraverseAllPageLinks(Uri pageUri)
    {
        HtmlDocument doc = m_htmlWeb.Load(pageUri);
        var paths = doc.DocumentNode.Descendants("a")
                        .Select(a => a.GetAttributeValue("href", null))
                        .Where(path => !String.IsNullOrEmpty(path));

        foreach (var path in paths)
        {
            // Try concatenating uri with path assuming it as a relative path
            TraversePagePath(TryGetUriWithLocalPath(pageUri, path));

            // Try creating uri with path assuming it as an absolute path
            TraversePagePath(TryGetUriWithAbsolutePath(path));
        }
    }
}
