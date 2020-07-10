using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SSCMS.Plugins
{
    public interface IPluginMiddleware : IPluginExtension
    {
        Task UseAsync(RequestDelegate next, HttpContext context);
    }
}
