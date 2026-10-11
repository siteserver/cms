using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using SkiaSharp;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class FontUtils
    {
        // 内嵌字体，按加载顺序保存，第一个为默认字体
        private static readonly List<SKTypeface> Typefaces = new List<SKTypeface>();

        static FontUtils()
        {
            var list = new List<string>
            {
                "Fonts/arial.ttf",
                "Fonts/arialbd.ttf",
                "Fonts/arialbi.ttf",
                "Fonts/ariali.ttf",
                "Fonts/ariblk.ttf",
                "Fonts/simhei.ttf",
            };

            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            foreach (var fileName in list)
            {
                using var reader = embeddedProvider.GetFileInfo(fileName).CreateReadStream();
                var typeface = SKTypeface.FromStream(reader);
                if (typeface != null) Typefaces.Add(typeface);
            }
        }

        public static List<string> GetFontFamilies()
        {
            return Typefaces.Select(x => x.FamilyName).Distinct().ToList();
        }

        public static SKTypeface GetFont(string fontName)
        {
            // 同一字体族有多个字重时，优先取常规字重
            var typeface = Typefaces
                .Where(x => StringUtils.EqualsIgnoreCase(x.FamilyName, fontName))
                .OrderBy(x => x.FontStyle.Weight == (int)SKFontStyleWeight.Normal && x.FontStyle.Slant == SKFontStyleSlant.Upright ? 0 : 1)
                .FirstOrDefault();
            return typeface ?? DefaultFont;
        }

        public static SKTypeface DefaultFont => Typefaces.First();
    }
}
