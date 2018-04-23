using System;
using SiteServer.Utils;

namespace SiteServer.Cli.Core
{
    public class TextLogInfo
    {
        public DateTime DateTime { private get; set; }
        public string Detail { private get; set; }
        public Exception Exception { private get; set; }

        public override string ToString()
        {
            return TranslateUtils.JsonSerialize(new
            {
                DateTime,
                Detail,
                Exception?.Message,
                Exception?.StackTrace
            });
        }
    }
}
