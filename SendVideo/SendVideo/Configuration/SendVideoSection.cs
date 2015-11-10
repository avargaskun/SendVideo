using System.Configuration;

namespace SendVideo.Configuration
{
    public class SendVideoSection : ConfigurationSection
    {
        [ConfigurationProperty("handbrake", IsRequired = true)]
        public HandbrakeConfiguration Handbrake => (HandbrakeConfiguration)this["handbrake"];

        [ConfigurationProperty("smtp", IsRequired = true)]
        public SmtpServer SmptServer => (SmtpServer)this["smtp"];

        [ConfigurationProperty("observedFolders", IsRequired = true)]
        public ObservedFolderCollection ObservedFolders => (ObservedFolderCollection)this["observedFolders"];

        [ConfigurationProperty("recipients", IsRequired = true)]
        public RecipientCollection Recipients => (RecipientCollection)this["recipients"];
    }
}
