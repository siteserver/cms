using System.Threading.Tasks;
using SiteServer.CMS.Context;
using SiteServer.Utils;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
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

	    internal static async Task<string> ParseAsync(string stlEntity, PageInfo pageInfo)
	    {
	        var parsedContent = string.Empty;
	        if (pageInfo?.User == null) return string.Empty;

	        try
	        {
	            var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
	            var attributeName = entityName.Substring(6, entityName.Length - 7);

                if (StringUtils.EqualsIgnoreCase(nameof(User.Id), attributeName))
                {
                    parsedContent = pageInfo.User.Id.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.UserName), attributeName))
                {
                    parsedContent = pageInfo.User.UserName;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.CreateDate), attributeName))
                {
                    parsedContent = DateUtils.Format(pageInfo.User.CreateDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.LastActivityDate), attributeName))
                {
                    parsedContent = DateUtils.Format(pageInfo.User.LastActivityDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.CountOfLogin), attributeName))
                {
                    parsedContent = pageInfo.User.CountOfLogin.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.DisplayName), attributeName))
                {
                    parsedContent = pageInfo.User.DisplayName;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.Email), attributeName))
                {
                    parsedContent = pageInfo.User.Email;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.Mobile), attributeName))
                {
                    parsedContent = pageInfo.User.Mobile;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.AvatarUrl), attributeName))
                {
                    parsedContent = UserManager.GetUserAvatarUrl(pageInfo.User);
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.Gender), attributeName))
                {
                    parsedContent = pageInfo.User.Gender;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.Birthday), attributeName))
                {
                    parsedContent = pageInfo.User.Birthday;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.WeiXin), attributeName))
                {
                    parsedContent = pageInfo.User.WeiXin;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.Qq), attributeName))
                {
                    parsedContent = pageInfo.User.Qq;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.WeiBo), attributeName))
                {
                    parsedContent = pageInfo.User.WeiBo;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.Bio), attributeName))
                {
                    parsedContent = pageInfo.User.Bio;
                }
                else
                {
                    parsedContent = pageInfo.User.Get<string>(attributeName);
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
