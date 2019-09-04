using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Common.Enums;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Common
{
    public static class ContentUtility
    {
        public static string PagePlaceHolder = "[SITESERVER_PAGE]";//内容翻页占位符

        public static string GetTitleFormatString(bool isStrong, bool isEm, bool isU, string color)
        {
            return $"{isStrong}_{isEm}_{isU}_{color}";
        }

        // public static bool SetTitleFormatControls(string titleFormatString, CheckBox formatStrong, CheckBox formatEm, CheckBox formatU, TextBox formatColor)
        // {
        //     var isTitleFormatted = false;
        //     if (!string.IsNullOrEmpty(titleFormatString))
        //     {
        //         var formats = titleFormatString.Split('_');
        //         if (formats.Length == 4)
        //         {
        //             formatStrong.Checked = TranslateUtils.ToBool(formats[0]);
        //             formatEm.Checked = TranslateUtils.ToBool(formats[1]);
        //             formatU.Checked = TranslateUtils.ToBool(formats[2]);
        //             formatColor.Text = formats[3];
        //             if (formatStrong.Checked || formatEm.Checked || formatU.Checked || !string.IsNullOrEmpty(formatColor.Text))
        //             {
        //                 isTitleFormatted = true;
        //             }
        //         }
        //     }
        //     return isTitleFormatted;
        // }

        public static void SetTitleFormatControls(string titleFormatString, out bool formatStrong, out bool formatEm, out bool formatU, out string formatColor)
        {
            formatStrong = formatEm = formatU = false;
            formatColor = string.Empty;

            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    formatStrong = TranslateUtils.ToBool(formats[0]);
                    formatEm = TranslateUtils.ToBool(formats[1]);
                    formatU = TranslateUtils.ToBool(formats[2]);
                    formatColor = formats[3];
                }
            }
        }

        public static string FormatTitle(string titleFormatString, string title)
        {
            var formattedTitle = title;
            if (!string.IsNullOrEmpty(titleFormatString))
            {
                var formats = titleFormatString.Split('_');
                if (formats.Length == 4)
                {
                    var isStrong = TranslateUtils.ToBool(formats[0]);
                    var isEm = TranslateUtils.ToBool(formats[1]);
                    var isU = TranslateUtils.ToBool(formats[2]);
                    var color = formats[3];

                    if (!string.IsNullOrEmpty(color))
                    {
                        if (!color.StartsWith("#"))
                        {
                            color = "#" + color;
                        }
                        formattedTitle = $@"<span style=""color:{color}"">{formattedTitle}</span>";
                    }
                    if (isStrong)
                    {
                        formattedTitle = $"<strong>{formattedTitle}</strong>";
                    }
                    if (isEm)
                    {
                        formattedTitle = $"<em>{formattedTitle}</em>";
                    }
                    if (isU)
                    {
                        formattedTitle = $"<u>{formattedTitle}</u>";
                    }
                }
            }
            return formattedTitle;
        }



        public static string GetAutoPageContent(string content, int pageWordNum)
        {
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(content))
            {
                content = content.Replace(PagePlaceHolder, string.Empty);
                AutoPage(builder, content, pageWordNum);
            }
            return builder.ToString();
        }

        private static void AutoPage(StringBuilder builder, string content, int pageWordNum)
        {
            if (content.Length > pageWordNum)
            {
                var i = content.IndexOf("</P>", pageWordNum, StringComparison.Ordinal);
                if (i == -1)
                {
                    i = content.IndexOf("</p>", pageWordNum, StringComparison.Ordinal);
                }

                if (i != -1)
                {
                    var start = i + 4;
                    builder.Append(content.Substring(0, start));
                    content = content.Substring(start);
                    if (!string.IsNullOrEmpty(content))
                    {
                        builder.Append(PagePlaceHolder);
                        AutoPage(builder, content, pageWordNum);
                    }
                }
                else
                {
                    builder.Append(content);
                }
            }
            else
            {
                builder.Append(content);
            }
        }

        public static List<TableStyle> GetAllTableStyleInfoList(List<TableStyle> tableStyleInfoList)
        {
            var taxis = 1;
            var list = new List<TableStyle>
            {
                new TableStyle
                {
                    AttributeName = ContentAttribute.Id,
                    DisplayName = "内容Id",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.Title,
                    DisplayName = "标题",
                    Taxis = taxis++
                }
            };

            if (tableStyleInfoList != null)
            {
                foreach (var tableStyleInfo in tableStyleInfoList)
                {
                    if (!list.Exists(t => t.AttributeName == tableStyleInfo.AttributeName))
                    {
                        list.Add(new TableStyle
                        {
                            AttributeName = tableStyleInfo.AttributeName,
                            DisplayName = tableStyleInfo.DisplayName,
                            Type = tableStyleInfo.Type,
                            Taxis = taxis++
                        });
                    }
                }
            }

            list.AddRange(new List<TableStyle>
            {
                new TableStyle
                {
                    AttributeName = ContentAttribute.LinkUrl,
                    DisplayName = "外部链接",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.AddDate,
                    DisplayName = "添加时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.LastModifiedDate,
                    DisplayName = "最后修改时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.GroupNameCollection,
                    DisplayName = "内容组",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.Tags,
                    DisplayName = "标签",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.UserId,
                    DisplayName = "添加人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.LastModifiedUserId,
                    DisplayName = "最后修改人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.SourceId,
                    DisplayName = "来源标识",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.Hits,
                    DisplayName = "点击量",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.HitsByDay,
                    DisplayName = "日点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.HitsByWeek,
                    DisplayName = "周点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.HitsByMonth,
                    DisplayName = "月点击",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.LastHitsDate,
                    DisplayName = "最后点击时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.Downloads,
                    DisplayName = "下载量",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.CheckUserId,
                    DisplayName = "审核人",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.CheckDate,
                    DisplayName = "审核时间",
                    Taxis = taxis++
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.CheckReasons,
                    DisplayName = "审核原因",
                    Taxis = taxis
                },
            });

            return list.OrderBy(styleInfo => styleInfo.Taxis == 0 ? int.MaxValue : styleInfo.Taxis).ToList();
        }

        public static List<TableStyle> GetEditableTableStyleInfoList(List<TableStyle> tableStyleInfoList)
        {
            var list = new List<TableStyle>
            {
                new TableStyle
                {
                    AttributeName = ContentAttribute.Title,
                    Type = InputType.Text,
                    DisplayName = "标题"
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.LinkUrl,
                    Type = InputType.Text,
                    DisplayName = "外部链接"
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.AddDate,
                    Type = InputType.DateTime,
                    DisplayName = "添加时间"
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.GroupNameCollection,
                    Type = InputType.CheckBox,
                    DisplayName = "内容组"
                },
                new TableStyle
                {
                    AttributeName = ContentAttribute.Tags,
                    Type = InputType.CheckBox,
                    DisplayName = "标签"
                }
            };

            if (tableStyleInfoList != null)
            {
                list.InsertRange(2, tableStyleInfoList);
            }

            return list;
        }

        public static async Task<bool> AfterContentAddedAsync(CrossSiteTransManager crossSiteTransManager, IPluginManager pluginManager, ISiteRepository siteRepository, IErrorLogRepository errorLogRepository, Site siteInfo, Channel channelInfo, int contentId, bool isCrossSiteTrans, bool isAutomatic)
        {
            var isTranslated = false;
            if (isCrossSiteTrans && isAutomatic)
            {
                var targetSiteId = 0;
                var transType = ECrossSiteTransTypeUtils.GetEnumType(channelInfo.TransType);

                if (transType == ECrossSiteTransType.SpecifiedSite)
                {
                    targetSiteId = channelInfo.TransSiteId;
                }
                else if (transType == ECrossSiteTransType.SelfSite)
                {
                    targetSiteId = siteInfo.Id;
                }
                else if (transType == ECrossSiteTransType.ParentSite)
                {
                    targetSiteId = await siteRepository.GetParentSiteIdAsync(siteInfo.Id);
                }

                if (targetSiteId > 0)
                {
                    var targetSiteInfo = await siteRepository.GetSiteAsync(targetSiteId);
                    if (targetSiteInfo != null)
                    {
                        var targetChannelIdArrayList = TranslateUtils.StringCollectionToIntList(channelInfo.TransChannelIds);
                        if (targetChannelIdArrayList.Count > 0)
                        {
                            foreach (var targetChannelId in targetChannelIdArrayList)
                            {
                                await crossSiteTransManager.TransContentInfoAsync(siteInfo, channelInfo, contentId, targetSiteInfo, targetChannelId);
                                isTranslated = true;
                            }
                        }
                    }
                }
            }

            foreach (var service in await pluginManager.GetServicesAsync())
            {
                try
                {
                    service.OnContentAddCompleted(new ContentEventArgs(siteInfo.Id, channelInfo.Id, contentId));
                }
                catch (Exception ex)
                {
                    await errorLogRepository.AddErrorLogAsync(service.PluginId, ex, nameof(service.OnContentAddCompleted));
                }
            }

            return isTranslated;
        }

        public static Dictionary<int, List<int>> GetIDsDictionary(NameValueCollection queryString)
        {
            var dic = new Dictionary<int, List<int>>();

            if (!string.IsNullOrEmpty(queryString["IDsCollection"]))
            {
                foreach (var ids in TranslateUtils.StringCollectionToStringList(queryString["IDsCollection"]))
                {
                    var channelId = TranslateUtils.ToIntWithNegative(ids.Split('_')[0]);
                    var contentId = TranslateUtils.ToInt(ids.Split('_')[1]);
                    var contentIdList = new List<int>();
                    if (dic.ContainsKey(channelId))
                    {
                        contentIdList = dic[channelId];
                    }
                    if (!contentIdList.Contains(contentId))
                    {
                        contentIdList.Add(contentId);
                    }

                    dic[channelId] = contentIdList;
                }
            }
            else
            {
                var channelId = TranslateUtils.ToInt(queryString["channelId"]);
                dic[channelId] = TranslateUtils.StringCollectionToIntList(queryString["contentIdCollection"]);
            }

            return dic;
        }

        public static string GetTitleHtml(string titleFormat)
        {
            var builder = new StringBuilder();
            var formatStrong = false;
            var formatEm = false;
            var formatU = false;
            var formatColor = string.Empty;
            if (titleFormat != null)
            {
                SetTitleFormatControls(titleFormat, out formatStrong, out formatEm, out formatU, out formatColor);
            }

            builder.Append(
                $@"<a class=""btn"" href=""javascript:;"" onclick=""$('#div_{ContentAttribute.Title}').toggle();return false;""><i class=""icon-text-height""></i></a>
<script type=""text/javascript"">
function {ContentAttribute.Title}_strong(e){{
var e = $(e);
if ($('#{ContentAttribute.Title}_formatStrong').val() == 'true'){{
$('#{ContentAttribute.Title}_formatStrong').val('false');
e.removeClass('btn-success');
}}else{{
$('#{ContentAttribute.Title}_formatStrong').val('true');
e.addClass('btn-success');
}}
}}
function {ContentAttribute.Title}_em(e){{
var e = $(e);
if ($('#{ContentAttribute.Title}_formatEM').val() == 'true'){{
$('#{ContentAttribute.Title}_formatEM').val('false');
e.removeClass('btn-success');
}}else{{
$('#{ContentAttribute.Title}_formatEM').val('true');
e.addClass('btn-success');
}}
}}
function {ContentAttribute.Title}_u(e){{
var e = $(e);
if ($('#{ContentAttribute.Title}_formatU').val() == 'true'){{
$('#{ContentAttribute.Title}_formatU').val('false');
e.removeClass('btn-success');
}}else{{
$('#{ContentAttribute.Title}_formatU').val('true');
e.addClass('btn-success');
}}
}}
function {ContentAttribute.Title}_color(){{
if ($('#{ContentAttribute.Title}_formatColor').val()){{
$('#{ContentAttribute.Title}_colorBtn').css('color', $('#{ContentAttribute.Title}_formatColor').val());
$('#{ContentAttribute.Title}_colorBtn').addClass('btn-success');
}}else{{
$('#{ContentAttribute.Title}_colorBtn').css('color', '');
$('#{ContentAttribute.Title}_colorBtn').removeClass('btn-success');
}}
$('#{ContentAttribute.Title}_colorContainer').hide();
}}
</script>
");

            builder.Append($@"
<div class=""btn-group btn-group-sm"" style=""float:left;"">
    <button class=""btn{(formatStrong ? @" btn-success" : string.Empty)}"" style=""font-weight:bold"" onclick=""{ContentAttribute.Title}_strong(this);return false;"">粗体</button>
    <button class=""btn{(formatEm ? " btn-success" : string.Empty)}"" style=""font-style:italic"" onclick=""{ContentAttribute.Title}_em(this);return false;"">斜体</button>
    <button class=""btn{(formatU ? " btn-success" : string.Empty)}"" style=""text-decoration:underline"" onclick=""{ContentAttribute.Title}_u(this);return false;"">下划线</button>
    <button class=""btn{(!string.IsNullOrEmpty(formatColor) ? " btn-success" : string.Empty)}"" id=""{ContentAttribute.Title}_colorBtn"" onclick=""$('#{ContentAttribute.Title}_colorContainer').toggle();return false;"">颜色</button>
</div>
<div id=""{ContentAttribute.Title}_colorContainer"" class=""input-append"" style=""float:left;display:none"">
    <input id=""{ContentAttribute.Title}_formatColor"" name=""{ContentAttribute.Title}_formatColor"" class=""input-mini color {{required:false}}"" type=""text"" value=""{formatColor}"" placeholder=""颜色值"">
    <button class=""btn"" type=""button"" onclick=""Title_color();return false;"">确定</button>
</div>
<input id=""{ContentAttribute.Title}_formatStrong"" name=""{ContentAttribute.Title}_formatStrong"" type=""hidden"" value=""{formatStrong.ToString().ToLower()}"" />
<input id=""{ContentAttribute.Title}_formatEM"" name=""{ContentAttribute.Title}_formatEM"" type=""hidden"" value=""{formatEm.ToString().ToLower()}"" />
<input id=""{ContentAttribute.Title}_formatU"" name=""{ContentAttribute.Title}_formatU"" type=""hidden"" value=""{formatU.ToString().ToLower()}"" />
");

            return builder.ToString();
        }

        public static string GetTagsHtml(string tagsAjaxUrl)
        {
            const string tagScript = @"
<script type=""text/javascript"">
function getTags(tag){
	$.get('[url]&tag=' + encodeURIComponent(tag) + '&r=' + Math.random(), function(data) {
		if(data !=''){
			var arr = data.split('|');
			var temp='';
			for(i=0;i<arr.length;i++)
			{
				temp += '<li><a>'+arr[i].replace(tag,'<b>' + tag + '</b>') + '</a></li>';
			}
			var myli='<ul>'+temp+'</ul>';
			$('#tagTips').html(myli);
			$('#tagTips').show();
		}else{
            $('#tagTips').hide();
        }
		$('#tagTips li').click(function () {
			var tag = $('#TbTags').val();
			var i = tag.lastIndexOf(' ');
			if (i > 0)
			{
				tag = tag.substring(0, i) + ' ' + $(this).text();
			}else{
				tag = $(this).text();	
			}
			$('#TbTags').val(tag);
			$('#tagTips').hide();
		})
	});	
}
$(document).ready(function () {
$('#TbTags').keyup(function (e) {
    if (e.keyCode != 40 && e.keyCode != 38) {
        var tag = $('#TbTags').val();
		var i = tag.lastIndexOf(' ');
		if (i > 0){ tag = tag.substring(i + 1);}
        if (tag != '' && tag != ' '){
            window.setTimeout(""getTags('"" + tag + ""');"", 200);
        }else{
            $('#tagTips').hide();
        }
    }
}).blur(function () {
	window.setTimeout(""$('#tagTips').hide();"", 200);
})});
</script>
<div id=""tagTips"" class=""inputTips""></div>
";
            return tagScript.Replace("[url]", tagsAjaxUrl);
        }
    }
}
