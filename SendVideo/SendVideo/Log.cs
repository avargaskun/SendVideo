using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendVideo
{
    public static class Log
    {
        public static TextWriter Out { get; set; } = new StreamWriter(File.Open(@"C:\SendVideo.log", FileMode.Create, FileAccess.ReadWrite, FileShare.Read));

        public static void Debug(string message)
        {
            Out.WriteLine($"[{DateTime.Now:G}] {message}");
            Out.Flush();
        }
    }
}
