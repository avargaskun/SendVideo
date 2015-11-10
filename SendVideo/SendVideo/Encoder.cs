using SendVideo.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace SendVideo
{
    public class Encoder
    {
        private readonly string handbrakePath;

        private readonly string handbrakeArguments;

        public Encoder(HandbrakeConfiguration configuration)
        {
            if (!TryFindFile(configuration.Path, out this.handbrakePath))
            {
                throw new ApplicationException($"Could not find handbrake at {configuration.Path}");
            }

            this.handbrakeArguments = configuration.Arguments ?? string.Empty;
            Log.Debug($"Using handbrake at: {this.handbrakePath}");
        }

        public async Task<FileInfo> Encode(string input)
        {
            string outputFile = Path.GetTempFileName();

            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = this.handbrakePath,
                    Arguments = $"-i \"{input}\" -o \"{outputFile}\" {this.handbrakeArguments}",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                await RunProcessAsync(process);
            }

            return new FileInfo(outputFile);
        }

        public static Task RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<bool>();

            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) =>
            {
                if (process.ExitCode == 0)
                {
                    tcs.TrySetResult(false);
                }
                else
                {
                    Log.Debug($"{process.StartInfo.FileName} {process.StartInfo.Arguments}\n{process.StandardOutput.ReadToEnd()}");
                    tcs.TrySetException(new ApplicationException($"Handbrake process exited with error code: {process.ExitCode}"));
                }
            };

            process.Start();
            return tcs.Task;
        }

        private static bool TryFindFile(string path, out string found)
        {
            found = path;

            if (!File.Exists(found) && !Path.IsPathRooted(path))
            {
                var codebase = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                found = Path.Combine(codebase, path);
            }

            if (!File.Exists(found))
            {
                return false;
            }

            found = Path.GetFullPath(found);
            return true;
        }
    }
}
