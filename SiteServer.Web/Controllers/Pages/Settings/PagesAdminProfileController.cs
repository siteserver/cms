using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.API.Controllers.Pages.Settings
{
    [RoutePrefix("pages/settings/adminProfile")]
    public class PagesAdminProfileController : ApiController
    {
        private const string Route = "";
        private const string RouteUpload = "upload";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new RequestImpl();
                var userId = request.GetQueryInt("userId");
                if (!request.IsAdminLoggin) return Unauthorized();
                if (request.AdminId != userId &&
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                AdministratorInfo adminInfo;
                if (userId > 0)
                {
                    adminInfo = AdminManager.GetAdminInfoByUserId(userId);
                    if (adminInfo == null) return NotFound();
                }
                else
                {
                    adminInfo = new AdministratorInfo();
                }

                var departments = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(0, "<无所属部门>")
                };
                var departmentIdList = DepartmentManager.GetDepartmentIdList();
                var isLastNodeArrayOfDepartment = new bool[departmentIdList.Count];
                foreach (var departmentId in departmentIdList)
                {
                    var departmentInfo = DepartmentManager.GetDepartmentInfo(departmentId);
                    departments.Add(new KeyValuePair<int, string>(departmentId,
                        GetDepartment(isLastNodeArrayOfDepartment, departmentInfo.DepartmentName,
                            departmentInfo.ParentsCount, departmentInfo.IsLastNode)));
                }

                var areas = new List<KeyValuePair<int, string>>
                {
                    new KeyValuePair<int, string>(0, "<无所在区域>")
                };
                var areaIdList = AreaManager.GetAreaIdList();
                var isLastNodeArrayOfArea = new bool[areaIdList.Count];
                foreach (var areaId in areaIdList)
                {
                    var areaInfo = AreaManager.GetAreaInfo(areaId);
                    areas.Add(new KeyValuePair<int, string>(areaId,
                        GetArea(isLastNodeArrayOfArea, areaInfo.AreaName, areaInfo.ParentsCount, areaInfo.IsLastNode)));
                }

                return Ok(new
                {
                    Value = adminInfo,
                    Departments = departments,
                    Areas = areas,
                    request.AdminToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static string GetDepartment(IList<bool> isLastNodeArrayOfDepartment, string departmentName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                isLastNodeArrayOfDepartment[parentsCount] = false;
            }
            else
            {
                isLastNodeArrayOfDepartment[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, isLastNodeArrayOfDepartment[i] ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, departmentName);
            return str;
        }

        private static string GetArea(IList<bool> isLastNodeArrayOfArea, string areaName, int parentsCount, bool isLastNode)
        {
            var str = "";
            if (isLastNode == false)
            {
                isLastNodeArrayOfArea[parentsCount] = false;
            }
            else
            {
                isLastNodeArrayOfArea[parentsCount] = true;
            }
            for (var i = 0; i < parentsCount; i++)
            {
                str = string.Concat(str, isLastNodeArrayOfArea[i] ? "　" : "│");
            }
            str = string.Concat(str, isLastNode ? "└" : "├");
            str = string.Concat(str, areaName);
            return str;
        }

        [HttpPost, Route(RouteUpload)]
        public IHttpActionResult Upload()
        {
            try
            {
                var request = new RequestImpl();
                var userId = request.GetQueryInt("userId");
                if (!request.IsAdminLoggin) return Unauthorized();
                var adminInfo = AdminManager.GetAdminInfoByUserId(userId);
                if (adminInfo == null) return NotFound();
                if (request.AdminId != userId &&
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                var avatarUrl = string.Empty;

                foreach (string name in HttpContext.Current.Request.Files)
                {
                    var postFile = HttpContext.Current.Request.Files[name];

                    if (postFile == null)
                    {
                        return BadRequest("Could not read image from body");
                    }

                    var fileName = AdminManager.GetUserUploadFileName(postFile.FileName);
                    var filePath = AdminManager.GetUserUploadPath(userId, fileName);

                    if (!EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                    {
                        return BadRequest("image file extension is not correct");
                    }

                    postFile.SaveAs(filePath);

                    avatarUrl = AdminManager.GetUserUploadUrl(userId, fileName);
                }

                return Ok(new
                {
                    Value = avatarUrl
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Submit()
        {
            try
            {
                var request = new RequestImpl();
                var userId = request.GetQueryInt("userId");
                if (!request.IsAdminLoggin) return Unauthorized();
                if (request.AdminId != userId &&
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Admin))
                {
                    return Unauthorized();
                }

                AdministratorInfo adminInfo;
                if (userId > 0)
                {
                    adminInfo = AdminManager.GetAdminInfoByUserId(userId);
                    if (adminInfo == null) return NotFound();
                }
                else
                {
                    adminInfo = new AdministratorInfo();
                }

                var userName = request.GetPostString("userName");
                var password = request.GetPostString("password");
                var displayName = request.GetPostString("displayName");
                var avatarUrl = request.GetPostString("avatarUrl");
                var mobile = request.GetPostString("mobile");
                var email = request.GetPostString("email");
                var departmentId = request.GetPostInt("departmentId");
                var areaId = request.GetPostInt("areaId");

                if (adminInfo.Id == 0)
                {
                    adminInfo.UserName = userName;
                    adminInfo.Password = password;
                    adminInfo.CreatorUserName = request.AdminName;
                    adminInfo.CreationDate = DateTime.Now;
                }
                else
                {
                    if (adminInfo.Mobile != mobile && !string.IsNullOrEmpty(mobile) && DataProvider.AdministratorDao.IsMobileExists(mobile))
                    {
                        return BadRequest("资料修改失败，手机号码已存在");
                    }

                    if (adminInfo.Email != email && !string.IsNullOrEmpty(email) && DataProvider.AdministratorDao.IsEmailExists(email))
                    {
                        return BadRequest("资料修改失败，邮箱地址已存在");
                    }
                }

                adminInfo.DisplayName = displayName;
                adminInfo.AvatarUrl = avatarUrl;
                adminInfo.Mobile = mobile;
                adminInfo.Email = email;
                adminInfo.DepartmentId = departmentId;
                adminInfo.AreaId = areaId;

                if (adminInfo.Id == 0)
                {
                    if (!DataProvider.AdministratorDao.Insert(adminInfo, out var errorMessage))
                    {
                        return BadRequest($"管理员添加失败：{errorMessage}");
                    }
                    request.AddAdminLog("添加管理员", $"管理员:{adminInfo.UserName}");
                }
                else
                {
                    DataProvider.AdministratorDao.Update(adminInfo);
                    request.AddAdminLog("修改管理员属性", $"管理员:{adminInfo.UserName}");
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
