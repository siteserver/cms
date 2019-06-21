using System;

namespace SS.CMS.Utils.Tests
{
    public static class TestEnv
    {
        public static bool IsTestMachine => Environment.MachineName == "DESKTOP-7S7SBTS";
    }
}
