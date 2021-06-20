using Newtonsoft.Json;

namespace DiscordDataGrabber.Models
{
    public class DiscordUser
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "discriminator")]
        public string Discriminator { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public object Phone { get; set; }

        [JsonProperty("verified")]
        public bool Verified { get; set; }
    }
}