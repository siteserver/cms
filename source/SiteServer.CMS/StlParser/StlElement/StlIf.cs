using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Xml;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "条件判断", Description = "通过 stl:if 标签在模板中根据条件判断显示内容")]
    public class StlIf
    {
        private StlIf() { }
        public const string ElementName = "stl:if";

        private const string AttributeType = "type";			                                    //测试类型
        private const string AttributeOperate = "operate";				                            //测试操作
        private const string AttributeValue = "value";				                                //测试值
        private const string AttributeContext = "context";                                          //所处上下文
        private const string AttributeIsDynamic = "isDynamic";                                      //是否动态显示

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, "测试类型"},
            {AttributeOperate, "测试操作"},
            {AttributeValue, "测试值"},
            {AttributeContext, "所处上下文"},
            {AttributeIsDynamic, "是否动态显示"}
        };

        public const string TypeIsUserLoggin = "IsUserLoggin";                                      //用户是否已登录
        public const string TypeIsAdministratorLoggin = "IsAdministratorLoggin";                    //管理员是否已登录
        public const string TypeIsUserOrAdministratorLoggin = "IsUserOrAdministratorLoggin";        //用户或管理员是否已登录
        public const string TypeUserGroup = "UserGroup";                                            //用户组
        public const string TypeChannelName = "ChannelName";			                            //栏目名称
        public const string TypeChannelIndex = "ChannelIndex";			                            //栏目索引
        public const string TypeTemplateName = "TemplateName";			                            //模板名称
        public const string TypeTemplateType = "TemplateType";			                            //模板类型
        public const string TypeTopLevel = "TopLevel";			                                    //栏目级别
        public const string TypeUpChannel = "UpChannel";			                                //上级栏目
        public const string TypeUpChannelOrSelf = "UpChannelOrSelf";			                    //当前栏目或上级栏目
        public const string TypeSelfChannel = "SelfChannel";			                            //当前栏目
        public const string TypeGroupChannel = "GroupChannel";			                            //栏目组名称
        public const string TypeGroupContent = "GroupContent";			                            //内容组名称
        public const string TypeAddDate = "AddDate";			                                    //添加时间
        public const string TypeLastEditDate = "LastEditDate";			                            //最后编辑时间（仅用于判断内容）
        public const string TypeItemIndex = "ItemIndex";			                                //当前项序号
        public const string TypeOddItem = "OddItem";			                                    //奇数项

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeIsUserLoggin, "用户是否已登录"},
            {TypeIsAdministratorLoggin, "管理员是否已登录"},
            {TypeIsUserOrAdministratorLoggin, "用户或管理员是否已登录"},
            {TypeUserGroup, "用户组"},
            {TypeChannelName, "栏目名称"},
            {TypeChannelIndex, "栏目索引"},
            {TypeTemplateName, "模板名称"},
            {TypeTemplateType, "模板类型"},
            {TypeTopLevel, "栏目级别"},
            {TypeUpChannel, "上级栏目"},
            {TypeUpChannelOrSelf, "当前栏目或上级栏目"},
            {TypeSelfChannel, "当前栏目"},
            {TypeGroupChannel, "栏目组名称"},
            {TypeGroupContent, "内容组名称"},
            {TypeAddDate, "添加时间"},
            {TypeLastEditDate, "最后编辑时间（仅用于判断内容）"},
            {TypeItemIndex, "当前项序号"},
            {TypeOddItem, "奇数项"}
        };

        public const string OperateEmpty = "Empty";
        public const string OperateNotEmpty = "NotEmpty";			                                //值不为空
        public const string OperateEquals = "Equals";			                                    //值等于
        public const string OperateNotEquals = "NotEquals";			                                //值不等于
        public const string OperateGreatThan = "GreatThan";			                                //值大于
        public const string OperateLessThan = "LessThan";			                                //值小于
        public const string OperateIn = "In";			                                            //值属于
        public const string OperateNotIn = "NotIn";                                                 //值不属于

        public static SortedList<string, string> OperateList => new SortedList<string, string>
        {
            {OperateEmpty, "值为空"},
            {OperateNotEmpty, "值不为空"},
            {OperateEquals, "值等于"},
            {OperateNotEquals, "值不等于"},
            {OperateGreatThan, "值大于"},
            {OperateLessThan, "值小于"},
            {OperateIn, "值属于"},
            {OperateNotIn, "值不属于"}
        };


        internal static string Parse(string stlElement, XmlNode node, PageInfo pageInfo, ContextInfo contextInfoRef)
        {
            string parsedContent;
            var contextInfo = contextInfoRef.Clone();
            try
            {
                var testTypeStr = string.Empty;
                var testOperate = OperateEquals;
                var testValue = string.Empty;
                var isDynamic = false;

                var ie = node.Attributes?.GetEnumerator();
                if (ie != null)
                {
                    while (ie.MoveNext())
                    {
                        var attr = (XmlAttribute)ie.Current;

                        if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeType) || StringUtils.EqualsIgnoreCase(attr.Name, "testType"))
                        {
                            testTypeStr = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeOperate) || StringUtils.EqualsIgnoreCase(attr.Name, "testOperate"))
                        {
                            testOperate = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeValue) || StringUtils.EqualsIgnoreCase(attr.Name, "testValue"))
                        {
                            testValue = attr.Value;
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeContext))
                        {
                            contextInfo.ContextType = EContextTypeUtils.GetEnumType(attr.Value);
                        }
                        else if (StringUtils.EqualsIgnoreCase(attr.Name, AttributeIsDynamic))
                        {
                            isDynamic = TranslateUtils.ToBool(attr.Value);
                        }
                    }
                }

                parsedContent = isDynamic ? StlDynamic.ParseDynamicElement(stlElement, pageInfo, contextInfo) : ParseImpl(node, pageInfo, contextInfo, testTypeStr, testOperate, testValue);
            }
            catch (Exception ex)
            {
                parsedContent = StlParserUtility.GetStlErrorMessage(ElementName, ex);
            }

            return parsedContent;
        }

        private static string ParseImpl(XmlNode node, PageInfo pageInfo, ContextInfo contextInfo, string testType, string testOperate, string testValue)
        {
            string successTemplateString;
            string failureTemplateString;

            StlParserUtility.GetYesOrNoTemplateString(node, pageInfo, out successTemplateString, out failureTemplateString);

            if (StringUtils.EqualsIgnoreCase(testType, TypeIsUserLoggin) ||
                StringUtils.EqualsIgnoreCase(testType, TypeIsAdministratorLoggin) ||
                StringUtils.EqualsIgnoreCase(testType, TypeIsUserOrAdministratorLoggin) ||
                StringUtils.EqualsIgnoreCase(testType, TypeUserGroup))
            {
                return TestTypeDynamic(pageInfo, contextInfo, testType, testValue, testOperate, successTemplateString,
                    failureTemplateString);
            }

            var isSuccess = false;
            if (StringUtils.EqualsIgnoreCase(testType, TypeChannelName))
            {
                var channelName = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId).NodeName;
                isSuccess = TestTypeValue(testOperate, testValue, channelName);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeChannelIndex))
            {
                var channelIndex = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId).NodeIndexName;
                isSuccess = TestTypeValue(testOperate, testValue, channelIndex);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeTemplateName))
            {
                isSuccess = TestTypeValue(testOperate, testValue, pageInfo.TemplateInfo.TemplateName);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeTemplateType))
            {
                isSuccess = TestTypeValue(testOperate, testValue, ETemplateTypeUtils.GetValue(pageInfo.TemplateInfo.TemplateType));
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeTopLevel))
            {
                var topLevel = NodeManager.GetTopLevel(pageInfo.PublishmentSystemId, contextInfo.ChannelId);
                isSuccess = IsNumber(topLevel, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeUpChannel))
            {
                isSuccess = TestTypeUpChannel(pageInfo, contextInfo, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeUpChannelOrSelf))
            {
                isSuccess = TestTypeUpChannelOrSelf(pageInfo, contextInfo, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeSelfChannel))
            {
                isSuccess = pageInfo.PageNodeId == contextInfo.ChannelId;
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeGroupChannel))
            {
                var groupChannels =
                TranslateUtils.StringCollectionToStringList(
                    NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId).NodeGroupNameCollection);
                isSuccess = TestTypeValues(testOperate, testValue, groupChannels);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeGroupContent))
            {
                if (contextInfo.ContextType == EContextType.Content)
                {
                    var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                    var groupContents =
                        TranslateUtils.StringCollectionToStringList(BaiRongDataProvider.ContentDao.GetValue(tableName,
                            contextInfo.ContentId, ContentAttribute.ContentGroupNameCollection));
                    isSuccess = TestTypeValues(testOperate, testValue, groupContents);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeAddDate))
            {
                var addDate = GetAddDateByContext(pageInfo, contextInfo);
                isSuccess = IsDateTime(addDate, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeLastEditDate))
            {
                var lastEditDate = GetLastEditDateByContext(pageInfo, contextInfo);
                isSuccess = IsDateTime(lastEditDate, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeItemIndex))
            {
                var itemIndex = StlParserUtility.GetItemIndex(contextInfo);
                isSuccess = IsNumber(itemIndex, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeOddItem))
            {
                var itemIndex = StlParserUtility.GetItemIndex(contextInfo);
                isSuccess = itemIndex % 2 == 1;
            }
            else
            {
                isSuccess = TestTypeDefault(pageInfo, contextInfo, testType, testOperate, testValue);
            }

            var parsedContent = isSuccess ? successTemplateString : failureTemplateString;

            if (string.IsNullOrEmpty(parsedContent)) return string.Empty;

            var innerBuilder = new StringBuilder(parsedContent);
            StlParserManager.ParseInnerContent(innerBuilder, pageInfo, contextInfo);

            parsedContent = innerBuilder.ToString();

            return parsedContent;
        }

        private static bool TestTypeDefault(PageInfo pageInfo, ContextInfo contextInfo, string testType, string testOperate,
            string testValue)
        {
            var isSuccess = false;

            var theValue = GetAttributeValueByContext(pageInfo, contextInfo, testType);

            if (StringUtils.IsDateTime(theValue))
            {
                var dateTime = TranslateUtils.ToDateTime(theValue);
                isSuccess = IsDateTime(dateTime, testOperate, testValue);
            }
            else if (StringUtils.IsNumber(theValue))
            {
                var number = TranslateUtils.ToInt(theValue);
                isSuccess = IsNumber(number, testOperate, testValue);
            }
            else
            {
                if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotEmpty))
                {
                    if (!string.IsNullOrEmpty(theValue))
                    {
                        isSuccess = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(testOperate, OperateEmpty))
                {
                    if (string.IsNullOrEmpty(theValue))
                    {
                        isSuccess = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals))
                {
                    if (StringUtils.EqualsIgnoreCase(theValue, testValue))
                    {
                        isSuccess = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotEquals))
                {
                    if (!StringUtils.EqualsIgnoreCase(theValue, testValue))
                    {
                        isSuccess = true;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(testOperate, OperateGreatThan))
                {
                    if (StringUtils.Contains(theValue, "-"))
                    {
                        if (TranslateUtils.ToDateTime(theValue) > TranslateUtils.ToDateTime(testValue))
                        {
                            isSuccess = true;
                        }
                    }
                    else
                    {
                        if (TranslateUtils.ToInt(theValue) > TranslateUtils.ToInt(testValue))
                        {
                            isSuccess = true;
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(testOperate, OperateLessThan))
                {
                    if (StringUtils.Contains(theValue, "-"))
                    {
                        if (TranslateUtils.ToDateTime(theValue) < TranslateUtils.ToDateTime(testValue))
                        {
                            isSuccess = true;
                        }
                    }
                    else
                    {
                        if (TranslateUtils.ToInt(theValue) < TranslateUtils.ToInt(testValue))
                        {
                            isSuccess = true;
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
                {
                    var stringList = TranslateUtils.StringCollectionToStringList(testValue);

                    foreach (var str in stringList)
                    {
                        if (StringUtils.EndsWithIgnoreCase(str, "*"))
                        {
                            var theStr = str.Substring(0, str.Length - 1);
                            if (StringUtils.StartsWithIgnoreCase(theValue, theStr))
                            {
                                isSuccess = true;
                                break;
                            }
                        }
                        else
                        {
                            if (StringUtils.EqualsIgnoreCase(theValue, str))
                            {
                                isSuccess = true;
                                break;
                            }
                        }
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
                {
                    var stringList = TranslateUtils.StringCollectionToStringList(testValue);

                    var isIn = false;
                    foreach (var str in stringList)
                    {
                        if (StringUtils.EndsWithIgnoreCase(str, "*"))
                        {
                            var theStr = str.Substring(0, str.Length - 1);
                            if (StringUtils.StartsWithIgnoreCase(theValue, theStr))
                            {
                                isIn = true;
                                break;
                            }
                        }
                        else
                        {
                            if (StringUtils.EqualsIgnoreCase(theValue, str))
                            {
                                isIn = true;
                                break;
                            }
                        }
                    }
                    if (!isIn)
                    {
                        isSuccess = true;
                    }
                }
            }
            return isSuccess;
        }

        private static string TestTypeDynamic(PageInfo pageInfo, ContextInfo contextInfo, string testType, string testValue, string testOperate, string successTemplateString, string failureTemplateString)
        {
            pageInfo.AddPageScriptsIfNotExists(PageInfo.Components.StlClient);

            var ajaxDivId = StlParserUtility.GetAjaxDivId(pageInfo.UniqueId);

            var functionName = $"stlIf_{ajaxDivId}";

            if (string.IsNullOrEmpty(successTemplateString) && string.IsNullOrEmpty(failureTemplateString))
            {
                return string.Empty;
            }

            var pageUrl = StlUtility.GetStlCurrentUrl(pageInfo, contextInfo.ChannelId, contextInfo.ContentId, contextInfo.ContentInfo);

            var ifApiUrl = ActionsIf.GetUrl(pageInfo.ApiUrl);
            var ifApiParms = ActionsIf.GetParameters(pageInfo.PublishmentSystemId, contextInfo.ChannelId, contextInfo.ContentId, pageInfo.TemplateInfo.TemplateId, ajaxDivId, pageUrl, testType, testValue, testOperate, successTemplateString, failureTemplateString);

            var builder = new StringBuilder();
            builder.Append($@"<span id=""{ajaxDivId}""></span>");

            builder.Append($@"
<script type=""text/javascript"" language=""javascript"">
function {functionName}(pageNum)
{{
    var url = ""{ifApiUrl}"";
    var data = {ifApiParms};

    stlClient.post(url, data, function (err, data, status) {{
        if (!err) document.getElementById(""{ajaxDivId}"").innerHTML = data.html;
    }});
}}
{functionName}(0);
</script>
");

            return builder.ToString();
        }

        public static bool TestTypeValues(string testOperate, string testValue, List<string> actualValues)
        {
            var isSuccess = false;

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals) ||
                StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                var stringArrayList = TranslateUtils.StringCollectionToStringList(testValue);

                foreach (string str in stringArrayList)
                {
                    if (actualValues.Contains(str))
                    {
                        isSuccess = true;
                        break;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotEquals) ||
                     StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                var stringArrayList = TranslateUtils.StringCollectionToStringList(testValue);

                var isIn = false;
                foreach (string str in stringArrayList)
                {
                    if (actualValues.Contains(str))
                    {
                        isIn = true;
                        break;
                    }
                }
                if (!isIn)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }

        private static bool TestTypeUpChannelOrSelf(PageInfo pageInfo, ContextInfo contextInfo, string testOperate,
            string testValue)
        {
            var isSuccess = false;

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                var isIn = false;
                foreach (var channelIndex in channelIndexes)
                {
                    var parentId = DataProvider.NodeDao.GetNodeIdByNodeIndexName(pageInfo.PublishmentSystemId, channelIndex);
                    if (NodeManager.IsAncestorOrSelf(pageInfo.PublishmentSystemId, parentId, pageInfo.PageNodeId))
                    {
                        isIn = true;
                        break;
                    }
                }
                if (isIn)
                {
                    isSuccess = true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                var isIn = false;
                foreach (var channelIndex in channelIndexes)
                {
                    var parentId = DataProvider.NodeDao.GetNodeIdByNodeIndexName(pageInfo.PublishmentSystemId, channelIndex);
                    if (NodeManager.IsAncestorOrSelf(pageInfo.PublishmentSystemId, parentId, pageInfo.PageNodeId))
                    {
                        isIn = true;
                        break;
                    }
                }
                if (!isIn)
                {
                    isSuccess = true;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(testValue))
                {
                    if (NodeManager.IsAncestorOrSelf(pageInfo.PublishmentSystemId, contextInfo.ChannelId, pageInfo.PageNodeId))
                    {
                        isSuccess = true;
                    }
                }
                else
                {
                    var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                    foreach (var channelIndex in channelIndexes)
                    {
                        var parentId = DataProvider.NodeDao.GetNodeIdByNodeIndexName(pageInfo.PublishmentSystemId, channelIndex);
                        if (NodeManager.IsAncestorOrSelf(pageInfo.PublishmentSystemId, parentId, pageInfo.PageNodeId))
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }
            return isSuccess;
        }

        private static bool TestTypeUpChannel(PageInfo pageInfo, ContextInfo contextInfo, string testOperate, string testValue)
        {
            var isSuccess = false;

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                var isIn = false;
                foreach (var channelIndex in channelIndexes)
                {
                    var parentId = DataProvider.NodeDao.GetNodeIdByNodeIndexName(pageInfo.PublishmentSystemId, channelIndex);
                    if (parentId != pageInfo.PageNodeId &&
                        NodeManager.IsAncestorOrSelf(pageInfo.PublishmentSystemId, parentId, pageInfo.PageNodeId))
                    {
                        isIn = true;
                        break;
                    }
                }
                if (!isIn)
                {
                    isSuccess = true;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(testValue))
                {
                    if (contextInfo.ChannelId != pageInfo.PageNodeId &&
                        NodeManager.IsAncestorOrSelf(pageInfo.PublishmentSystemId, contextInfo.ChannelId, pageInfo.PageNodeId))
                    {
                        isSuccess = true;
                    }
                }
                else
                {
                    var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                    foreach (var channelIndex in channelIndexes)
                    {
                        var parentId = DataProvider.NodeDao.GetNodeIdByNodeIndexName(pageInfo.PublishmentSystemId, channelIndex);
                        if (parentId != pageInfo.PageNodeId &&
                            NodeManager.IsAncestorOrSelf(pageInfo.PublishmentSystemId, parentId, pageInfo.PageNodeId))
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }
            return isSuccess;
        }

        public static bool TestTypeValue(string testOperate, string testValue, string actualValue)
        {
            var isSuccess = false;
            if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals))
            {
                if (StringUtils.EndsWithIgnoreCase(testValue, "*"))
                {
                    var theStr = testValue.Substring(0, testValue.Length - 1);
                    if (StringUtils.StartsWithIgnoreCase(actualValue, theStr))
                    {
                        isSuccess = true;
                    }
                }
                else
                {
                    if (StringUtils.EqualsIgnoreCase(actualValue, testValue))
                    {
                        isSuccess = true;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotEquals))
            {
                if (!StringUtils.EqualsIgnoreCase(actualValue, testValue))
                {
                    isSuccess = true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                var stringList = TranslateUtils.StringCollectionToStringList(testValue);

                foreach (var str in stringList)
                {
                    if (StringUtils.EndsWithIgnoreCase(str, "*"))
                    {
                        var theStr = str.Substring(0, str.Length - 1);
                        if (StringUtils.StartsWithIgnoreCase(actualValue, theStr))
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                    else
                    {
                        if (StringUtils.EqualsIgnoreCase(actualValue, str))
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                var stringList = TranslateUtils.StringCollectionToStringList(testValue);

                var isIn = false;
                foreach (var str in stringList)
                {
                    if (StringUtils.EndsWithIgnoreCase(str, "*"))
                    {
                        var theStr = str.Substring(0, str.Length - 1);
                        if (StringUtils.StartsWithIgnoreCase(actualValue, theStr))
                        {
                            isIn = true;
                            break;
                        }
                    }
                    else
                    {
                        if (StringUtils.EqualsIgnoreCase(actualValue, str))
                        {
                            isIn = true;
                            break;
                        }
                    }
                }
                if (!isIn)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }

        private static string GetAttributeValueByContext(PageInfo pageInfo, ContextInfo contextInfo, string testTypeStr)
        {
            string theValue = null;
            if (contextInfo.ContextType == EContextType.Content)
            {
                theValue = GetValueFromContent(pageInfo, contextInfo, testTypeStr);
            }
            else if (contextInfo.ContextType == EContextType.Channel)
            {
                theValue = GetValueFromChannel(pageInfo, contextInfo, testTypeStr);
            }
            else if (contextInfo.ContextType == EContextType.Comment)
            {
                if (contextInfo.ItemContainer.CommentItem != null)
                {
                    theValue = DataBinder.Eval(contextInfo.ItemContainer.CommentItem.DataItem, testTypeStr, "{0}");
                }
            }
            else if (contextInfo.ContextType == EContextType.InputContent)
            {
                if (contextInfo.ItemContainer.InputItem != null)
                {
                    theValue = DataBinder.Eval(contextInfo.ItemContainer.InputItem.DataItem, testTypeStr, "{0}");
                }
            }
            else if (contextInfo.ContextType == EContextType.SqlContent)
            {
                if (contextInfo.ItemContainer.SqlItem != null)
                {
                    theValue = DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, testTypeStr, "{0}");
                }
            }
            else if (contextInfo.ContextType == EContextType.Site)
            {
                if (contextInfo.ItemContainer.SiteItem != null)
                {
                    theValue = DataBinder.Eval(contextInfo.ItemContainer.SiteItem.DataItem, testTypeStr, "{0}");
                }
            }
            else
            {
                if (contextInfo.ItemContainer != null)
                {
                    if (contextInfo.ItemContainer.CommentItem != null)
                    {
                        theValue = DataBinder.Eval(contextInfo.ItemContainer.CommentItem.DataItem, testTypeStr, "{0}");
                    }
                    else if (contextInfo.ItemContainer.InputItem != null)
                    {
                        theValue = DataBinder.Eval(contextInfo.ItemContainer.InputItem.DataItem, testTypeStr, "{0}");
                    }
                    else if (contextInfo.ItemContainer.ContentItem != null)
                    {
                        theValue = DataBinder.Eval(contextInfo.ItemContainer.ContentItem.DataItem, testTypeStr, "{0}");
                    }
                    else if (contextInfo.ItemContainer.ChannelItem != null)
                    {
                        theValue = DataBinder.Eval(contextInfo.ItemContainer.ChannelItem.DataItem, testTypeStr, "{0}");
                    }
                    else if (contextInfo.ItemContainer.SqlItem != null)
                    {
                        theValue = DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, testTypeStr, "{0}");
                    }
                }
                else if (contextInfo.ContentId != 0)//获取内容
                {
                    theValue = GetValueFromContent(pageInfo, contextInfo, testTypeStr);
                }
                else if (contextInfo.ChannelId != 0)//获取栏目
                {
                    theValue = GetValueFromChannel(pageInfo, contextInfo, testTypeStr);
                }
            }

            return theValue ?? string.Empty;
        }

        private static string GetValueFromChannel(PageInfo pageInfo, ContextInfo contextInfo, string testTypeStr)
        {
            string theValue;

            var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);

            if (StringUtils.EqualsIgnoreCase(NodeAttribute.AddDate, testTypeStr))
            {
                theValue = DateUtils.GetDateAndTimeString(channel.AddDate);
            }
            else if (StringUtils.EqualsIgnoreCase(NodeAttribute.Title, testTypeStr))
            {
                theValue = channel.NodeName;
            }
            else if (StringUtils.EqualsIgnoreCase(NodeAttribute.ImageUrl, testTypeStr))
            {
                theValue = channel.ImageUrl;
            }
            else if (StringUtils.EqualsIgnoreCase(NodeAttribute.Content, testTypeStr))
            {
                theValue = channel.Content;
            }
            else if (StringUtils.EqualsIgnoreCase(NodeAttribute.CountOfChannels, testTypeStr))
            {
                theValue = channel.ChildrenCount.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(NodeAttribute.CountOfContents, testTypeStr))
            {
                theValue = channel.ContentNum.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(NodeAttribute.CountOfImageContents, testTypeStr))
            {
                var count = DataProvider.BackgroundContentDao.GetCountCheckedImage(pageInfo.PublishmentSystemId, channel.NodeId);
                theValue = count.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(NodeAttribute.LinkUrl, testTypeStr))
            {
                theValue = channel.LinkUrl;
            }
            else
            {
                theValue = channel.Additional.Attributes[testTypeStr];
            }
            return theValue;
        }

        private static string GetValueFromContent(PageInfo pageInfo, ContextInfo contextInfo, string testTypeStr)
        {
            string theValue = null;

            if (contextInfo.ItemContainer?.ContentItem != null)
            {
                theValue = SqlUtils.Eval(contextInfo.ItemContainer.ContentItem.DataItem, testTypeStr) as string;
            }

            if (theValue == null)
            {
                if (contextInfo.ContentInfo == null)
                {
                    var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                    theValue = BaiRongDataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, testTypeStr);
                }
                else
                {
                    theValue = contextInfo.ContentInfo.GetExtendedAttribute(testTypeStr);
                }
            }
            return theValue;
        }

        private static DateTime GetAddDateByContext(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var addDate = DateUtils.SqlMinValue;

            if (contextInfo.ContextType == EContextType.Content)
            {
                if (contextInfo.ContentInfo == null)
                {
                    var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                    addDate = BaiRongDataProvider.ContentDao.GetAddDate(tableName, contextInfo.ContentId);
                }
                else
                {
                    addDate = contextInfo.ContentInfo.AddDate;
                }
            }
            else if (contextInfo.ContextType == EContextType.Channel)
            {
                var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);

                addDate = channel.AddDate;
            }
            else if (contextInfo.ContextType == EContextType.Comment)
            {
                if (contextInfo.ItemContainer.CommentItem != null)
                {
                    addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.CommentItem.DataItem, "AddDate");
                }
            }
            else if (contextInfo.ContextType == EContextType.InputContent)
            {
                if (contextInfo.ItemContainer.InputItem != null)
                {
                    addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.AddDate);
                }
            }
            else
            {
                if (contextInfo.ItemContainer != null)
                {
                    if (contextInfo.ItemContainer.CommentItem != null)
                    {
                        addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.CommentItem.DataItem, "AddDate");
                    }
                    else if (contextInfo.ItemContainer.InputItem != null)
                    {
                        addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.AddDate);
                    }
                    else if (contextInfo.ItemContainer.ContentItem != null)
                    {
                        addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.ContentItem.DataItem, ContentAttribute.AddDate);
                    }
                    else if (contextInfo.ItemContainer.ChannelItem != null)
                    {
                        addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.ChannelItem.DataItem, NodeAttribute.AddDate);
                    }
                    else if (contextInfo.ItemContainer.SqlItem != null)
                    {
                        addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, "AddDate");
                    }
                }
                else if (contextInfo.ContentId != 0)//获取内容
                {
                    if (contextInfo.ContentInfo == null)
                    {
                        var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                        addDate = BaiRongDataProvider.ContentDao.GetAddDate(tableName, contextInfo.ContentId);
                    }
                    else
                    {
                        addDate = contextInfo.ContentInfo.AddDate;
                    }
                }
                else if (contextInfo.ChannelId != 0)//获取栏目
                {
                    var channel = NodeManager.GetNodeInfo(pageInfo.PublishmentSystemId, contextInfo.ChannelId);
                    addDate = channel.AddDate;
                }
            }

            return addDate;
        }

        private static DateTime GetLastEditDateByContext(PageInfo pageInfo, ContextInfo contextInfo)
        {
            var lastEditDate = DateUtils.SqlMinValue;

            if (contextInfo.ContextType == EContextType.Content)
            {
                if (contextInfo.ContentInfo == null)
                {
                    var tableName = NodeManager.GetTableName(pageInfo.PublishmentSystemInfo, contextInfo.ChannelId);
                    lastEditDate = BaiRongDataProvider.ContentDao.GetLastEditDate(tableName, contextInfo.ContentId);
                }
                else
                {
                    lastEditDate = contextInfo.ContentInfo.LastEditDate;
                }
            }

            return lastEditDate;
        }

        private static bool IsNumber(int number, string testOperate, string testValue)
        {
            var isSuccess = false;
            if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals))
            {
                if (number == TranslateUtils.ToInt(testValue))
                {
                    isSuccess = true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotEquals))
            {
                if (number != TranslateUtils.ToInt(testValue))
                {
                    isSuccess = true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateGreatThan))
            {
                if (number > TranslateUtils.ToInt(testValue))
                {
                    isSuccess = true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateLessThan))
            {
                if (number < TranslateUtils.ToInt(testValue))
                {
                    isSuccess = true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                var intArrayList = TranslateUtils.StringCollectionToIntList(testValue);
                foreach (int i in intArrayList)
                {
                    if (i == number)
                    {
                        isSuccess = true;
                        break;
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                var intArrayList = TranslateUtils.StringCollectionToIntList(testValue);
                var isIn = false;
                foreach (int i in intArrayList)
                {
                    if (i == number)
                    {
                        isIn = true;
                        break;
                    }
                }
                if (!isIn)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }

        private static bool IsDateTime(DateTime dateTime, string testOperate, string testValue)
        {
            var isSuccess = false;

            DateTime dateTimeToTest;

            if (StringUtils.EqualsIgnoreCase("now", testValue))
            {
                dateTimeToTest = DateTime.Now;
            }
            else if (DateUtils.IsSince(testValue))
            {
                var hours = DateUtils.GetSinceHours(testValue);
                dateTimeToTest = DateTime.Now.AddHours(-hours);
            }
            else
            {
                dateTimeToTest = TranslateUtils.ToDateTime(testValue);
            }

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals) || StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                isSuccess = dateTime.Date == dateTimeToTest.Date;
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateGreatThan))
            {
                isSuccess = dateTime > dateTimeToTest;
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateLessThan))
            {
                isSuccess = dateTime < dateTimeToTest;
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotEquals) || StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                isSuccess = dateTime.Date != dateTimeToTest.Date;
            }

            return isSuccess;
        }
    }
}
