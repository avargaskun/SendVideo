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
        public Service()
        {
            this.InitializeComponent();
        }

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
            this.OnStart(args);
            Console.WriteLine("Press a key to exit ...");
            Console.ReadKey();
            this.OnStop();
        }

        protected override void OnStart(string[] args)
        {
            Log.EventLog = this.EventLog;
            Log.Info("SendVideo service starting ...");

            try
            {
                var configuration = ConfigurationManager.GetSection("sendVideo") as SendVideoSection;
                if (configuration == null)
                {
                    throw new ApplicationException("Section 'sendVideo' not found in application configuration file!");
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
                Log.Error($"Could not start service with {e.GetType().Name}: {e.Message}");
                return;
            }
        }

        protected override void OnStop()
        {
            Log.Info("SendVideo service stopping ...");
        }
    }
}
