using System.Collections.Generic;

namespace SSCMS.Cli.Models
{
    public class Theme
    {
        public string Name { get; set; }

        public string CoverUrl { get; set; }

        public string Summary { get; set; }

        public List<string> Tags { get; set; }

        public List<string> ThumbUrls { get; set; }

        public List<string> Compatibilities { get; set; }

        public decimal Price { get; set; }
    }
}
