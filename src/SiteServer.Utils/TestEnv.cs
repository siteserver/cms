using System;

namespace SiteServer.Utils
{
    public static class TestEnv
    {
        private const string TestMachine = "DESKTOP-7S7SBTS";

        public static bool IntegrationTestMachine => Environment.MachineName == TestMachine;
    }
}
