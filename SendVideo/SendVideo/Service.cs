using SendVideo.Configuration;
using System;
using System.Configuration;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;

namespace SendVideo
{
    public partial class Service : ServiceBase
    {
        public static void Install()
        {
            ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
        }

        public static void Uninstall()
        {
            ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
        }

        public void RunLocal(string[] args)
        {
            Log.Out = Console.Out;
            this.OnStart(args);
            Console.WriteLine("Press a key to exit ...");
            Console.ReadKey();
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            Log.Debug("SendVideo service starting ...");

            try
            {
                var configuration = ConfigurationManager.GetSection("sendVideo") as SendVideoSection;
                if (configuration == null)
                {
                    Log.Debug("Configuration section not found!");
                }

                var encoder = new Encoder(configuration.Handbrake);
                var sender = new MailSender(configuration.SmptServer);

                foreach (var folder in configuration.ObservedFolders.Cast<ObservedFolder>())
                {
                    new FolderObserver(folder, encoder, sender, configuration.Recipients.Cast<Recipient>());
                }
            }
            catch (Exception e)
            {
                Log.Debug($"Could not start service with {e.GetType().Name}: {e.Message}");
                throw;
            }
        }

        protected override void OnStop()
        {
            Log.Debug("SendVideo service stopping ...");
        }
    }
}
