using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Core.Api.Sys.Stl;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Core.StlParser.Utility;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.StlParser.StlElement
{
    [StlElement(Title = "条件判断", Description = "通过 stl:if 标签在模板中根据条件判断显示内容")]
    public class StlIf
    {
        private StlIf() { }
        public const string ElementName = "stl:if";

        [StlAttribute(Title = "测试类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "测试操作")]
        private const string Op = nameof(Op);

        [StlAttribute(Title = "测试值")]
        private const string Value = nameof(Value);

        [StlAttribute(Title = "所处上下文")]
        private const string Context = nameof(Context);

        [StlAttribute(Title = "动态请求发送前执行的JS代码")]
        private const string OnBeforeSend = nameof(OnBeforeSend);

        [StlAttribute(Title = "动态请求成功后执行的JS代码")]
        private const string OnSuccess = nameof(OnSuccess);

        [StlAttribute(Title = "动态请求结束后执行的JS代码")]
        private const string OnComplete = nameof(OnComplete);

        [StlAttribute(Title = "动态请求失败后执行的JS代码")]
        private const string OnError = nameof(OnError);

        public const string TypeIsUserLoggin = "IsUserLoggin";                                      //用户是否已登录
        public const string TypeIsAdministratorLoggin = "IsAdministratorLoggin";                    //管理员是否已登录
        public const string TypeIsUserOrAdministratorLoggin = "IsUserOrAdministratorLoggin";        //用户或管理员是否已登录
        private const string TypeChannelName = "ChannelName";			                            //栏目名称
        private const string TypeChannelIndex = "ChannelIndex";			                            //栏目索引
        private const string TypeTemplateName = "TemplateName";			                            //模板名称
        private const string TypTemplateType = "TemplateType";			                            //模板类型
        private const string TypeTopLevel = "TopLevel";			                                    //栏目级别
        private const string TypeUpChannel = "UpChannel";			                                //上级栏目
        private const string TypeUpChannelOrSelf = "UpChannelOrSelf";			                    //当前栏目或上级栏目
        private const string TypeSelfChannel = "SelfChannel";			                            //当前栏目
        private const string TypeGroupChannel = "GroupChannel";			                            //栏目组名称
        private const string TypeGroupContent = "GroupContent";			                            //内容组名称
        private const string TypeAddDate = "AddDate";			                                    //添加时间
        private const string TypeLastEditDate = "LastEditDate";			                            //最后编辑时间（仅用于判断内容）
        private const string TypeItemIndex = "ItemIndex";			                                //当前项序号
        private const string TypeOddItem = "OddItem";			                                    //奇数项

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeIsUserLoggin, "用户是否已登录"},
            {TypeIsAdministratorLoggin, "管理员是否已登录"},
            {TypeIsUserOrAdministratorLoggin, "用户或管理员是否已登录"},
            {TypeChannelName, "栏目名称"},
            {TypeChannelIndex, "栏目索引"},
            {TypeTemplateName, "模板名称"},
            {TypTemplateType, "模板类型"},
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


        internal static string Parse(ParseContext parseContext)
        {
            var testTypeStr = string.Empty;
            var testOperate = string.Empty;
            var testValue = string.Empty;
            var onBeforeSend = string.Empty;
            var onSuccess = string.Empty;
            var onComplete = string.Empty;
            var onError = string.Empty;

            foreach (var name in parseContext.Attributes.AllKeys)
            {
                var value = parseContext.Attributes[name];

                if (StringUtils.EqualsIgnoreCase(name, Type) || StringUtils.EqualsIgnoreCase(name, "testType"))
                {
                    testTypeStr = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Op) || StringUtils.EqualsIgnoreCase(name, "operate") || StringUtils.EqualsIgnoreCase(name, "testOperate"))
                {
                    testOperate = value;
                }
                else if (StringUtils.EqualsIgnoreCase(name, Value) || StringUtils.EqualsIgnoreCase(name, "testValue"))
                {
                    testValue = value;
                    if (string.IsNullOrEmpty(testOperate))
                    {
                        testOperate = OperateEquals;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    parseContext.ContextType = EContextTypeUtils.GetEnumType(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnBeforeSend))
                {
                    onBeforeSend = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnSuccess))
                {
                    onSuccess = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnComplete))
                {
                    onComplete = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnError))
                {
                    onError = parseContext.ReplaceStlEntitiesForAttributeValue(value);
                }
            }

            if (string.IsNullOrEmpty(testOperate))
            {
                testOperate = OperateNotEmpty;
            }

            return ParseImpl(parseContext, testTypeStr, testOperate, testValue, onBeforeSend, onSuccess, onComplete, onError);
        }

        private static string ParseImpl(ParseContext parseContext, string testType, string testOperate, string testValue, string onBeforeSend, string onSuccess, string onComplete, string onError)
        {
            string loading;
            string yes;
            string no;

            StlParserUtility.GetLoadingYesNo(parseContext.InnerHtml, out loading, out yes, out no);

            if (StringUtils.EqualsIgnoreCase(testType, TypeIsUserLoggin) ||
                StringUtils.EqualsIgnoreCase(testType, TypeIsAdministratorLoggin) ||
                StringUtils.EqualsIgnoreCase(testType, TypeIsUserOrAdministratorLoggin))
            {
                return ParseDynamic(parseContext, testType, testValue, testOperate, loading,
                    yes, no, onBeforeSend, onSuccess, onComplete, onError);
            }

            var isSuccess = false;
            if (StringUtils.EqualsIgnoreCase(testType, TypeChannelName))
            {
                var channelName = parseContext.ChannelRepository.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId).ChannelName;
                isSuccess = TestTypeValue(testOperate, testValue, channelName);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeChannelIndex))
            {
                var channelIndex = parseContext.ChannelRepository.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId).IndexName;
                isSuccess = TestTypeValue(testOperate, testValue, channelIndex);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeTemplateName))
            {
                isSuccess = TestTypeValue(testOperate, testValue, parseContext.TemplateInfo.TemplateName);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypTemplateType))
            {
                isSuccess = TestTypeValue(testOperate, testValue, parseContext.TemplateInfo.Type.Value);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeTopLevel))
            {
                var topLevel = parseContext.ChannelRepository.GetTopLevel(parseContext.SiteId, parseContext.ChannelId);
                isSuccess = IsNumber(topLevel, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeUpChannel))
            {
                isSuccess = TestTypeUpChannel(parseContext, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeUpChannelOrSelf))
            {
                isSuccess = TestTypeUpChannelOrSelf(parseContext, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeSelfChannel))
            {
                isSuccess = parseContext.PageChannelId == parseContext.ChannelId;
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeGroupChannel))
            {
                var groupChannels =
                TranslateUtils.StringCollectionToStringList(
                    parseContext.ChannelRepository.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId).GroupNameCollection);
                isSuccess = TestTypeValues(testOperate, testValue, groupChannels);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeGroupContent))
            {
                if (parseContext.ContextType == EContextType.Content)
                {
                    //var groupContents = TranslateUtils.StringCollectionToStringList(DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, ContentAttribute.ContentGroupNameCollection));
                    var groupContents = TranslateUtils.StringCollectionToStringList(parseContext.ChannelInfo.ContentRepository.StlGetValue(parseContext.ChannelInfo, parseContext.ContentId, ContentAttribute.GroupNameCollection));
                    isSuccess = TestTypeValues(testOperate, testValue, groupContents);
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeAddDate))
            {
                var addDate = GetAddDateByContext(parseContext);
                isSuccess = IsDateTime(addDate, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeLastEditDate))
            {
                var lastEditDate = GetLastEditDateByContext(parseContext);
                isSuccess = IsDateTime(lastEditDate, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeItemIndex))
            {
                var itemIndex = StlParserUtility.GetItemIndex(parseContext);
                isSuccess = IsNumber(itemIndex, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeOddItem))
            {
                var itemIndex = StlParserUtility.GetItemIndex(parseContext);
                isSuccess = itemIndex % 2 == 1;
            }
            else
            {
                isSuccess = TestTypeDefault(parseContext, testType, testOperate, testValue);
            }

            var parsedContent = isSuccess ? yes : no;

            if (string.IsNullOrEmpty(parsedContent)) return string.Empty;

            var innerBuilder = new StringBuilder(parsedContent);
            parseContext.ParseInnerContent(innerBuilder);

            parsedContent = innerBuilder.ToString();

            return parsedContent;
        }

        private static bool TestTypeDefault(ParseContext parseContext, string testType, string testOperate,
            string testValue)
        {
            var isSuccess = false;

            var theValue = GetAttributeValueByContext(parseContext, testType);

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
            else
            {
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
                    if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals))
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
            }

            return isSuccess;
        }

        private static string ParseDynamic(ParseContext parseContext, string testType, string testValue, string testOperate, string loading, string yes, string no, string onBeforeSend, string onSuccess, string onComplete, string onError)
        {
            if (string.IsNullOrEmpty(yes) && string.IsNullOrEmpty(no))
            {
                return string.Empty;
            }

            parseContext.PageInfo.AddPageBodyCodeIfNotExists(parseContext.UrlManager, PageInfo.Const.StlClient);
            var ajaxDivId = StlParserUtility.GetAjaxDivId(parseContext.UniqueId);

            //运行解析以便为页面生成所需JS引用
            if (!string.IsNullOrEmpty(yes))
            {
                parseContext.ParseInnerContent(new StringBuilder(yes));
            }
            if (!string.IsNullOrEmpty(no))
            {
                parseContext.ParseInnerContent(new StringBuilder(no));
            }

            var dynamicInfo = new DynamicInfo
            {
                ElementName = ElementName,
                SiteId = parseContext.SiteId,
                ChannelId = parseContext.ChannelId,
                ContentId = parseContext.ContentId,
                TemplateId = parseContext.TemplateInfo.Id,
                AjaxDivId = ajaxDivId,
                LoadingTemplate = loading,
                SuccessTemplate = yes,
                FailureTemplate = no,
                OnBeforeSend = onBeforeSend,
                OnSuccess = onSuccess,
                OnComplete = onComplete,
                OnError = onError
            };
            var ifInfo = new DynamicInfo.IfInfo
            {
                Type = testType,
                Op = testOperate,
                Value = testValue
            };
            dynamicInfo.ElementValues = TranslateUtils.JsonSerialize(ifInfo);

            return dynamicInfo.GetScript(parseContext.SettingsManager, ApiRouteActionsIf.GetUrl());
        }

        private static bool TestTypeValues(string testOperate, string testValue, List<string> actualValues)
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

        private static bool TestTypeUpChannelOrSelf(ParseContext parseContext, string testOperate,
            string testValue)
        {
            var isSuccess = false;

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                var isIn = false;
                foreach (var channelIndex in channelIndexes)
                {
                    //var parentId = DataProvider.ChannelDao.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                    var parentId = parseContext.ChannelRepository.GetChannelIdByIndexName(parseContext.SiteId, channelIndex);
                    if (parseContext.ChannelRepository.IsAncestorOrSelf(parseContext.SiteId, parentId, parseContext.PageChannelId))
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
                    //var parentId = DataProvider.ChannelDao.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                    var parentId = parseContext.ChannelRepository.GetChannelIdByIndexName(parseContext.SiteId, channelIndex);
                    if (parseContext.ChannelRepository.IsAncestorOrSelf(parseContext.SiteId, parentId, parseContext.PageChannelId))
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
                    if (parseContext.ChannelRepository.IsAncestorOrSelf(parseContext.SiteId, parseContext.ChannelId, parseContext.PageChannelId))
                    {
                        isSuccess = true;
                    }
                }
                else
                {
                    var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                    foreach (var channelIndex in channelIndexes)
                    {
                        //var parentId = DataProvider.ChannelDao.GetIdByIndexName(parseContext.SiteId, channelIndex);
                        var parentId = parseContext.ChannelRepository.GetChannelIdByIndexName(parseContext.SiteId, channelIndex);
                        if (parseContext.ChannelRepository.IsAncestorOrSelf(parseContext.SiteId, parentId, parseContext.PageChannelId))
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }
            return isSuccess;
        }

        private static bool TestTypeUpChannel(ParseContext parseContext, string testOperate, string testValue)
        {
            var isSuccess = false;

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                var isIn = false;
                foreach (var channelIndex in channelIndexes)
                {
                    //var parentId = DataProvider.ChannelDao.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                    var parentId = parseContext.ChannelRepository.GetChannelIdByIndexName(parseContext.SiteId, channelIndex);
                    if (parentId != parseContext.PageChannelId &&
                        parseContext.ChannelRepository.IsAncestorOrSelf(parseContext.SiteId, parentId, parseContext.PageChannelId))
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
                    if (parseContext.ChannelId != parseContext.PageChannelId &&
                        parseContext.ChannelRepository.IsAncestorOrSelf(parseContext.SiteId, parseContext.ChannelId, parseContext.PageChannelId))
                    {
                        isSuccess = true;
                    }
                }
                else
                {
                    var channelIndexes = TranslateUtils.StringCollectionToStringList(testValue);
                    foreach (var channelIndex in channelIndexes)
                    {
                        //var parentId = DataProvider.ChannelDao.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                        var parentId = parseContext.ChannelRepository.GetChannelIdByIndexName(parseContext.SiteId, channelIndex);
                        if (parentId != parseContext.PageChannelId &&
                            parseContext.ChannelRepository.IsAncestorOrSelf(parseContext.SiteId, parentId, parseContext.PageChannelId))
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }
            return isSuccess;
        }

        private static bool TestTypeValue(string testOperate, string testValue, string actualValue)
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

        private static string GetAttributeValueByContext(ParseContext parseContext, string testTypeStr)
        {
            string theValue = null;
            if (parseContext.ContextType == EContextType.Site)
            {
                if (!parseContext.Container.SiteItem.Equals(default(KeyValuePair<int, SiteInfo>)))
                {
                    var siteInfo = parseContext.SiteRepository.GetSiteInfo(parseContext.Container.SiteItem.Value.Id);
                    if (siteInfo != null)
                    {
                        theValue = GetValueFromSite(parseContext, siteInfo, testTypeStr);
                    }
                }
                else
                {
                    theValue = GetValueFromSite(parseContext, parseContext.SiteInfo, testTypeStr);
                }
            }
            else if (parseContext.ContextType == EContextType.Content)
            {
                theValue = GetValueFromContent(parseContext, testTypeStr);
            }
            else if (parseContext.ContextType == EContextType.Channel)
            {
                theValue = GetValueFromChannel(parseContext, testTypeStr);
            }
            else if (parseContext.ContextType == EContextType.SqlContent)
            {
                if (!parseContext.Container.SqlItem.Equals(default(KeyValuePair<int, Dictionary<string, object>>)))
                {
                    if (parseContext.Container.SqlItem.Value.TryGetValue(testTypeStr, out var value))
                    {
                        theValue = Convert.ToString(value);
                    }
                }
            }
            else
            {
                if (parseContext.Container != null)
                {
                    if (!parseContext.Container.SqlItem.Equals(default(KeyValuePair<int, Dictionary<string, object>>)))
                    {
                        if (parseContext.Container.SqlItem.Value.TryGetValue(testTypeStr, out var value))
                        {
                            theValue = Convert.ToString(value);
                        }
                    }
                }
                else if (parseContext.ContentId != 0)//获取内容
                {
                    theValue = GetValueFromContent(parseContext, testTypeStr);
                }
                else if (parseContext.ChannelId != 0)//获取栏目
                {
                    theValue = GetValueFromChannel(parseContext, testTypeStr);
                }
            }

            return theValue ?? string.Empty;
        }

        private static string GetValueFromSite(ParseContext parseContext, SiteInfo siteInfo, string testTypeStr)
        {
            string theValue;

            if (StringUtils.EqualsIgnoreCase(SiteAttribute.Id, testTypeStr))
            {
                theValue = siteInfo.Id.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(SiteAttribute.SiteName, testTypeStr))
            {
                theValue = siteInfo.SiteName;
            }
            else if (StringUtils.EqualsIgnoreCase(SiteAttribute.SiteDir, testTypeStr))
            {
                theValue = siteInfo.SiteDir;
            }
            else if (StringUtils.EqualsIgnoreCase(SiteAttribute.TableName, testTypeStr))
            {
                theValue = siteInfo.TableName;
            }
            else if (StringUtils.EqualsIgnoreCase(SiteAttribute.IsRoot, testTypeStr))
            {
                theValue = siteInfo.IsRoot.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(SiteAttribute.ParentId, testTypeStr))
            {
                theValue = siteInfo.ParentId.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(SiteAttribute.Taxis, testTypeStr))
            {
                theValue = siteInfo.Taxis.ToString();
            }
            else
            {
                theValue = siteInfo.Get<string>(testTypeStr);
            }
            return theValue;
        }

        private static string GetValueFromChannel(ParseContext parseContext, string testTypeStr)
        {
            string theValue;

            var channel = parseContext.ChannelRepository.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId);

            if (StringUtils.EqualsIgnoreCase(ChannelAttribute.CreatedDate, testTypeStr))
            {
                theValue = DateUtils.GetDateAndTimeString(channel.CreatedDate);
            }
            else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.Title, testTypeStr))
            {
                theValue = channel.ChannelName;
            }
            else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.ImageUrl, testTypeStr))
            {
                theValue = channel.ImageUrl;
            }
            else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.Content, testTypeStr))
            {
                theValue = channel.Content;
            }
            else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.CountOfChannels, testTypeStr))
            {
                theValue = channel.ChildrenCount.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.CountOfContents, testTypeStr))
            {
                var count = channel.ContentRepository.GetCount(parseContext.SiteInfo, channel, true);
                theValue = count.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.CountOfImageContents, testTypeStr))
            {
                //var count = DataProvider.BackgroundContentDao.GetCountCheckedImage(parseContext.SiteId, channel.ChannelId);
                var count = channel.ContentRepository.StlGetCountCheckedImage(parseContext.SiteId, channel);
                theValue = count.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(ChannelAttribute.LinkUrl, testTypeStr))
            {
                theValue = channel.LinkUrl;
            }
            else
            {
                theValue = channel.Get<string>(testTypeStr);
            }
            return theValue;
        }

        private static string GetValueFromContent(ParseContext parseContext, string testTypeStr)
        {
            string theValue;

            if (parseContext.ContentInfo == null)
            {
                //theValue = DataProvider.ContentDao.GetValue(tableName, contextInfo.ContentId, testTypeStr);
                theValue = parseContext.ChannelInfo.ContentRepository.StlGetValue(parseContext.ChannelInfo, parseContext.ContentId, testTypeStr);
            }
            else
            {
                theValue = parseContext.ContentInfo.Get<string>(testTypeStr);
            }

            return theValue;
        }

        private static DateTimeOffset? GetAddDateByContext(ParseContext parseContext)
        {
            DateTimeOffset? addDate = null;

            if (parseContext.ContextType == EContextType.Content)
            {
                if (parseContext.ContentInfo.AddDate.HasValue)
                {
                    addDate = parseContext.ContentInfo.AddDate.Value;
                }
            }
            else if (parseContext.ContextType == EContextType.Channel)
            {
                var channel = parseContext.ChannelRepository.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId);
                if (channel.CreatedDate.HasValue)
                {
                    addDate = channel.CreatedDate.Value;
                }
            }
            else
            {
                if (parseContext.Container != null)
                {
                    //else if (contextInfo.ItemContainer.InputItem != null)
                    //{
                    //    addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.InputItem.DataItem, InputContentAttribute.AddDate);
                    //}
                    //else if (contextInfo.ItemContainer.ContentItem != null)
                    //{
                    //    addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.ContentItem.DataItem, ContentAttribute.AddDate);
                    //}
                    //else if (contextInfo.ItemContainer.ChannelItem != null)
                    //{
                    //    addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.ChannelItem.DataItem, NodeAttribute.AddDate);
                    //}
                    if (!parseContext.Container.SqlItem.Equals(default(KeyValuePair<int, Dictionary<string, object>>)))
                    {
                        if (parseContext.Container.SqlItem.Value.TryGetValue("AddDate", out var value))
                        {
                            addDate = (DateTime)value;
                        }
                    }
                }
                else if (parseContext.ContentId != 0)//获取内容
                {
                    if (parseContext.ContentInfo.AddDate.HasValue)
                    {
                        addDate = parseContext.ContentInfo.AddDate.Value;
                    }
                }
                else if (parseContext.ChannelId != 0)//获取栏目
                {
                    var channel = parseContext.ChannelRepository.GetChannelInfo(parseContext.SiteId, parseContext.ChannelId);
                    if (channel.CreatedDate.HasValue)
                    {
                        addDate = channel.CreatedDate.Value;
                    }
                }
            }

            return addDate;
        }

        private static DateTimeOffset? GetLastEditDateByContext(ParseContext parseContext)
        {
            DateTimeOffset? lastEditDate = null;

            if (parseContext.ContextType == EContextType.Content)
            {
                if (parseContext.ContentInfo.LastModifiedDate.HasValue)
                {
                    lastEditDate = parseContext.ContentInfo.LastModifiedDate.Value;
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
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
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

        private static bool IsDateTime(DateTimeOffset? dateTime, string testOperate, string testValue)
        {
            if (dateTime == null) return false;
            var isSuccess = false;

            DateTimeOffset dateTimeToTest;

            if (StringUtils.EqualsIgnoreCase("now", testValue))
            {
                dateTimeToTest = DateTimeOffset.UtcNow;
            }
            else if (DateUtils.IsSince(testValue))
            {
                var hours = DateUtils.GetSinceHours(testValue);
                dateTimeToTest = DateTimeOffset.UtcNow.AddHours(-hours);
            }
            else
            {
                dateTimeToTest = TranslateUtils.ToDateTime(testValue);
            }

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals) || StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                isSuccess = dateTime.Value.Date == dateTimeToTest.Date;
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
                isSuccess = dateTime.Value.Date != dateTimeToTest.Date;
            }

            return isSuccess;
        }
    }
}

