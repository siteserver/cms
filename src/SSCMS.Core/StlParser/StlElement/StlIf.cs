using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Models;
using SSCMS.Parse;
using SSCMS.Core.StlParser.Utility;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;
using Dynamic = SSCMS.Parse.Dynamic;

namespace SSCMS.Core.StlParser.StlElement
{
    [StlElement(Title = "条件判断", Description = "通过 stl:if 标签在模板中根据条件判断显示内容")]
    public static class StlIf
    {
        public const string ElementName = "stl:if";

        [StlAttribute(Title = "测试类型")]
        private const string Type = nameof(Type);

        [StlAttribute(Title = "测试操作")]
        private const string Op = nameof(Op);

        [StlAttribute(Title = "测试值")]
        private const string Value = nameof(Value);

        [StlAttribute(Title = "判断成功输出值")]
        private const string Yes = nameof(Yes);

        [StlAttribute(Title = "判断失败输出值")]
        private const string No = nameof(No);

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
        private const string TypeChannelName = "ChannelName";			                            //栏目名称
        private const string TypeChannelIndex = "ChannelIndex";			                            //栏目索引
        private const string TypeTemplateName = "TemplateName";			                            //模板名称
        private const string TypTemplateType = "TemplateType";			                            //模板类型
        private const string TypeTopLevel = "TopLevel";			                                    //栏目级别
        private const string TypeUpChannel = "UpChannel";			                                //上级栏目
        private const string TypeSelf = "Self";			                                            //当前栏目
        private const string TypeUpChannelOrSelf = "UpChannelOrSelf";			                    //当前栏目或上级栏目
        private const string TypeCurrent = "Current";			                                    //当前栏目或上级栏目
        private const string TypeIndex = "Index";			                                        //当前页面为首页
        private const string TypeHasChildren = "HasChildren";			                            //是否下级栏目
        private const string TypeGroupChannel = "GroupChannel";			                            //栏目组名称
        private const string TypeGroupContent = "GroupContent";			                            //内容组名称
        private const string TypeAddDate = "AddDate";			                                    //添加时间
        private const string TypeLastModifiedDate = "LastModifiedDate";			                            //最后编辑时间（仅用于判断内容）
        private const string TypeItemIndex = "ItemIndex";			                                //当前项序号
        private const string TypeOddItem = "OddItem";			                                    //奇数项

        private const string OperateEmpty = "Empty";
        private const string OperateNotEmpty = "NotEmpty";			                                //值不为空
        private const string OperateEquals = "Equals";			                                    //值等于
        private const string OperateNotEquals = "NotEquals";			                                //值不等于
        private const string OperateGreatThan = "GreatThan";			                                //值大于
        private const string OperateLessThan = "LessThan";			                                //值小于
        private const string OperateIn = "In";			                                            //值属于
        private const string OperateNotIn = "NotIn";                                                 //值不属于

        internal static async Task<object> ParseAsync(IParseManager parseManager)
        {
            var testTypeStr = string.Empty;
            var testOperate = string.Empty;
            var testValue = string.Empty;
            var yes = string.Empty;
            var no = string.Empty;
            var onBeforeSend = string.Empty;
            var onSuccess = string.Empty;
            var onComplete = string.Empty;
            var onError = string.Empty;

            foreach (var name in parseManager.ContextInfo.Attributes.AllKeys)
            {
                var value = parseManager.ContextInfo.Attributes[name];

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
                    testValue = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                    if (string.IsNullOrEmpty(testOperate))
                    {
                        testOperate = OperateEquals;
                    }
                }
                else if (StringUtils.EqualsIgnoreCase(name, Yes))
                {
                    yes = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, No))
                {
                    no = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, Context))
                {
                    parseManager.ContextInfo.ContextType = TranslateUtils.ToEnum(value, ParseType.Undefined);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnBeforeSend))
                {
                    onBeforeSend = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnSuccess))
                {
                    onSuccess = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnComplete))
                {
                    onComplete = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
                else if (StringUtils.EqualsIgnoreCase(name, OnError))
                {
                    onError = await parseManager.ReplaceStlEntitiesForAttributeValueAsync(value);
                }
            }

            if (string.IsNullOrEmpty(testOperate))
            {
                testOperate = OperateNotEmpty;
            }

            return await ParseAsync(parseManager, testTypeStr, testOperate, testValue, yes, no, onBeforeSend, onSuccess, onComplete, onError);
        }

        private static async Task<string> ParseAsync(IParseManager parseManager, string testType, string testOperate, string testValue, string attributeYes, string attributeNo, string onBeforeSend, string onSuccess, string onComplete, string onError)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var innerHtml = contextInfo.InnerHtml; 

            StlParserUtility.GetLoadingYesNo(innerHtml, out var loading, out var yes, out var no);
            if (string.IsNullOrEmpty(yes) && !string.IsNullOrEmpty(attributeYes))
            {
                yes = attributeYes;
            }
            if (string.IsNullOrEmpty(no) && !string.IsNullOrEmpty(attributeNo))
            {
                no = attributeNo;
            }

            if (StringUtils.EqualsIgnoreCase(testType, TypeIsUserLoggin))
            {
                return await ParseDynamicAsync(parseManager, testType, testValue, testOperate, loading,
                    yes, no, onBeforeSend, onSuccess, onComplete, onError);
            }

            var isSuccess = false;
            if (StringUtils.EqualsIgnoreCase(testType, TypeChannelName))
            {
                var channelName = await databaseManager.ChannelRepository.GetChannelNameAsync(pageInfo.SiteId, contextInfo.ChannelId);
                isSuccess = TestTypeValue(testOperate, testValue, channelName);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeChannelIndex))
            {
                var channelIndex = await databaseManager.ChannelRepository.GetIndexNameAsync(pageInfo.SiteId, contextInfo.ChannelId);
                isSuccess = TestTypeValue(testOperate, testValue, channelIndex);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeTemplateName))
            {
                isSuccess = TestTypeValue(testOperate, testValue, pageInfo.Template.TemplateName);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypTemplateType))
            {
                isSuccess = TestTypeValue(testOperate, testValue, pageInfo.Template.TemplateType.GetValue());
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeTopLevel))
            {
                var topLevel = await databaseManager.ChannelRepository.GetTopLevelAsync(pageInfo.SiteId, contextInfo.ChannelId);
                isSuccess = IsNumber(topLevel, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeUpChannel))
            {
                isSuccess = await TestTypeUpChannelAsync(parseManager, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeSelf))
            {
                isSuccess = IsBool(pageInfo.PageChannelId == contextInfo.ChannelId, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeUpChannelOrSelf) || StringUtils.EqualsIgnoreCase(testType, TypeCurrent))
            {
                isSuccess = await TestTypeUpChannelOrSelfAsync(parseManager, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeIndex))
            {
                isSuccess = IsBool(contextInfo.ChannelId == pageInfo.SiteId, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeHasChildren))
            {
                var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);
                isSuccess = IsBool(channel.ChildrenCount > 0, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeGroupChannel))
            {
                var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);
                isSuccess = TestTypeValues(testOperate, testValue, channel.GroupNames);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeGroupContent))
            {
                if (contextInfo.ContextType == ParseType.Content)
                {
                    var content = await parseManager.GetContentAsync();
                    if (content != null)
                    {
                        var groupContents = content.GroupNames;
                        isSuccess = TestTypeValues(testOperate, testValue, groupContents);
                    }
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeAddDate))
            {
                var addDate = await GetAddDateByContextAsync(parseManager);
                isSuccess = IsDateTime(addDate, testOperate, testValue);
            }
            else if (StringUtils.EqualsIgnoreCase(testType, TypeLastModifiedDate))
            {
                var lastModifiedDate = await GetLastModifiedDateGetAsync(parseManager);
                isSuccess = IsDateTime(lastModifiedDate, testOperate, testValue);
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
                isSuccess = await TestTypeDefaultAsync(parseManager, testType, testOperate, testValue);
            }

            var parsedContent = isSuccess ? yes : no;

            if (string.IsNullOrEmpty(parsedContent)) return string.Empty;

            if (parseManager.ContextInfo.IsStlEntity)
            {
                return parsedContent;
            }

            var innerBuilder = new StringBuilder(parsedContent);
            await parseManager.ParseInnerContentAsync(innerBuilder);

            parsedContent = innerBuilder.ToString();

            return parsedContent;
        }

        private static async Task<bool> TestTypeDefaultAsync(IParseManager parseManager, string testType, string testOperate,
            string testValue)
        {
            var isSuccess = false;

            var theValue = await GetAttributeValueByContextAsync(parseManager, testType);

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
                        if (!string.IsNullOrEmpty(theValue) && theValue.Contains('-'))
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
                        if (!string.IsNullOrEmpty(theValue) && theValue.Contains('-'))
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
                        var stringList = ListUtils.GetStringList(testValue);

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
                        var stringList = ListUtils.GetStringList(testValue);

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

        private static async Task<string> ParseDynamicAsync(IParseManager parseManager, string testType, string testValue, string testOperate, string loading, string yes, string no, string onBeforeSend, string onSuccess, string onComplete, string onError)
        {
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            if (string.IsNullOrEmpty(yes) && string.IsNullOrEmpty(no))
            {
                return string.Empty;
            }

            await pageInfo.AddPageHeadCodeIfNotExistsAsync(ParsePage.Const.StlClient);
            var elementId = StringUtils.GetElementId();

            var dynamicInfo = new Dynamic
            {
                SiteId = pageInfo.SiteId,
                ChannelId = contextInfo.ChannelId,
                ContentId = contextInfo.ContentId,
                TemplateId = pageInfo.Template.Id,
                ElementId = elementId,
                LoadingTemplate = loading,
                YesTemplate = yes,
                NoTemplate = no,
                IsInline = true,
                OnBeforeSend = onBeforeSend,
                OnSuccess = onSuccess,
                OnComplete = onComplete,
                OnError = onError
            };
            var ifInfo = new DynamicIfInfo
            {
                Type = testType,
                Op = testOperate,
                Value = testValue
            };
            dynamicInfo.Settings = TranslateUtils.JsonSerialize(ifInfo);

            var dynamicUrl = parseManager.PathManager.GetIfApiUrl(contextInfo.Site);
            return await StlDynamic.GetScriptAsync(parseManager, dynamicUrl, dynamicInfo);
        }

        private static bool TestTypeValues(string testOperate, string testValue, List<string> actualValues)
        {
            var isSuccess = false;

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals) ||
                StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                var stringList = ListUtils.GetStringList(testValue);

                foreach (var str in stringList)
                {
                    if (!actualValues.Contains(str)) continue;
                    isSuccess = true;
                    break;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotEquals) ||
                     StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                var stringList = ListUtils.GetStringList(testValue);

                var isIn = false;
                foreach (var str in stringList)
                {
                    if (!actualValues.Contains(str)) continue;
                    isIn = true;
                    break;
                }
                if (!isIn)
                {
                    isSuccess = true;
                }
            }
            return isSuccess;
        }

        private static async Task<bool> TestTypeUpChannelOrSelfAsync(IParseManager parseManager, string testOperate, string testValue)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;
            var isSuccess = false;

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateIn))
            {
                var channelIndexes = ListUtils.GetStringList(testValue);
                var isIn = false;
                foreach (var channelIndex in channelIndexes)
                {
                    //var parentId = databaseManager.ChannelRepository.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                    var parentId = await databaseManager.ChannelRepository.GetChannelIdByIndexNameAsync(pageInfo.SiteId, channelIndex);
                    if (!await databaseManager.ChannelRepository.IsAncestorOrSelfAsync(pageInfo.SiteId, parentId, pageInfo.PageChannelId)) continue;
                    isIn = true;
                    break;
                }
                if (isIn)
                {
                    isSuccess = true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                var channelIndexes = ListUtils.GetStringList(testValue);
                var isIn = false;
                foreach (var channelIndex in channelIndexes)
                {
                    //var parentId = databaseManager.ChannelRepository.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                    int parentId = await databaseManager.ChannelRepository.GetChannelIdByIndexNameAsync(pageInfo.SiteId, channelIndex);
                    if (await databaseManager.ChannelRepository.IsAncestorOrSelfAsync(pageInfo.SiteId, parentId, pageInfo.PageChannelId))
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
                    if (await databaseManager.ChannelRepository.IsAncestorOrSelfAsync(pageInfo.SiteId, contextInfo.ChannelId, pageInfo.PageChannelId))
                    {
                        isSuccess = true;
                    }
                }
                else
                {
                    var channelIndexes = ListUtils.GetStringList(testValue);
                    foreach (var channelIndex in channelIndexes)
                    {
                        //var parentId = databaseManager.ChannelRepository.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                        var parentId = await databaseManager.ChannelRepository.GetChannelIdByIndexNameAsync(pageInfo.SiteId, channelIndex);
                        if (await databaseManager.ChannelRepository.IsAncestorOrSelfAsync(pageInfo.SiteId, parentId, pageInfo.PageChannelId))
                        {
                            isSuccess = true;
                            break;
                        }
                    }
                }
            }
            return isSuccess;
        }

        private static async Task<bool> TestTypeUpChannelAsync(IParseManager parseManager, string testOperate, string testValue)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;

            var isSuccess = false;

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotIn))
            {
                var channelIndexes = ListUtils.GetStringList(testValue);
                var isIn = false;
                foreach (var channelIndex in channelIndexes)
                {
                    //var parentId = databaseManager.ChannelRepository.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                    var parentId = await databaseManager.ChannelRepository.GetChannelIdByIndexNameAsync(pageInfo.SiteId, channelIndex);
                    if (parentId != pageInfo.PageChannelId &&
                        await databaseManager.ChannelRepository.IsAncestorOrSelfAsync(pageInfo.SiteId, parentId, pageInfo.PageChannelId))
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
                    if (contextInfo.ChannelId != pageInfo.PageChannelId &&
                        await databaseManager.ChannelRepository.IsAncestorOrSelfAsync(pageInfo.SiteId, contextInfo.ChannelId, pageInfo.PageChannelId))
                    {
                        isSuccess = true;
                    }
                }
                else
                {
                    var channelIndexes = ListUtils.GetStringList(testValue);
                    foreach (var channelIndex in channelIndexes)
                    {
                        //var parentId = databaseManager.ChannelRepository.GetIdByIndexName(pageInfo.SiteId, channelIndex);
                        var parentId = await databaseManager.ChannelRepository.GetChannelIdByIndexNameAsync(pageInfo.SiteId, channelIndex);
                        if (parentId != pageInfo.PageChannelId &&
                            await databaseManager.ChannelRepository.IsAncestorOrSelfAsync(pageInfo.SiteId, parentId, pageInfo.PageChannelId))
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
                var stringList = ListUtils.GetStringList(testValue);

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
                var stringList = ListUtils.GetStringList(testValue);

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

        private static async Task<string> GetAttributeValueByContextAsync(IParseManager parseManager, string testTypeStr)
        {
            var contextInfo = parseManager.ContextInfo;

            string theValue = null;
            if (contextInfo.ContextType == ParseType.Content)
            {
                theValue = await GetValueFromContentAsync(parseManager, testTypeStr);
            }
            else if (contextInfo.ContextType == ParseType.Channel)
            {
                theValue = await GetValueFromChannelAsync(parseManager, testTypeStr);
            }
            else if (contextInfo.ContextType == ParseType.SqlContent)
            {
                contextInfo.ItemContainer.SqlItem.Value.TryGetValue(testTypeStr, out var obj);
                if (obj != null)
                {
                    theValue = obj.ToString();
                }
            }
            else if (contextInfo.ContextType == ParseType.Site)
            {
                var site = contextInfo.ItemContainer.SiteItem.Value;
                if (site != null)
                {
                    theValue = site.Get<string>(testTypeStr);
                }
            }
            else
            {
                if (contextInfo.ItemContainer != null)
                {
                    //else if (contextInfo.ItemContainer.InputItem != null)
                    //{
                    //    theValue = DataBinder.Eval(contextInfo.ItemContainer.InputItem.DataItem, testTypeStr, "{0}");
                    //}
                    //else if (contextInfo.ItemContainer.ContentItem != null)
                    //{
                    //    theValue = DataBinder.Eval(contextInfo.ItemContainer.ContentItem.DataItem, testTypeStr, "{0}");
                    //}
                    //else if (contextInfo.ItemContainer.ChannelItem != null)
                    //{
                    //    theValue = DataBinder.Eval(contextInfo.ItemContainer.ChannelItem.DataItem, testTypeStr, "{0}");
                    //}
                    //if (contextInfo.ItemContainer.SqlItem != null)
                    //{
                    //    theValue = DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, testTypeStr, "{0}");
                    //}
                }
                else if (contextInfo.ContentId != 0)//获取内容
                {
                    theValue = await GetValueFromContentAsync(parseManager, testTypeStr);
                }
                else if (contextInfo.ChannelId != 0)//获取栏目
                {
                    theValue = await GetValueFromChannelAsync(parseManager, testTypeStr);
                }
            }

            return theValue ?? string.Empty;
        }

        private static async Task<string> GetValueFromChannelAsync(IParseManager parseManager, string testTypeStr)
        {
            var databaseManager = parseManager.DatabaseManager;
            var pageInfo = parseManager.PageInfo;
            var contextInfo = parseManager.ContextInfo;
            string theValue;

            var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);

            if (StringUtils.EqualsIgnoreCase(nameof(Channel.AddDate), testTypeStr))
            {
                theValue = DateUtils.GetDateAndTimeString(channel.AddDate);
            }
            else if (StringUtils.EqualsIgnoreCase(StlParserUtility.Title, testTypeStr) || StringUtils.EqualsIgnoreCase(nameof(Channel.ChannelName), testTypeStr))
            {
                theValue = channel.ChannelName;
            }
            else if (StringUtils.EqualsIgnoreCase(nameof(Channel.ImageUrl), testTypeStr))
            {
                theValue = channel.ImageUrl;
            }
            else if (StringUtils.EqualsIgnoreCase(nameof(Channel.Content), testTypeStr))
            {
                theValue = channel.Content;
            }
            else if (StringUtils.EqualsIgnoreCase(StlParserUtility.CountOfChannels, testTypeStr))
            {
                theValue = channel.ChildrenCount.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(StlParserUtility.CountOfContents, testTypeStr))
            {
                var count = await databaseManager.ContentRepository.GetCountAsync(pageInfo.Site, channel);
                theValue = count.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(StlParserUtility.CountOfImageContents, testTypeStr))
            {
                var count = await databaseManager.ContentRepository.GetCountCheckedImageAsync(pageInfo.Site, channel);
                theValue = count.ToString();
            }
            else if (StringUtils.EqualsIgnoreCase(nameof(Channel.LinkUrl), testTypeStr))
            {
                theValue = channel.LinkUrl;
            }
            else
            {
                theValue = channel.Get<string>(testTypeStr);
            }
            return theValue;
        }

        private static async Task<string> GetValueFromContentAsync(IParseManager parseManager, string testTypeStr)
        {
            var theValue = string.Empty;

            var contentInfo = await parseManager.GetContentAsync();

            if (contentInfo != null)
            {
                if (StringUtils.EqualsIgnoreCase(testTypeStr, "IsTop"))
                {
                    theValue = contentInfo.Get<string>(nameof(Content.Top));
                }
                else if (StringUtils.EqualsIgnoreCase(testTypeStr, "IsRecommend"))
                {
                    theValue = contentInfo.Get<string>(nameof(Content.Recommend));
                }
                else if (StringUtils.EqualsIgnoreCase(testTypeStr, "IsColor"))
                {
                    theValue = contentInfo.Get<string>(nameof(Content.Color));
                }
                else if (StringUtils.EqualsIgnoreCase(testTypeStr, "IsHot"))
                {
                    theValue = contentInfo.Get<string>(nameof(Content.Hot));
                }
                else
                {
                    theValue = contentInfo.Get<string>(testTypeStr);
                }
            }

            return theValue;
        }

        private static async Task<DateTime> GetAddDateByContextAsync(IParseManager parseManager)
        {
            var databaseManager = parseManager.DatabaseManager;
            var contextInfo = parseManager.ContextInfo;

            var addDate = Constants.SqlMinValue;

            var contentInfo = await parseManager.GetContentAsync();

            if (contextInfo.ContextType == ParseType.Content)
            {
                if (contentInfo.AddDate.HasValue)
                {
                    addDate = contentInfo.AddDate.Value;
                }
            }
            else if (contextInfo.ContextType == ParseType.Channel)
            {
                var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);
                if (channel.AddDate.HasValue)
                {
                    addDate = channel.AddDate.Value;
                }
            }
            else
            {
                if (contextInfo.ItemContainer != null)
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
                    //if (contextInfo.ItemContainer.SqlItem != null)
                    //{
                    //    addDate = (DateTime)DataBinder.Eval(contextInfo.ItemContainer.SqlItem.DataItem, "AddDate");
                    //}
                }
                else if (contextInfo.ContentId != 0)//获取内容
                {
                    if (contentInfo.AddDate.HasValue)
                    {
                        addDate = contentInfo.AddDate.Value;
                    }
                }
                else if (contextInfo.ChannelId != 0)//获取栏目
                {
                    var channel = await databaseManager.ChannelRepository.GetAsync(contextInfo.ChannelId);
                    if (channel.AddDate.HasValue)
                    {
                        addDate = channel.AddDate.Value;
                    }
                }
            }

            return addDate;
        }

        private static async Task<DateTime> GetLastModifiedDateGetAsync(IParseManager parseManager)
        {
            var lastModifiedDate = Constants.SqlMinValue;

            var contentInfo = await parseManager.GetContentAsync();

            if (parseManager.ContextInfo.ContextType == ParseType.Content)
            {
                if (contentInfo.LastModifiedDate.HasValue)
                {
                    lastModifiedDate = contentInfo.LastModifiedDate.Value;
                }
            }

            return lastModifiedDate;
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
                var intArrayList = ListUtils.GetIntList(testValue);
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
                var intArrayList = ListUtils.GetIntList(testValue);
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

        private static bool IsBool(bool boolValue, string testOperate, string testValue)
        {
            var isSuccess = false;

            if (string.IsNullOrEmpty(testValue))
            {
                testValue = true.ToString();
            }
            if (string.IsNullOrEmpty(testOperate) || StringUtils.EqualsIgnoreCase(testOperate, OperateNotEmpty))
            {
                testOperate = OperateEquals;
            }

            if (StringUtils.EqualsIgnoreCase(testOperate, OperateEquals))
            {
                if (boolValue == TranslateUtils.ToBool(testValue))
                {
                    isSuccess = true;
                }
            }
            else if (StringUtils.EqualsIgnoreCase(testOperate, OperateNotEquals))
            {
                if (boolValue != TranslateUtils.ToBool(testValue))
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

