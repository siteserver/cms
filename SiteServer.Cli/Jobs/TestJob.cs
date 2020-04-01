using System;
using System.Reflection;
using System.Threading.Tasks;
using NDesk.Options;
using SiteServer.Cli.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Jobs
{
    public class TestJob
    {
        public const string CommandName = "test add";

        private bool _isHelp;
        private string _webConfigFileName;

        private readonly OptionSet _options;

        public TestJob()
        {
            _options = new OptionSet
            {
                {
                    "c|config=", "the {web.config} file name.",
                    v => _webConfigFileName = v
                },
                {
                    "h|help", "命令说明",
                    v => _isHelp = v != null
                }
            };
        }

        public async Task Execute(IJobContext context)
        {
            if (!CliUtils.ParseArgs(_options, context.Args)) return;

            if (_isHelp)
            {
                return;
            }

            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            await Console.Out.WriteLineAsync($"SiteServer CLI Version: {version.Substring(0, version.Length - 2)}");
            await Console.Out.WriteLineAsync($"Work Directory: {CliUtils.PhysicalApplicationPath}");
            await Console.Out.WriteLineAsync($"siteserver.exe Path: {Assembly.GetExecutingAssembly().Location}");
            await Console.Out.WriteLineAsync();
        }
    }
}
