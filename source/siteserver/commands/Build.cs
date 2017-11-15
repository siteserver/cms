using System;

namespace siteserver.commands
{
    internal class Build
    {
        public const string CommandName = "build";

        public static void Start(bool isAll)
        {
            Console.WriteLine(isAll);
            Console.WriteLine("build site...");
        }
    }
}
