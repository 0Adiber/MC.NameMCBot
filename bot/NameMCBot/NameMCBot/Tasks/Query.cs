using OQ.MineBot.PluginBase;
using OQ.MineBot.PluginBase.Base.Plugin.Tasks;
using OQ.MineBot.PluginBase.Classes.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NameMCBot.Tasks
{
    class Query : ITask
    {
        public Query()
        {

        }

        public override bool Exec()
        {
            return true;
        }

        public override void Start()
        {
            player.events.onChat += OnChat;
        }

        public override void Stop()
        {
            player.events.onChat -= OnChat;
        }

        private void OnChat(IPlayer player, IChat message, byte position)
        {
            string msg = message.GetText();
            
            if(msg.StartsWith("[Clans]"))
            {
                if (!msg.Contains(":")) return;
                string[] parts = msg.Split(new char[] { ':' });
                if (!parts[1].Trim().StartsWith(".nr")) return;

                string[] cmd = parts[1].Trim().Split(new char[] { ' ' });
                if (cmd.Length != 2) return;

                if(cmd[1].Trim().Contains("."))
                {
                    player.functions.Chat("/cc [NameMCBot] Keine Punkte! Weil ich nicht weiß ob das vielleicht eine Server-IP ist.");
                    return;
                }

                string site = GetPageAsString("https://de.namemc.com/search?q=" + cmd[1].Trim());

                if(site.StartsWith("ERROR: "))
                {
                    player.functions.Chat("/cc [NameMCBot] " + site + " Zweiter Versuch..");
                    site = GetPageAsString("https://de.namemc.com/search?q=" + cmd[1].Trim());
                    if(site.StartsWith("ERROR: "))
                    {
                        player.functions.Chat("/cc [NameMCBot] Zweiter Versuch fehlgeschlagen. " + site);
                        return;
                    }
                }

                if(site.Contains("Profile: 0 Ergebnisse"))
                {
                    player.functions.Chat("/cc [NameMCBot] Keine Ergebnisse zum Namen: " + cmd[1]);
                    return;
                }

                int start = site.IndexOf("py-0");
                site = site.Substring(start, site.IndexOf("/main") - start);

                player.functions.Chat("/cc [NameMCBot] -- \"" + cmd[1] + "\" -- ");

                int cnt = 0;
                foreach (string p in getHtmlSplitted(site))
                {
                    if (String.IsNullOrWhiteSpace(p)) break;
                    player.functions.Chat("/cc - " + p);
                    cnt++;
                    if (cnt == 10) return;
                }

                player.functions.Chat("/cc [NameMCBot] -- ENDE -- ");

            }
        }

        private static string GetPageAsString(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            request.Credentials = CredentialCache.DefaultCredentials;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if(response.StatusCode != HttpStatusCode.OK)
            {
                return "ERROR: " + response.StatusCode;
            }

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
                    names.Add(inTagValue.Substring(inTagValue.LastIndexOf(">") + 1));
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
            foreach (var n in names)
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
