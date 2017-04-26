using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
    public class VoteContentAttribute
	{
        protected VoteContentAttribute()
		{
		}

        //hidden
        public const string IsImageVote = "IsImageVote";
        public const string IsSummary = "IsSummary";
        public const string Participants = "Participants";
        public const string IsClosed = "IsClosed";
        public const string IsTop = "IsTop";

        //system
        public const string Title = "Title";
        public const string SubTitle = "SubTitle";
        public const string MaxSelectNum = "MaxSelectNum";
        public const string ImageUrl = "ImageUrl";
        public const string Content = "Content";
        public const string Summary = "Summary";
        public const string AddDate = "AddDate";
        public const string EndDate = "EndDate";
        public const string IsVotedView = "IsVotedView";
        public const string HiddenContent = "HiddenContent";

        public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>();
                arraylist.AddRange(HiddenAttributes);
                arraylist.AddRange(SystemAttributes);
                return arraylist;
            }
        }

        private static List<string> _hiddenAttributes;
        public static List<string> HiddenAttributes => _hiddenAttributes ?? (_hiddenAttributes = new List<string>(ContentAttribute.HiddenAttributes)
        {
            IsImageVote.ToLower(),
            IsSummary.ToLower(),
            Participants.ToLower(),
            IsClosed.ToLower(),
            IsTop.ToLower()
        });

	    private static List<string> _systemAttributes;
        public static List<string> SystemAttributes => _systemAttributes ?? (_systemAttributes = new List<string>
        {
            Title.ToLower(),
            SubTitle.ToLower(),
            MaxSelectNum.ToLower(),
            ImageUrl.ToLower(),
            Content.ToLower(),
            Summary.ToLower(),
            AddDate.ToLower(),
            EndDate.ToLower(),
            IsVotedView.ToLower(),
            HiddenContent.ToLower()
        });

	    private static List<string> _excludeAttributes;
        public static List<string> ExcludeAttributes => _excludeAttributes ?? (_excludeAttributes = new List<string>(ContentAttribute.ExcludeAttributes)
        {
            SubTitle.ToLower(),
            MaxSelectNum.ToLower(),
            AddDate.ToLower(),
            EndDate.ToLower(),
            IsVotedView.ToLower(),
            HiddenContent.ToLower()
        });
	}
}
