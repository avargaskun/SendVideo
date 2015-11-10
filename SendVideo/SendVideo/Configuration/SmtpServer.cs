using System.Configuration;

namespace SendVideo.Configuration
{
    public class SmtpServer : ConfigurationElement
    {
        [ConfigurationProperty("username")]
        public string Username => (string)this["username"];

        [ConfigurationProperty("password")]
        public string Password => (string)this["password"];

        [ConfigurationProperty("from")]
        public string From => (string)this["from"];

        [ConfigurationProperty("subject")]
        public string Subject => (string)this["subject"];

        [ConfigurationProperty("address")]
        public string Address => (string)this["address"];

        [ConfigurationProperty("port")]
        public int Port => (int)this["port"];

        [ConfigurationProperty("useSsl")]
        public bool UseSsl => (bool)this["useSsl"];
    }
}
