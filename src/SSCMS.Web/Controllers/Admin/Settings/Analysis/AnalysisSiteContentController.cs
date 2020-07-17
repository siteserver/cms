using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AnalysisSiteContentController : ControllerBase
    {
        private const string Route = "settings/analysisSiteContent";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IStatRepository _statRepository;

        public AnalysisSiteContentController(IAuthManager authManager, ISiteRepository siteRepository, IStatRepository statRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _statRepository = statRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromBody] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAnalysisSiteContent))
            {
                return Unauthorized();
            }

            var lowerDate = TranslateUtils.ToDateTime(request.DateFrom);
            var higherDate = TranslateUtils.ToDateTime(request.DateTo, DateTime.Now);

            var sites = await _siteRepository.GetCascadeChildrenAsync(0);
            sites.Insert(0, new Cascade<int>
            {
                Value = 0,
                Label = "<所有站点>"
            });

            var addStats = await _statRepository.GetStatsAsync(lowerDate, higherDate, StatType.ContentAdd, request.SiteId);
            var editStats = await _statRepository.GetStatsAsync(lowerDate, higherDate, StatType.ContentEdit, request.SiteId);

            var getStats = new List<GetStat>();
            var totalDays = (higherDate - lowerDate).TotalDays;
            for (var i = 0; i <= totalDays; i++)
            {
                var date = lowerDate.AddDays(i).ToString("M-d");

                var add = addStats.FirstOrDefault(x => x.CreatedDate.HasValue && x.CreatedDate.Value.ToString("M-d") == date);
                var edit = editStats.FirstOrDefault(x => x.CreatedDate.HasValue && x.CreatedDate.Value.ToString("M-d") == date);

                getStats.Add(new GetStat
                {
                    Date = date,
                    Add = add?.Count ?? 0,
                    Edit = edit?.Count ?? 0
                });
            }

            var days = getStats.Select(x => x.Date).ToList();
            var addCount = getStats.Select(x => x.Add).ToList();
            var editCount = getStats.Select(x => x.Edit).ToList();

            return new GetResult
            {
                Sites = sites,
                Days = days,
                AddCount = addCount,
                EditCount = editCount
            };
        }
    }
}
