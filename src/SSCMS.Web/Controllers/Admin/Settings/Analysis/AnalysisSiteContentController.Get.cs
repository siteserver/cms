using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    public partial class AnalysisSiteContentController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromBody] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAnalysisSiteContent))
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
            var adminAddStats = await _statRepository.GetStatsWithAdminIdAsync(lowerDate,
                higherDate, StatType.ContentAdd, request.SiteId);
            var adminEditStats = await _statRepository.GetStatsWithAdminIdAsync(lowerDate,
                higherDate, StatType.ContentEdit, request.SiteId);

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

            var adminStats = new List<GetAdminStat>();
            foreach (var adminAddStat in adminAddStats)
            {
                var stat = adminStats.Find(x => x.AdminId == adminAddStat.AdminId);
                if (stat == null)
                {
                    var admin =
                        await _administratorRepository.GetByUserIdAsync(adminAddStat.AdminId);
                    if (admin == null) continue;

                    stat = new GetAdminStat
                    {
                        AdminId = adminAddStat.AdminId,
                        AdminName = admin.UserName,
                        Add = 0,
                        Edit = 0
                    };
                    adminStats.Add(stat);
                }

                stat.Add += 1;
            }
            foreach (var adminEditStat in adminEditStats)
            {
                var stat = adminStats.Find(x => x.AdminId == adminEditStat.AdminId);
                if (stat == null)
                {
                    var admin =
                        await _administratorRepository.GetByUserIdAsync(adminEditStat.AdminId);
                    if (admin == null) continue;

                    stat = new GetAdminStat
                    {
                        AdminId = adminEditStat.AdminId,
                        AdminName = admin.UserName,
                        Add = 0,
                        Edit = 0
                    };
                    adminStats.Add(stat);
                }

                stat.Edit += 1;
            }

            return new GetResult
            {
                Sites = sites,
                Days = days,
                AddCount = addCount,
                EditCount = editCount,
                AdminStats = adminStats.OrderByDescending(x => x.Add + x.Edit)
            };
        }
    }
}