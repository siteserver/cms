using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.API.Model
{
    public class Comment
    {
        public Comment(CommentInfo commentInfo, UserInfo userInfo)
        {
            if (commentInfo == null) return;

            Id = commentInfo.Id;
            AddDate = DateUtils.GetDateAndTimeString(commentInfo.AddDate, EDateFormatType.Chinese, ETimeFormatType.ShortTime);
            GoodCount = commentInfo.GoodCount;
            Content = commentInfo.Content;
            IsChecked = commentInfo.IsChecked;
            DisplayName = userInfo?.DisplayName;
            AvatarUrl = PageUtility.GetUserAvatarUrl(PageUtils.GetApiUrl(), userInfo);
        }

        public int Id { get; set; }

        public string AddDate { get; set; }

        public int GoodCount { get; set; }

        public string Content { get; set; }

        public bool IsChecked { get; set; }

        public string DisplayName { get; set; }

        public string AvatarUrl { get; set; }
    }
}