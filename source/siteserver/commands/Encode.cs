using System;
using BaiRong.Core;

namespace siteserver.commands
{
    internal class Encode
    {
        public const string CommandName = "encode";

        public static void Start(string val)
        {
            Console.WriteLine("Encoded String: {0}", TranslateUtils.EncryptStringBySecretKey(val));
        }
    }
}
