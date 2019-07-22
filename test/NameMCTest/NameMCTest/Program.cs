using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NameMCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string site = GetPageAsString("https://de.namemc.com/search?q=MrKuh");
            
            int start = site.IndexOf("py-0");
            site = site.Substring(start, site.IndexOf("/main")-start);

            string[] parts = getHtmlSplitted(site);
            List<string> result = new List<string>();

            foreach(string p in parts)
            {
                if (String.IsNullOrWhiteSpace(p)) break;
                result.Add(p);
            }

            Console.ReadKey();
        }

        public static string GetPageAsString(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";

            var content = String.Empty;
            HttpStatusCode statusCode;
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                var contentType = response.ContentType;
                Encoding encoding = null;
                if (contentType != null)
                {
                    var match = Regex.Match(contentType, @"(?<=charset\=).*");
                    if (match.Success)
                        encoding = Encoding.GetEncoding(match.ToString());
                }

                encoding = encoding ?? Encoding.UTF8;

                statusCode = ((HttpWebResponse)response).StatusCode;
                using (var reader = new StreamReader(stream, encoding))
                    content = reader.ReadToEnd();
            }
            return content;
        }

        public static string[] getHtmlSplitted(string text)
        {
            var list = new List<string>();
            var pattern = "(<a|</a>)";
            var isInTag = false;
            var inTagValue = String.Empty;

            foreach (var subStr in Regex.Split(text, pattern))
            {
                if (subStr.Equals("<a"))
                {
                    isInTag = true;
                    continue;
                }
                else if (subStr.Equals("</a>"))
                {
                    isInTag = false;
                    list.Add(inTagValue.Substring(inTagValue.LastIndexOf(">")+1));
                    continue;
                }

                if (isInTag)
                {
                    inTagValue = subStr;
                    continue;
                }

                //list.Add(subStr);

            }
            list.RemoveAt(0);
            return list.ToArray();
        }

    }
}
