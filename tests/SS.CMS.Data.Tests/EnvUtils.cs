using System;
using System.Xml;
using SS.CMS.Data.Utils;

namespace SS.CMS.Data.Tests
{
    public static class EnvUtils
    {
        private const string TestMachine = "DESKTOP-7S7SBTS";

        public static bool IntegrationTestMachine => Environment.MachineName == TestMachine;

        public static DbContext DbContext { get; set; }
    }
}