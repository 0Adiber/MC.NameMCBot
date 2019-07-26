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
                if (!parts[1].Trim().StartsWith(".")) return;

                string[] cmd = parts[1].Split(new char[] { ' ' });
                if (!(cmd.Length != 2)) player.functions.Chat("/cc [NameMCBot] Befehl: '.nr <name>'");

                string site = GetPageAsString("https://de.namemc.com/search?q=" + cmd[1]);

                int start = site.IndexOf("py-0");
                site = site.Substring(start, site.IndexOf("/main") - start);

                //List<string> result = new List<string>();

                player.functions.Chat("/cc [NameMCBot] ~-~-~-~-~ \"" + cmd[1] + "\"~-~-~-~-~");

                foreach (string p in getHtmlSplitted(site))
                {
                    if (String.IsNullOrWhiteSpace(p)) break;
                    //result.Add(p);
                    player.functions.Chat("/cc - " + p);
                }

                player.functions.Chat("/cc [NameMCBot] ~-~-~-~-~ENDE~-~-~-~-~");

            }
        }

        private static string GetPageAsString(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.106 Safari/537.36";
            request.Credentials = CredentialCache.DefaultCredentials;

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
                    list.Add(inTagValue.Substring(inTagValue.LastIndexOf(">") + 1));
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
