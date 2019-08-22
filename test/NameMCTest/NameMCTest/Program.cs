using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace NameMCTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string site = dummerScheiss("https://de.namemc.com/search?q=computiful1975");
            
            int start = site.IndexOf("py-0");
            site = site.Substring(start, site.IndexOf("/main")-start);

            string[] names = getHtmlSplitted(site);

            // getting page number
            int times = 1;
            Int32.TryParse("1 ".Trim(), out times);

            if (names.Length < (10 * (times - 1)) + 1) times = (int)((names.Length - names.Length % 10) / 10)+1;

            //output start
            Console.WriteLine("/cc [NameMCBot] -- laasdhf -- ");

            //outputting names based on page number
            for (int i = 10 * (times-1); i < 10*times && i < names.Length; i++)
            {
                if (String.IsNullOrWhiteSpace(names[i])) break;
                Console.WriteLine("/cc - " + names[i]);
            }

            //output end "ende" or page number
            if (names.Length <= 10 * times)
            {
                Console.WriteLine("/cc [NameMCBot] -- ENDE -- ");
            }
            else
            {
                Console.WriteLine("/cc [NameMCBot] --- " + times + " --- ");
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

        public static string dummerScheiss(string url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            request.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            WebResponse response = request.GetResponse();

            string responseFromServer = "";
            using (Stream dataStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(dataStream);
                responseFromServer = reader.ReadToEnd();
            }

            response.Close();
            return responseFromServer;
        }

        public static string[] getHtmlSplitted(string text)
        {
            var names = new List<string>();
            var times = new List<string>();
            var list = new List<string>();

            //
            //names
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
                    names.Add(inTagValue.Substring(inTagValue.LastIndexOf(">")+1));
                    continue;
                }

                if (isInTag)
                {
                    inTagValue = subStr;
                    continue;
                }
            }

            //
            //times
            pattern = "(<time|</time>)";
            isInTag = false;
            inTagValue = String.Empty;


            foreach (var subStr in Regex.Split(text, pattern))
            {
                if (subStr.Equals("<time"))
                {
                    isInTag = true;
                    continue;
                }
                else if (subStr.Equals("</time>"))
                {
                    isInTag = false;
                    times.Add(inTagValue.Substring(inTagValue.LastIndexOf(">") + 1));
                    continue;
                }

                if (isInTag)
                {
                    inTagValue = subStr;
                    continue;
                }
            }

            //
            //combine

            names.RemoveAt(0);

            int i = 0;
            foreach(var n in names)
            {
                if (string.IsNullOrWhiteSpace(n)) break;
                string t = (i < times.Count ? times[i++] : "").Replace("T", "@").Replace("-", ".");
                list.Add(n + " - " + Reverse(t.Substring(0, t.IndexOf("@") == -1 ? t.Length : t.IndexOf("@"))));
            }

            string temp = list.Last();
            list.RemoveAt(list.Count - 1);
            list.Add(temp.Substring(0, temp.IndexOf(" - ")));

            return list.ToArray();
        }

        private static string Reverse(string s)
        {
            string[] parts = s.Split(new char[] { '.' });
            if (parts.Length == 3)
            {
                return parts[2] + "." + parts[1] + "." + parts[0];
            }
            return "";
        }

    }
}
