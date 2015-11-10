using System.Configuration;

namespace SendVideo.Configuration
{
    public class RecipientCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Recipient();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Recipient)element).Name;
        }
    }
}
