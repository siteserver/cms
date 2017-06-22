using System;
using BaiRong.Core;
using CommandLine;

namespace siteserver.commands
{
    internal class Encode
    {
        public const string CommandName = "encode";

        public static void Start(string val)
        {
            Console.WriteLine("加密字符串: {0}", TranslateUtils.EncryptStringBySecretKey(val));
        }
    }
}
