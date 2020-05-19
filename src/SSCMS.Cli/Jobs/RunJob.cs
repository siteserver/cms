using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Mono.Options;
using SSCMS.Cli.Abstractions;
using SSCMS.Cli.Core;

namespace SSCMS.Cli.Jobs
{
    public class RunJob : IJobService
    {
        public string CommandName => "run";

        private bool _isHelp;

        private readonly OptionSet _options;

        public RunJob()
        {
            _options = new OptionSet
            {
                {
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public void PrintUsage()
        {
            Console.WriteLine($"Usage: sscms-cli {CommandName}");
            Console.WriteLine("Summary: run sscms");
            Console.WriteLine("Options:");
            _options.WriteOptionDescriptions(Console.Out);
            Console.WriteLine();
        }

        public async Task ExecuteAsync(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                PrintUsage();
                return;
            }

            var psi = new ProcessStartInfo("./sscms") { RedirectStandardOutput = true };
            var proc = Process.Start(psi);
            if (proc == null)
            {
                await WriteUtils.PrintErrorAsync("Can not run sscms.");
            }
            else
            {
                Console.WriteLine("Starting SS CMS...");
                Thread.Sleep(3000);

                OpenUrl("http://localhost:5000");

                using var sr = proc.StandardOutput;
                while (!sr.EndOfStream)
                {
                    Console.WriteLine(sr.ReadLine());
                }

                if (!proc.HasExited)
                {
                    proc.Kill();
                }
            }
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}