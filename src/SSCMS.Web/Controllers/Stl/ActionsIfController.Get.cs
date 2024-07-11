using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Models;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Stl
{
    public partial class ActionsIfController
    {
        [HttpPost, Route(Constants.RouteStlRouteActionsIf)]
        public async Task<GetResult> Get([FromBody] GetRequest request)
        {
            var user = await _authManager.GetUserAsync();

            var dynamicInfo = StlDynamic.GetDynamicInfo(_settingsManager, request.Value, request.Page, user, Request.Path + Request.QueryString);
            var ifInfo = TranslateUtils.JsonDeserialize<DynamicIfInfo>(dynamicInfo.Settings);

            var isSuccess = false;
            var html = string.Empty;

            if (ifInfo != null)
            {
                if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserLoggin))
                {
                    isSuccess = _authManager.IsUser;
                }
                else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeUserName))
                {
                    if (user != null)
                    {
                        isSuccess = StlIf.TestTypeValue(ifInfo.Op, ifInfo.Value, user.UserName);
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeUserGroup))
                {
                    // if (user != null)
                    // {
                    //     var groups = await _userGroupRepository.GetUserGroupsAsync();
                    //     var group = groups.FirstOrDefault(g => g.Id == user.GroupId);
                    //     isSuccess = StlIf.TestTypeValue(ifInfo.Op, ifInfo.Value, group != null ? group.GroupName : string.Empty);
                    // }
                    if (user != null)
                    {
                        var groups = await _usersInGroupsRepository.GetGroupsAsync(user);
                        var groupNames = groups.Select(x => x.GroupName).ToList();
                        if (string.IsNullOrEmpty(ifInfo.Op))
                        {
                            ifInfo.Op = StlIf.OperateEquals;
                        }
                        if (StringUtils.EqualsIgnoreCase(ifInfo.Op, StlIf.OperateEquals) || StringUtils.EqualsIgnoreCase(ifInfo.Op, StlIf.OperateIn) || StringUtils.EqualsIgnoreCase(ifInfo.Op, StlIf.OperateContains))
                        {
                            isSuccess = ListUtils.ContainsIgnoreCase(groupNames, ifInfo.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(ifInfo.Op, StlIf.OperateNotEquals) || StringUtils.EqualsIgnoreCase(ifInfo.Op, StlIf.OperateNotIn) || StringUtils.EqualsIgnoreCase(ifInfo.Op, StlIf.OperateContains))
                        {
                            isSuccess = !ListUtils.ContainsIgnoreCase(groupNames, ifInfo.Value);
                        }
                        // isSuccess = StlIf.TestTypeValue(ifInfo.Op, ifInfo.Value, ListUtils.ToString(groupNames));
                        
                        // var groups = await _userGroupRepository.GetUserGroupsAsync(true);
                        // var group = groups.FirstOrDefault(g => g.Id == user.GroupId);
                        // isSuccess = StlIf.TestTypeValue(ifInfo.Op, ifInfo.Value, group != null ? group.GroupName : string.Empty);
                        // if (!isSuccess)
                        // {
                        //     group = groups.FirstOrDefault(g => g.Id == user.GroupId2);
                        //     isSuccess = StlIf.TestTypeValue(ifInfo.Op, ifInfo.Value, group != null ? group.GroupName : string.Empty);
                        // }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsAdministratorLoggin))
                {
                    isSuccess = _authManager.IsAdmin;
                }
                else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserOrAdministratorLoggin))
                {
                    isSuccess = _authManager.IsUser || _authManager.IsAdmin;
                }

                var template = isSuccess ? dynamicInfo.YesTemplate : dynamicInfo.NoTemplate;
                html = await StlDynamic.ParseDynamicAsync(_parseManager, dynamicInfo, template);
            }

            return new GetResult
            {
                Value = isSuccess,
                Html = html
            };
        }
    }
}
