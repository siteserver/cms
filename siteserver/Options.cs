using System;
using System.Collections.Generic;
using System.Text;
using BaiRong.Core;
using CommandLine;
using CommandLine.Text;

namespace siteserver
{
    internal class BuildSubOptions
    {
        [Option('a', "all", HelpText = "Tell the command to automatically stage files.")]
        public bool All { get; set; }
    }

    internal class RunSubOptions
    {
        // Remainder omitted
    }

    internal class TestSubOptions
    {
        [Option('a', "all", HelpText = "Tell the command to automatically stage files.")]
        public bool All { get; set; }
    }

    internal class EncodeSubOptions
    {
        [Option('s', "string", Required = true, HelpText = "Input string to be processed.")]
        public string String { get; set; }
    }

    internal class DecodeSubOptions
    {
        [Option('s', "string", Required = true, HelpText = "Input string to be processed.")]
        public string String { get; set; }
    }

    internal class PluginSubOptions
    {
        [Option('s', "string", Required = true, HelpText = "Command to execute.")]
        public string String { get; set; }
    }

    internal class Options
    {
        [Option]
        public bool Verbose { get; set; } // --verbose

        [Option('q')]
        public bool Quiet { get; set; }   // -q

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("SiteServer 命令行", "V" + AppManager.Version),
                Copyright = new CopyrightInfo("北京百容千域软件技术开发有限责任公司", DateTime.Now.Year),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("用法: siteserver <command> <options>");
            help.AddOptions(this);
            return help;
        }

        [VerbOption("build", HelpText = "生成站点")]
        public BuildSubOptions BuildVerb { get; set; }

        [VerbOption("run", HelpText = "运行任务")]
        public RunSubOptions WatchVerb { get; set; }

        [VerbOption("test", HelpText = "测试")]
        public TestSubOptions TestVerb { get; set; }

        [VerbOption("encode", HelpText = "加密字符串")]
        public EncodeSubOptions EncodeVerb { get; set; }

        [VerbOption("decode", HelpText = "解密字符串")]
        public DecodeSubOptions DecodeVerb { get; set; }
    }
}
