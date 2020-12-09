using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Home.Common.Editor
{
    public partial class LayerArticleController
    {
        [HttpGet, Route(RouteId)]
        public async Task<ActionResult<MaterialArticle>> Get([FromQuery] int id)
        {
            return await _materialArticleRepository.GetAsync(id);
        }
    }
}
