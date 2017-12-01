using System;
using BaiRong.Core;
using CommandLine;

namespace siteserver.commands
{
    internal class Plugin
    {
        public const string CommandName = "plugin";

        public static void Start(string val)
        {
            Console.WriteLine("加密字符串: {0}", TranslateUtils.EncryptStringBySecretKey(val));
        }
    }
}
