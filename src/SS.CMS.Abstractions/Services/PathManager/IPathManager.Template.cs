using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPathManager
    {
        Task<string> GetTemplateFilePathAsync(Site site, Template template);

        Task WriteContentToTemplateFileAsync(Site site, Template template, string content, int adminId);

        Task<string> GetTemplateContentAsync(Site site, Template template);

        Task<string> GetIncludeContentAsync(Site site, string file);

        Task<string> GetContentByFilePathAsync(string filePath);
    }
}
