using System.Threading.Tasks;
using SSCMS.Dto;

namespace SSCMS.Services
{
    public interface ISpellManager
    {
        Task<SpellSettings> GetSpellSettingsAsync();

        Task<SpellResult> SpellingCheckAsync(string text);

        Task<(bool success, string errorMessage)> AddSpellWhiteListAsync(string word);
    }
}
