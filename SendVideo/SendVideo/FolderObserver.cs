using SendVideo.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SendVideo
{
    public class FolderObserver
    {
        private const int HRESULT_ERROR_SHARING_VIOLATION = -2147024864;

        private static readonly TimeSpan MaxFileCreationWait = TimeSpan.FromMinutes(1);

        private readonly ObservedFolder folder;

        private readonly Encoder encoder;

        private readonly MailSender sender;

        private readonly FileSystemWatcher watcher;

        private readonly List<Recipient> recipients;

        public FolderObserver(ObservedFolder folder, Encoder encoder, MailSender sender, IEnumerable<Recipient> allRecipients)
        {
            this.folder = folder;
            this.encoder = encoder;
            this.sender = sender;
            var names = folder.Recipients.Split(',');
            this.recipients = allRecipients.Where(r => names.Contains(r.Name)).ToList();
            this.watcher = new FileSystemWatcher(this.folder.Path);
            this.watcher.Created += OnFileCreated;
            this.watcher.EnableRaisingEvents = true;
        }

        private void OnFileCreated(object sender, FileSystemEventArgs args)
        {
            if (File.Exists(args.FullPath))
            {
                foreach (var recipient in this.recipients)
                {
                    this.SendVideoToRecipient(args.FullPath, recipient);
                }
            }
        }

        private async void SendVideoToRecipient(string input, Recipient recipient)
        {
            FileInfo video;

            try
            {
                // Acquiring a lock to ensure that the process generating the video is done with the file.
                using (await LockFileAsync(input))
                {
                }

                // Release the lock before running the encoder otherwise it won't be able to open the file.
                video = await encoder.Encode(input);
            }
            catch (Exception e)
            {
                Log.Warning($"Failed to process file: {input} with {e.GetType().Name}: {e.Message}");
                return;
            }

            try
            {
                await this.sender.SendVideo(video, recipient, this.folder.Message);
            }
            catch (Exception e)
            {
                Log.Warning($"Failed to send video to {recipient.Name} with {e.GetType().Name}: {e.Message}");
                return;
            }
            finally
            {
                video.Delete();
            }

            Log.Info($"Video successfully sent to {recipient.Name} ({recipient.Address})");
        }

        private static async Task<FileStream> LockFileAsync(string path)
        {
            var sw = Stopwatch.StartNew();
            do
            {
                try
                {
                    return File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                }
                catch (IOException ioe) when (ioe.HResult == HRESULT_ERROR_SHARING_VIOLATION)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(250));
                }
            } while (sw.Elapsed < MaxFileCreationWait);

            throw new ApplicationException($"Could not lock file {path}.");
        }
    }
}
