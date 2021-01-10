using SSCMS.Core.StlParser.Attributes;

namespace SSCMS.Core.StlParser.Models
{
    public class StlListBase
    {
        [StlAttribute(Title = "栏目索引")]
        public const string ChannelIndex = nameof(ChannelIndex);

        [StlAttribute(Title = "栏目索引")]
        public const string Index = nameof(Index);

        [StlAttribute(Title = "栏目名称")]
        public const string ChannelName = nameof(ChannelName);

        [StlAttribute(Title = "上级栏目")]
        public const string Parent = nameof(Parent);

        [StlAttribute(Title = "上级栏目的级别")]
        public const string UpLevel = nameof(UpLevel);

        [StlAttribute(Title = "从首页向下的栏目级别")]
        public const string TopLevel = nameof(TopLevel);

        [StlAttribute(Title = "范围")]
        public const string Scope = nameof(Scope);

        [StlAttribute(Title = "指定组")]
        public const string Group = nameof(Group);

        [StlAttribute(Title = "排除组")]
        public const string GroupNot = nameof(GroupNot);

        [StlAttribute(Title = "指定栏目组")]
        public const string GroupChannel = nameof(GroupChannel);

        [StlAttribute(Title = "排除栏目组")]
        public const string GroupChannelNot = nameof(GroupChannelNot);

        [StlAttribute(Title = "指定内容组")]
        public const string GroupContent = nameof(GroupContent);

        [StlAttribute(Title = "排除内容组")]
        public const string GroupContentNot = nameof(GroupContentNot);

        [StlAttribute(Title = "指定标签")]
        public const string Tags = nameof(Tags);

        [StlAttribute(Title = "仅显示置顶内容")]
        public const string IsTop = nameof(IsTop);

        [StlAttribute(Title = "仅显示推荐内容")]
        public const string IsRecommend = nameof(IsRecommend);

        [StlAttribute(Title = "仅显示热点内容")]
        public const string IsHot = nameof(IsHot);

        [StlAttribute(Title = "仅显示醒目内容")]
        public const string IsColor = nameof(IsColor);

        [StlAttribute(Title = "显示信息总数")]
        public const string TotalNum = nameof(TotalNum);

        [StlAttribute(Title = "从第几条信息开始显示")]
        public const string StartNum = nameof(StartNum);

        [StlAttribute(Title = "排序")]
        public const string Order = nameof(Order);

        [StlAttribute(Title = "条件")]
        public const string Where = nameof(Where);

        [StlAttribute(Title = "仅显示图片内容")]
        public const string IsImage = nameof(IsImage);

        [StlAttribute(Title = "仅显示视频内容")]
        public const string IsVideo = nameof(IsVideo);

        [StlAttribute(Title = "仅显示附件内容")]
        public const string IsFile = nameof(IsFile);

        [StlAttribute(Title = "布局")]
        public const string Layout = nameof(Layout);

        [StlAttribute(Title = "列数")]
        public const string Columns = nameof(Columns);

        [StlAttribute(Title = "方向")]
        public const string Direction = nameof(Direction);

        [StlAttribute(Title = "整体对齐")]
        public const string Align = nameof(Align);

        [StlAttribute(Title = "项水平对齐")]
        public const string ItemAlign = nameof(ItemAlign);

        [StlAttribute(Title = "项垂直对齐")]
        public const string ItemVerticalAlign = nameof(ItemVerticalAlign);

        [StlAttribute(Title = "项Css类")]
        public const string ItemClass = nameof(ItemClass);
    }
}
