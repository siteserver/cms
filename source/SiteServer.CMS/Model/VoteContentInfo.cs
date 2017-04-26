using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.CMS.Model
{
	public class VoteContentInfo : ContentInfo
	{
		public VoteContentInfo()
		{
            IsImageVote = false;
            IsSummary = false;
            Participants = 0;
            IsClosed = false;
            SubTitle = string.Empty;
            MaxSelectNum = 0;
            ImageUrl = string.Empty;
            Content = string.Empty;
            Summary = string.Empty;
            EndDate = DateTime.Now;
            IsVotedView = false;
            HiddenContent = string.Empty;
		}

        public VoteContentInfo(object dataItem)
            : base(dataItem)
		{
		}

        public bool IsImageVote
        {
            get { return GetBool(VoteContentAttribute.IsImageVote, false); }
            set { SetExtendedAttribute(VoteContentAttribute.IsImageVote, value.ToString()); }
        }

        public bool IsSummary
        {
            get { return GetBool(VoteContentAttribute.IsSummary, false); }
            set { SetExtendedAttribute(VoteContentAttribute.IsSummary, value.ToString()); }
        }

        public int Participants
		{
            get { return GetInt(VoteContentAttribute.Participants, 0); }
            set { SetExtendedAttribute(VoteContentAttribute.Participants, value.ToString()); }
		}

        public bool IsClosed
        {
            get { return GetBool(VoteContentAttribute.IsClosed, false); }
            set { SetExtendedAttribute(VoteContentAttribute.IsClosed, value.ToString()); }
        }

        public string SubTitle
		{
            get { return GetExtendedAttribute(VoteContentAttribute.SubTitle); }
            set { SetExtendedAttribute(VoteContentAttribute.SubTitle, value); }
		}

        public int MaxSelectNum
        {
            get { return GetInt(VoteContentAttribute.MaxSelectNum, 0); }
            set { SetExtendedAttribute(VoteContentAttribute.MaxSelectNum, value.ToString()); }
        }

        public string ImageUrl
        {
            get { return GetExtendedAttribute(VoteContentAttribute.ImageUrl); }
            set { SetExtendedAttribute(VoteContentAttribute.ImageUrl, value); }
        }

        public string Content
        {
            get { return GetExtendedAttribute(VoteContentAttribute.Content); }
            set { SetExtendedAttribute(VoteContentAttribute.Content, value); }
        }

        public string Summary
        {
            get { return GetExtendedAttribute(VoteContentAttribute.Summary); }
            set { SetExtendedAttribute(VoteContentAttribute.Summary, value); }
        }

        public DateTime EndDate
        {
            get { return GetDateTime(VoteContentAttribute.EndDate, DateTime.Now); }
            set { SetExtendedAttribute(VoteContentAttribute.EndDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public bool IsVotedView
        {
            get { return GetBool(VoteContentAttribute.IsVotedView, false); }
            set { SetExtendedAttribute(VoteContentAttribute.IsVotedView, value.ToString()); }
        }

        public string HiddenContent
        {
            get { return GetExtendedAttribute(VoteContentAttribute.HiddenContent); }
            set { SetExtendedAttribute(VoteContentAttribute.HiddenContent, value); }
        }

        public override List<string> GetDefaultAttributesNames()
        {
            return VoteContentAttribute.AllAttributes;
        }
	}
}
