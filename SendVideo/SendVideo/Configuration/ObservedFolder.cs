using System.Configuration;

namespace SendVideo.Configuration
{
    public class ObservedFolder : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true, IsKey = true)]
        public string Path => (string)this["path"];

        [ConfigurationProperty("message")]
        public string Message => (string)this["message"];

        [ConfigurationProperty("recipients", IsRequired = true)]
        public string Recipients => (string)this["recipients"];
    }
}
