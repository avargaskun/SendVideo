using System.Configuration;

namespace SendVideo.Configuration
{
    public class ObservedFolderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ObservedFolder();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ObservedFolder)element).Path;
        }
    }
}
