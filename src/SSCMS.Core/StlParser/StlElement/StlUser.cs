using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "显示用户信息", Description = "通过 stl:user 标签在模板中显示用户信息")]
    public static class StlUser
    {
        public const string ElementName = "stl:user";

        [StlAttribute(Title = "用户属性名称")]
        private const string Type = nameof(Type);

        public static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var type = string.Empty;
            
            var attributes = new NameValueCollection();

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type))
                {
                    type = value;
                }
                else
                {
                    attributes[name] = value;
                }
            }

            return await ParseAsync(parseManager, type, attributes);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string type, NameValueCollection attributes)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var parsedContent = string.Empty;
            if (pageInfo?.User == null) return string.Empty;

            if (!string.IsNullOrEmpty(contextInfo.InnerHtml))
            {
                var innerBuilder = new StringBuilder(contextInfo.InnerHtml);
                await parseManager.ParseInnerContentAsync(innerBuilder);
            }

            try
            {
                if (StringUtils.EqualsIgnoreCase(nameof(User.Id), type))
                {
                    parsedContent = pageInfo.User.Id.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.UserName), type))
                {
                    parsedContent = pageInfo.User.UserName;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.CreatedDate), type))
                {
                    parsedContent = DateUtils.Format(pageInfo.User.CreatedDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.LastActivityDate), type))
                {
                    parsedContent = DateUtils.Format(pageInfo.User.LastActivityDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.CountOfLogin), type))
                {
                    parsedContent = pageInfo.User.CountOfLogin.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.DisplayName), type))
                {
                    parsedContent = pageInfo.User.DisplayName;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.Email), type))
                {
                    parsedContent = pageInfo.User.Email;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.Mobile), type))
                {
                    parsedContent = pageInfo.User.Mobile;
                }
                else if (StringUtils.EqualsIgnoreCase(nameof(User.AvatarUrl), type))
                {
                    parsedContent = parseManager.PathManager.GetUserAvatarUrl(pageInfo.User);
                    if (!contextInfo.IsStlEntity)
                    {
                        attributes["src"] = parsedContent;
                        parsedContent = $@"<img {TranslateUtils.ToAttributesString(attributes)}>";
                    }
                }
                else
                {
                    parsedContent = pageInfo.User.Get<string>(type);
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
