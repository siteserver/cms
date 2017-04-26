using System.Collections.Specialized;

namespace SiteServer.CMS.StlParser.StlElement.Inner
{
    //列表项
    public class StlItem
	{
        public const string ElementName = "stl:item";
        public const string ElementName2 = "stl:itemtemplate";

        public const string Attribute_Type = "type";                        //内容项类型
        public const string Attribute_Selected = "selected";                //内容当前选定项类型
        public const string Attribute_SelectedValue = "selectedvalue";      //内容当前选定项的值

        public const string Type_Header = "header";                 //为 stl:contents 中的项提供头部内容
        public const string Type_Footer = "footer";                 //为 stl:contents 中的项提供底部内容
        public const string Type_Item = "item";                             //为 stl:contents 中的项提供内容和布局
        public const string Type_AlternatingItem = "alternatingitem";       //为 stl:contents 中的交替项提供内容和布局
        public const string Type_SelectedItem = "selecteditem";             //为 stl:contents 中当前选定项提供内容和布局
        public const string Type_Separator = "separator";                   //为 stl:contents 中各项之间的分隔符提供内容和布局

        public class ContentsItem
        {
            public const string Selected_Current = "current";                   //当前内容为选中内容
            public const string Selected_Image = "image";                       //带图片内容为选中内容
            public const string Selected_Video = "video";                       //带视频内容为选中内容
            public const string Selected_File = "file";                         //带附件内容为选中内容
            public const string Selected_IsTop = "istop";                       //置顶内容为选中内容
            public const string Selected_IsRecommend = "isrecommend";           //推荐内容为选中内容
            public const string Selected_IsHot = "ishot";                       //热点内容为选中内容
            public const string Selected_IsColor = "iscolor";                   //醒目内容为选中内容
            public const string Selected_ChannelName = "channelname";           //指定栏目名称的内容
        }

        public class ChannelsItem
        {
            public const string Selected_Current = "current";                   //当前栏目为选中栏目
            public const string Selected_Image = "image";                       //带图片栏目为选中栏目
            public const string Selected_Up = "up";                             //当前栏目的上级栏目为选中栏目
            public const string Selected_Top = "top";                           //当前栏目从首页向下的栏目为选中栏目
        }

        public static ListDictionary AttributeList
        {
            get
            {
                var attributes = new ListDictionary();

                attributes.Add(Attribute_Type, "内容列表项类型");
                attributes.Add(Attribute_Selected, "内容列表当前选定项类型");
                attributes.Add(Attribute_SelectedValue, "内容当前选定项的值");
                return attributes;
            }
        }
    }
}
