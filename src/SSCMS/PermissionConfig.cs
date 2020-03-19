using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMS
{
    public class PermissionConfig
    {
        public PermissionConfig(string name, string text)
        {
            Name = name;
            Text = text;
        }

        public string Name { get; }

        public string Text { get; }
    }
}
