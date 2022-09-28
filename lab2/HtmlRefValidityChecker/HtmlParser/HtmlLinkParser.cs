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

                public HtmlLinkParser(Uri uri)
				{
								m_paths = new HashSet<string>();
								m_htmlWeb = new HtmlWeb();
        m_uri = uri;
				}

				public void TraverseAllPageLinks(Uri uri)
				{
        HtmlDocument doc = m_htmlWeb.Load(uri);
        var paths = doc.DocumentNode.Descendants("a")
                        .Select(a => a.GetAttributeValue("href", null))
                        .Where(link => !String.IsNullOrEmpty(link));

        foreach (var path in paths)
        {
            Uri? pathCombinedUri = CombineUriWithPath(uri, path);
            if (pathCombinedUri != null)
            {
                if (m_uri.Host == pathCombinedUri.Host)
                {
                    AccountPageLink(pathCombinedUri);
                    //Console.WriteLine(pathCombinedUri.ToString());
                    continue;
                }
            }

            Uri.TryCreate(path, UriKind.Absolute, out Uri? pathUri);
            if (pathUri != null)
            {
                if (m_uri.Host == pathUri.Host)
                {
                    AccountPageLink(pathUri);
                    //Console.WriteLine(pathUri.ToString());
                }
            }
        }
				}

    public ISet<string> GetAllPageLinks()
    {
        TraverseAllPageLinks(m_uri);

        return m_paths;
    }

    public static Uri? CombineUriWithPath(Uri uri, string path)
    {
        Uri? pathUri = new Uri("about:blank");
        bool pathUriWasCreated = Uri.TryCreate(uri, path, out pathUri);

        if (!pathUriWasCreated || pathUri == null)
        {
            return null;
        }

        return pathUri;
    }

    public void AccountPageLink(Uri uri)
    {
        if (m_paths.Add(uri.LocalPath))
        {
            TraverseAllPageLinks(uri);
        }
    }

    public bool IsValidUri(Uri uri)
    {
        using (HttpClient Client = new HttpClient())
        {
            HttpResponseMessage result = Client.GetAsync(uri).Result;
            HttpStatusCode StatusCode = result.StatusCode;

            switch (StatusCode)
            {

                case HttpStatusCode.Accepted:
                    return true;
                case HttpStatusCode.OK:
                    return true;
                default:
                    return false;
            }
        }
    }

    public Uri ConcatUriWithLink(string path)
    {
        return new Uri(String.Format("{0}/{1}", m_uri.ToString().TrimEnd('/'), path.TrimStart('/')));
    }

    public async Task<int> CheckUriAccessibility(Uri uri)
    {
        HttpClient httpClient = new HttpClient();
        
        int statusNumber;
        HttpResponseMessage webResponse = await httpClient.GetAsync(uri);

        statusNumber = (int)webResponse.StatusCode;

        return statusNumber;
    }
}
