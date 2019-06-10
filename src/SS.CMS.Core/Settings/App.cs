using System;
using System.Collections.Generic;
using System.Text;

namespace SS.CMS.Core.Settings
{
    public interface IApp
    {
        string ApplicationPath { get; }
    }
    public class App : IApp
    {
        public string ApplicationPath { get; }
    }
}
