using System.Configuration;

namespace SendVideo.Configuration
{
    public class Recipient : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name => (string)this["name"];

        [ConfigurationProperty("address", IsRequired = true)]
        public string Address => (string)this["address"];

        [ConfigurationProperty("attachmentName")]
        public string AttachmentName => (string)this["attachmentName"];

        [ConfigurationProperty("contentType")]
        public string ContentType => (string)this["contentType"];
    }
}
