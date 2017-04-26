using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Xml;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Parser;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    public class StlAction
    {
        private StlAction() { }
        public const string ElementName = "stl:action";     //执行动作

        public const string AttributeType = "type";                        //动作类型
        public const string AttributeReturnUrl = "returnurl";              //动作完成后的返回地址
        public const string AttributeIsDynamic = "isdynamic";              //是否动态显示

        public const string TypeLogin = "Login";			            //登录
        public const string TypeRegister = "Register";			        //注册
        public const string TypeLogout = "Logout";			            //退出
        public const string TypeAddFavorite = "AddFavorite";			//将页面添加至收藏夹
        public const string TypeSetHomePage = "SetHomePage";			//将页面设置为首页
        public const string TypeTranslate = "Translate";			    //繁体/简体转换
        public const string TypeClose = "Close";			            //关闭页面
        public const string TypeComments = "Comments";			        //链接到评论页面

        public static ListDictionary AttributeList => new ListDictionary
        {
            {AttributeType, "动作类型"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            string parsedContent;
            var contextInfo = contextInfoRef.Clone();

            try
            {
                var attributes = new NameValueCollection();
                var ie = node.Attributes?.GetEnumerator();
                var type = string.Empty;
                var returnUrl = string.Empty;
                var isDynamic = false;

                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;
                        var attributeName = attr.Name.ToLower();

                        if (attributeName.Equals(AttributeType))
                        {
                            type = attr.Value;
                        }
                        else if (attributeName.Equals(AttributeReturnUrl))
                        {
                            returnUrl = StlEntityParser.ReplaceStlEntitiesForAttributeValue(attr.Value, pageInfo, contextInfo);
                        }
                        else if (attributeName.Equals(AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value, false);
                        }
                        else
                        {
                            attributes.Add(attributeName, attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(pageInfo, contextInfo, node, attributes, type, returnUrl);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(PageInfo pageInfo, ContextInfo contextInfo, XmlNode node, NameValueCollection attributes, string type, string returnUrl)
        {
            var stlAnchor = new HtmlAnchor();

            foreach (string attributeName in attributes.Keys)
            {
                stlAnchor.Attributes.Add(attributeName, attributes[attributeName]);
            }

            var url = PageUtils.UnclickedUrl;
            var onclick = string.Empty;

            var innerBuilder = new StringBuilder(node.InnerXml);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);
            stlAnchor.InnerHtml = innerBuilder.ToString();

            //计算动作开始
            if (!string.IsNullOrEmpty(type))
            {
                if (StringUtils.EqualsIgnoreCase(type, TypeLogin))
                {
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        returnUrl = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelID, contextInfo.ContentID, contextInfo.ContentInfo);
                    }

                    url = HomeUtils.GetLoginUrl(pageInfo.HomeUrl, returnUrl);
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeRegister))
                {
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        returnUrl = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelID, contextInfo.ContentID, contextInfo.ContentInfo);
                    }

                    url = HomeUtils.GetRegisterUrl(pageInfo.HomeUrl, returnUrl);
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeLogout))
                {
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        returnUrl = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelID, contextInfo.ContentID, contextInfo.ContentInfo);
                    }

                    url = HomeUtils.GetLogoutUrl(pageInfo.HomeUrl, returnUrl);
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeAddFavorite))
                {
                    pageInfo.SetPageScripts(TypeAddFavorite, @"
<script type=""text/javascript""> 
    function AddFavorite(){  
        if (document.all) {
            window.external.addFavorite(window.location.href, document.title);
        } 
        else if (window.sidebar) {
            window.sidebar.addPanel(document.title, window.location.href, """");
        }
    }
</script>
", true);
                    stlAnchor.Attributes["onclick"] = "AddFavorite();";
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeSetHomePage))
                {
                    url = pageInfo.PublishmentSystemInfo.PublishmentSystemUrl;
                    pageInfo.AddPageEndScriptsIfNotExists(TypeAddFavorite, $@"
<script type=""text/javascript""> 
    function SetHomepage(){{   
        if (document.all) {{
            document.body.style.behavior = 'url(#default#homepage)';
            document.body.setHomePage(""{url}"");
        }}
        else if (window.sidebar) {{
            if (window.netscape) {{
                try {{
                    netscape.security.PrivilegeManager.enablePrivilege(""UniversalXPConnect"");
                 }}
                catch(e) {{
                    alert(""该操作被浏览器拒绝，如果想启用该功能，请在地址栏内输入 about:config,然后将项 signed.applets.codebase_principal_support 值该为true"");
                }}
             }}
            var prefs = Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefBranch);
            prefs.setCharPref('browser.startup.homepage', ""{url}"");
        }}
    }}
</script>
");
                    stlAnchor.Attributes["onclick"] = "SetHomepage();";
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeTranslate))
                {
                    pageInfo.AddPageScriptsIfNotExists(PageInfo.JsAhTranslate);

                    var msgToTraditionalChinese = "繁體";
                    var msgToSimplifiedChinese = "简体";
                    if (!string.IsNullOrEmpty(stlAnchor.InnerHtml))
                    {
                        if (stlAnchor.InnerHtml.IndexOf(",", StringComparison.Ordinal) != -1)
                        {
                            msgToTraditionalChinese = stlAnchor.InnerHtml.Substring(0, stlAnchor.InnerHtml.IndexOf(",", StringComparison.Ordinal));
                            msgToSimplifiedChinese = stlAnchor.InnerHtml.Substring(stlAnchor.InnerHtml.IndexOf(",", StringComparison.Ordinal) + 1);
                        }
                        else
                        {
                            msgToTraditionalChinese = stlAnchor.InnerHtml;
                        }
                    }
                    stlAnchor.InnerHtml = msgToTraditionalChinese;

                    if (string.IsNullOrEmpty(stlAnchor.ID))
                    {
                        stlAnchor.ID = "translateLink";
                    }

                    pageInfo.SetPageEndScripts(TypeTranslate, $@"
<script type=""text/javascript""> 
var defaultEncoding = 0;
var translateDelay = 0;
var cookieDomain = ""/"";
var msgToTraditionalChinese = ""{msgToTraditionalChinese}"";
var msgToSimplifiedChinese = ""{msgToSimplifiedChinese}"";
var translateButtonId = ""{stlAnchor.ClientID}"";
translateInitilization();
</script>
");
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeClose))
                {
                    url = "javascript:window.close()";
                }
                else if (StringUtils.EqualsIgnoreCase(type, TypeComments))
                {
                    url = stlAnchor.Attributes["href"];
                    if (string.IsNullOrEmpty(url))
                    {
                        url = PageUtility.ParseNavigationUrl(pageInfo.PublishmentSystemInfo, "@/utils/comments.html");
                    }
                    if (contextInfo.ContextType == EContextType.Content)
                    {
                        var parameters = new NameValueCollection
                        {
                            {"channelID", contextInfo.ChannelID.ToString()},
                            {"contentID", contextInfo.ContentID.ToString()}
                        };
                        url = PageUtils.AddQueryString(url, parameters);
                    }
                    else if (contextInfo.ContextType == EContextType.Channel)
                    {
                        url = PageUtils.AddQueryString(url, "channelID", contextInfo.ChannelID.ToString());
                    }
                }
            }
            //计算动作结束

            stlAnchor.HRef = url;

            if (!string.IsNullOrEmpty(onclick))
            {
                stlAnchor.Attributes.Add("onclick", onclick);
            }

            return ControlUtils.GetControlRenderHtml(stlAnchor);
        }
    }
}
