using System;
using CommandLine;
using siteserver.commands;

namespace siteserver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!ServiceUtils.IsSiteServerDir)
            {
                Console.WriteLine("当前文件夹不是正确的SiteServer系统根目录");
                Console.ReadLine();
                return;
            }
            if (!ServiceUtils.IsInitialized)
            {
                Console.WriteLine("SiteServer系统未安装或数据库无法正确连接");
                Console.ReadLine();
                return;
            }

            var invokedVerb = string.Empty;
            object invokedVerbInstance = null;
            if (args.Length == 0)
            {
                invokedVerb = Run.CommandName;
            }
            else
            {
                var options = new Options();
                if (!Parser.Default.ParseArguments(args, options,
                    (verb, subOptions) =>
                    {
                        invokedVerb = verb;
                        invokedVerbInstance = subOptions;
                    }))
                {
                    Console.WriteLine(options.GetUsage());
                    return;
                }
            }

            if (invokedVerb == Build.CommandName)
            {
                var commitSubOptions = (BuildSubOptions)invokedVerbInstance;
                var isAll = commitSubOptions != null && commitSubOptions.All;
                Build.Start(isAll);
            }
            else if (invokedVerb == Test.CommandName)
            {
                Test.Start();
            }
            else if (invokedVerb == Encode.CommandName)
            {
                var subOptions = invokedVerbInstance as EncodeSubOptions;
                if (subOptions != null)
                {
                    Encode.Start(subOptions.String);
                }
            }
            else if (invokedVerb == Run.CommandName)
            {
                Run.Start();
            }
        }
    }
}

