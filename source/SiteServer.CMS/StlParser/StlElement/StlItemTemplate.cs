using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.CMS.StlParser.StlElement
{
    [Stl(Usage = "列表项", Description = "通过 stl:itemTemplate 标签在模板中控制列表中每一项的显示内容及样式")]
    public class StlItemTemplate
	{
        public const string ElementName = "stl:itemTemplate";

        public const string AttributeType = "type";
        public const string AttributeSelected = "selected";
        public const string AttributeSelectedValue = "selectedValue";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
            {AttributeType, StringUtils.SortedListToAttributeValueString("列表项类型", TypeList)},
            {AttributeSelected, StringUtils.SortedListToAttributeValueString("列表当前选定项类型", SelectedList)},
            {AttributeSelectedValue, "当前选定项的值"}
        };

        public const string TypeHeader = "header";
        public const string TypeFooter = "footer";
        public const string TypeItem = "item";
        public const string TypeAlternatingItem = "alternatingItem";
        public const string TypeSelectedItem = "selectedItem";
        public const string TypeSeparator = "separator";

        public static SortedList<string, string> TypeList => new SortedList<string, string>
        {
            {TypeHeader, "为列表提供头部模板"},
            {TypeFooter, "为列表提供底部模板"},
            {TypeItem, "为列表的项提供模板"},
            {TypeAlternatingItem, "为列表的交替项提供模板"},
            {TypeSelectedItem, "为列表当前选定项提供模板"},
            {TypeSeparator, "为列表各项之间的分隔符提供模板"}
        };

        public const string SelectedCurrent = "current";
        public const string SelectedImage = "image";
        public const string SelectedVideo = "video";
        public const string SelectedFile = "file";
        public const string SelectedIsTop = "isTop";
        public const string SelectedIsRecommend = "isRecommend";
        public const string SelectedIsHot = "isHot";
        public const string SelectedIsColor = "isColor";
        public const string SelectedChannelName = "channelName";
        public const string SelectedUp = "up";
        public const string SelectedTop = "top";

        public static SortedList<string, string> SelectedList => new SortedList<string, string>
        {
            {SelectedCurrent, "当前项为选中项"},
            {SelectedImage, "带图片项为选中项"},
            {SelectedVideo, "带视频项为选中项"},
            {SelectedFile, "带附件项为选中项"},
            {SelectedIsTop, "置顶项为选中项"},
            {SelectedIsRecommend, "推荐项为选中项"},
            {SelectedIsHot, "热点项为选中项"},
            {SelectedIsColor, "醒目项为选中项"},
            {SelectedChannelName, "指定栏目名称的项"},
            {SelectedUp, "当前栏目的上级栏目为选中项"},
            {SelectedTop, "当前栏目从首页向下的栏目为选中项"}
        };
    }
}
