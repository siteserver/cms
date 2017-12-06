using System;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.API.Model;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsEditController : ApiController
    {
        [HttpPost, Route(ActionsEdit.Route)]
        public IHttpActionResult Main()
        {
            var body = new RequestBody();

            if (!body.IsUserLoggin) return Unauthorized();

            try
            {
                var userInfo = body.UserInfo;
                if (userInfo == null)
                {
                    return NotFound();
                }

                if (body.GetPostString("avatarUrl") != null)
                {
                    userInfo.AvatarUrl = body.GetPostString("avatarUrl");
                }
                if (body.GetPostString("displayName") != null)
                {
                    userInfo.DisplayName = body.GetPostString("displayName");
                }
                if (body.GetPostString("gender") != null)
                {
                    userInfo.Gender = body.GetPostString("gender");
                }
                if (body.GetPostString("birthday") != null)
                {
                    userInfo.Birthday = body.GetPostString("birthday");
                }
                if (body.GetPostString("signature") != null)
                {
                    userInfo.Signature = body.GetPostString("signature");
                }
                if (body.GetPostString("organization") != null)
                {
                    userInfo.Organization = body.GetPostString("organization");
                }
                if (body.GetPostString("department") != null)
                {
                    userInfo.Department = body.GetPostString("department");
                }
                if (body.GetPostString("position") != null)
                {
                    userInfo.Position = body.GetPostString("position");
                }
                if (body.GetPostString("education") != null)
                {
                    userInfo.Education = body.GetPostString("education");
                }
                if (body.GetPostString("graduation") != null)
                {
                    userInfo.Graduation = body.GetPostString("graduation");
                }
                if (body.GetPostString("address") != null)
                {
                    userInfo.Address = body.GetPostString("address");
                }
                if (body.GetPostString("interests") != null)
                {
                    userInfo.Interests = body.GetPostString("interests");
                }
                if (body.GetPostString("mobile") != null)
                {
                    var mobile = body.GetPostString("mobile");
                    if (mobile != userInfo.Mobile)
                    {
                        var exists = BaiRongDataProvider.UserDao.IsMobileExists(mobile);
                        if (!exists)
                        {
                            LogUtils.AddUserLog(body.UserName, EUserActionType.UpdateMobile, mobile);
                            userInfo.Mobile = mobile;
                        }
                        else
                        {
                            return BadRequest("此手机号码已注册，请更换手机号码");
                        }
                    }
                }
                if (body.GetPostString("email") != null)
                {
                    var email = body.GetPostString("email");
                    if (email != userInfo.Email)
                    {
                        var exists = BaiRongDataProvider.UserDao.IsEmailExists(email);
                        if (!exists)
                        {
                            LogUtils.AddUserLog(body.UserName, EUserActionType.UpdateEmail, email);
                            userInfo.Email = email;
                        }
                        else
                        {
                            return BadRequest("此邮箱已注册，请更换邮箱");
                        }
                    }
                }

                BaiRongDataProvider.UserDao.Update(userInfo);
                return Ok(new User(userInfo));
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
