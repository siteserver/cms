using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Repositories;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlEntity
{
    [StlElement(Title = "用户实体", Description = "通过 {user.} 实体在模板中显示用户值")]
    public static class StlUserEntities
    {
        public const string EntityName = "user";

        internal static string Parse(string stlEntity, ParseContext context, IUserRepository userRepository)
        {
            var parsedContent = string.Empty;
            var userInfo = context.PageInfo.UserInfo;
            if (userInfo == null) return string.Empty;

            try
            {
                var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
                var attributeName = entityName.Substring(6, entityName.Length - 7);

                if (StringUtils.EqualsIgnoreCase(UserAttribute.Id, attributeName))
                {
                    parsedContent = userInfo.Id.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.UserName, attributeName))
                {
                    parsedContent = userInfo.UserName;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.CreationDate, attributeName))
                {
                    parsedContent = DateUtils.Format(userInfo.CreationDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.LastActivityDate, attributeName))
                {
                    parsedContent = DateUtils.Format(userInfo.LastActivityDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.CountOfLogin, attributeName))
                {
                    parsedContent = userInfo.CountOfLogin.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.DisplayName, attributeName))
                {
                    parsedContent = userInfo.DisplayName;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Email, attributeName))
                {
                    parsedContent = userInfo.Email;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Mobile, attributeName))
                {
                    parsedContent = userInfo.Mobile;
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.AvatarUrl, attributeName))
                {
                    parsedContent = context.UrlManager.GetUserAvatarUrl(userInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(UserAttribute.Bio, attributeName))
                {
                    parsedContent = userInfo.Bio;
                }
                else
                {
                    parsedContent = userInfo.Get<string>(attributeName);
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
