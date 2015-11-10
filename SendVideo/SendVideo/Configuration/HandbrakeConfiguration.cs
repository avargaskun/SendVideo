using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendVideo.Configuration
{
    public class HandbrakeConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path => (string)this["path"];

        [ConfigurationProperty("arguments", IsRequired = false)]
        public string Arguments => (string)this["arguments"];
    }
}
