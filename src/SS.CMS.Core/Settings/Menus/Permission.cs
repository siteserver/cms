using System;
using System.Collections.Generic;
using System.Text;

namespace SS.CMS.Core.Settings.Menus
{
    public class Permission
    {
        public Permission(string name, string text)
        {
            Name = name;
            Text = text;
        }

        public string Name { get; }

        public string Text { get; }
    }
}
