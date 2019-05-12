using System.Web.UI.WebControls;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class TranslateUtilsEx
    {
        public static Unit ToUnit(string unitStr)
        {
            var type = Unit.Empty;
            try
            {
                type = Unit.Parse(unitStr.Trim());
            }
            catch
            {
                // ignored
            }
            return type;
        }

        public static HorizontalAlign ToHorizontalAlign(string typeStr)
        {
            return (HorizontalAlign)TranslateUtils.ToEnum(typeof(HorizontalAlign), typeStr, HorizontalAlign.Left);
        }

        public static VerticalAlign ToVerticalAlign(string typeStr)
        {
            return (VerticalAlign)TranslateUtils.ToEnum(typeof(VerticalAlign), typeStr, VerticalAlign.Middle);
        }

        public static GridLines ToGridLines(string typeStr)
        {
            return (GridLines)TranslateUtils.ToEnum(typeof(GridLines), typeStr, GridLines.None);
        }

        public static RepeatDirection ToRepeatDirection(string typeStr)
        {
            return (RepeatDirection)TranslateUtils.ToEnum(typeof(RepeatDirection), typeStr, RepeatDirection.Vertical);
        }

        public static RepeatLayout ToRepeatLayout(string typeStr)
        {
            return (RepeatLayout)TranslateUtils.ToEnum(typeof(RepeatLayout), typeStr, RepeatLayout.Table);
        }
    }
}
