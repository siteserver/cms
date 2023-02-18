using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IFormManager
    {
        Task ImportFormAsync(int siteId, string directoryPath, bool overwrite);

        Task ExportFormAsync(int siteId, string directoryPath, int formId);
    }
}
