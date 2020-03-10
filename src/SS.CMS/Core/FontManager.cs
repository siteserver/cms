using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.Fonts;

namespace SS.CMS.Core
{
    public static class FontManager
    {
        public static List<string> GetFontFamilies()
        {
            return SystemFonts.Families.Select(x => x.Name).ToList();
        }
    }
}
