using System;

namespace SiteServer.Abstractions.Tests
{
    public static class TestEnv
    {
        public static bool IsTestMachine => Environment.MachineName == "DESKTOP-7S7SBTS";
    }
}
