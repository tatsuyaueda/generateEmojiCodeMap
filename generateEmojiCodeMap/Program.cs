using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace generateEmojiCodeMap
{
    class Program
    {

        static string gemojiDBJsonURL = "https://raw.githubusercontent.com/github/gemoji/master/db/emoji.json";

        static void Main(string[] args)
        {

            Console.WriteLine("Download Emoji DB");

            var jsonStr = string.Empty;

            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = System.Text.Encoding.UTF8;
                jsonStr = webClient.DownloadString(gemojiDBJsonURL);
            }

            Console.WriteLine("Parse Emoji DB");

            var jsonModel = JsonConvert.DeserializeObject<List<GEmoji>>(jsonStr);

            var fileName = System.IO.Path.Combine(
               System.AppDomain.CurrentDomain.BaseDirectory,
               DateTime.Now.ToString("emojiCodeMap_yyyyMMdd-hhmmss'.txt'")
            );

            Console.WriteLine("Output file:" + System.IO.Path.GetFileName(fileName));

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(fileName, false))
            {
                foreach (var item in jsonModel)
                {
                    if (item.Emoji == null)
                    {
                        item.Emoji = string.Empty;
                    }
                    var x = EncodeNonAsciiCharacters(item.Emoji);

                    foreach (var alias in item.Aliases)
                    {
                        sw.WriteLine(@"emojiCodeMap.Add("":{0}:"", ""{1}"");", alias.Replace(" ", "_"), x);
                    }

                }
                sw.Close();
            }

            Console.WriteLine("Complete");
            Console.WriteLine("Please enter key... to Exit Application");
            Console.ReadLine();


        }

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127)
                {
                    // This character is too big for ASCII
                    string encodedValue = "\\u" + ((int)c).ToString("X4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

    }

    /// <summary>
    /// JSONをパースするためのクラス
    /// </summary>
    public class GEmoji
    {
        [JsonProperty("emoji")]
        public string Emoji { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("aliases")]
        public List<string> Aliases { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("unicode_version")]
        public string UnicodeVersion { get; set; }

        [JsonProperty("ios_version")]
        public string IosVersion { get; set; }
    }
}
