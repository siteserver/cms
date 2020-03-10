using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.Fonts;
using SS.CMS.Abstractions;

namespace SS.CMS.Core
{
    public static class FontManager
    {
        public static List<string> GetFontFamilies()
        {
            return SystemFonts.Families.Select(x => x.Name).ToList();
        }

        private static FontFamily _defaultFamily;

        public static FontFamily DefaultFont
        {
            get
            {
                return _defaultFamily ??= SystemFonts.Families.FirstOrDefault(x =>
                                              StringUtils.EqualsIgnoreCase(x.Name, "Arial") ||
                                              StringUtils.EqualsIgnoreCase(x.Name, "Helvetica") ||
                                              StringUtils.EqualsIgnoreCase(x.Name, "Sans-Serif")) ??
                                          SystemFonts.Families.First();
            }
        }
    }
}
