using System;

namespace SS.CMS.Cli.Commands
{

    public sealed class VersionCommand
    {
        private static readonly Lazy<VersionCommand> Lazy = new Lazy<VersionCommand>(() => new VersionCommand());

        public static VersionCommand Instance => Lazy.Value;

        private VersionCommand()
        {
        }
    }
}
