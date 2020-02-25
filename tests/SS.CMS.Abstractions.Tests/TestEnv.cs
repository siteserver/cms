using System;

namespace SS.CMS.Abstractions.Tests
{
    public static class TestEnv
    {
        public static bool IsTestMachine => Environment.MachineName == "DESKTOP-7S7SBTS";
    }
}
