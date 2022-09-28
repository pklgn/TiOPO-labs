using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HtmlRefValidityChecker.HtmlParser;

class HtmlLinkParser
{
				private ISet<string> m_paths;
				private HtmlWeb m_htmlWeb;
    private Uri m_uri;
    private HttpClient m_httpClient = new HttpClient();

    public HtmlLinkParser(Uri uri)
				{
								m_paths = new HashSet<string>();
								m_htmlWeb = new HtmlWeb();
        m_uri = uri;
				}

				private void TraverseAllPageLinks(Uri uri)
				{
        HtmlDocument doc = m_htmlWeb.Load(uri);
        var paths = doc.DocumentNode.Descendants("a")
                        .Select(a => a.GetAttributeValue("href", null))
                        .Where(path => !String.IsNullOrEmpty(path));

        foreach (var path in paths)
        {
            // Try concatenating uri with path assuming it as a relative path
            Uri? pathCombinedUri = TryGetUriWithPath(uri, path);
            AccountPagePath(pathCombinedUri);

            // Try creating uri with path assuming it as an absolute path
            Uri.TryCreate(path, UriKind.Absolute, out Uri? pathUri);
            AccountPagePath(pathUri);
        }
				}

    public ISet<string> GetAllPageLinks()
    {
        TraverseAllPageLinks(m_uri);

        return m_paths;
    }

    public static Uri? TryGetUriWithPath(Uri uri, string path)
    {
        Uri? pathUri = new Uri("about:blank");
        bool pathUriWasCreated = Uri.TryCreate(uri, path, out pathUri);

        if (!pathUriWasCreated || pathUri == null)
        {
            return null;
        }

        return pathUri;
    }

    private void AccountPagePath(Uri? uri)
    {
        if (uri != null &&
            m_uri.Host == uri.Host &&
            m_paths.Add(uri.LocalPath))
        {
            TraverseAllPageLinks(uri);
        }
    }

    public Uri GetUriWithPath(string path)
    {
        return new Uri(String.Format("{0}/{1}", m_uri.ToString().TrimEnd('/'), path.TrimStart('/')));
    }

    public async Task<int> GetUriStatusCode(Uri uri)
    {
        HttpResponseMessage webResponse = await m_httpClient.GetAsync(uri);

        return (int)webResponse.StatusCode; ;
    }
}
