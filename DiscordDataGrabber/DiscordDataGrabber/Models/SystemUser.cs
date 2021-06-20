using Newtonsoft.Json;
using System;

namespace DiscordDataGrabber.Models
{
    public class SystemUser
    {

        [JsonProperty(PropertyName = "ip")]
        public string IpAddress { get; set; }

    }
}