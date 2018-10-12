using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [StlElement(Title = "用户实体", Description = "通过 {user.} 实体在模板中显示用户值")]
    public class StlUserEntities
	{
        private StlUserEntities()
		{
		}

        public const string EntityName = "user";

	    internal static string Parse(string stlEntity, PageInfo pageInfo)
	    {
	        var parsedContent = string.Empty;
	        if (pageInfo?.UserInfo == null) return string.Empty;

	        try
	        {
	            var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
	            var attributeName = entityName.Substring(6, entityName.Length - 7);

                if (StringUtils.EqualsIgnoreCase(UserAttribute.Id, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Id.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.UserName, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.UserName;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.CreateDate, attributeName))
                {
                    parsedContent = DateUtils.Format(pageInfo.UserInfo.CreateDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.LastActivityDate, attributeName))
                {
                    parsedContent = DateUtils.Format(pageInfo.UserInfo.LastActivityDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.CountOfLogin, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.CountOfLogin.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.DisplayName, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.DisplayName;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Email, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Email;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Mobile, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Mobile;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.AvatarUrl, attributeName))
                {
                    parsedContent = UserManager.GetUserAvatarUrl(pageInfo.UserInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Gender, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Gender;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Birthday, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Birthday;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.WeiXin, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.WeiXin;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Qq, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Qq;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.WeiBo, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.WeiBo;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Bio, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Bio;
                }
                else
                {
                    parsedContent = pageInfo.UserInfo.GetString(attributeName);
                }
            }
	        catch
	        {
	            // ignored
	        }

	        return parsedContent;
	    }
	}
}
