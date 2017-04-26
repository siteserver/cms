using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.CMS.Model
{
	public class ResumeContentInfo : ExtendedAttributes
	{
		public ResumeContentInfo()
		{
			Id = 0;
            PublishmentSystemId = 0;
            JobContentId = 0;
            UserName = string.Empty;
			AddDate = DateTime.Now;
		}

        public ResumeContentInfo(int id, int publishmentSystemId, int jobContentId, string userName, DateTime addDate)
		{
			Id = id;
            PublishmentSystemId = publishmentSystemId;
            JobContentId = jobContentId;
            UserName = userName;
			AddDate = addDate;
		}

		public int Id
		{
			get { return GetInt(ResumeContentAttribute.Id, 0); }
            set { SetExtendedAttribute(ResumeContentAttribute.Id, value.ToString()); }
		}

        public int PublishmentSystemId
        {
            get { return GetInt(ResumeContentAttribute.PublishmentSystemId, 0); }
            set { SetExtendedAttribute(ResumeContentAttribute.PublishmentSystemId, value.ToString()); }
        }

        public int JobContentId
        {
            get { return GetInt(ResumeContentAttribute.JobContentId, 0); }
            set { SetExtendedAttribute(ResumeContentAttribute.JobContentId, value.ToString()); }
        }

        public string UserName
        {
            get { return GetExtendedAttribute(ResumeContentAttribute.UserName); }
            set { SetExtendedAttribute(ResumeContentAttribute.UserName, value); }
        }

        public bool IsView
        {
            get { return GetBool(ResumeContentAttribute.IsView, false); }
            set { SetExtendedAttribute(ResumeContentAttribute.IsView, value.ToString()); }
        }

        public DateTime AddDate
        {
            get { return GetDateTime(ResumeContentAttribute.AddDate, DateTime.Now); }
            set { SetExtendedAttribute(ResumeContentAttribute.AddDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public override List<string> GetDefaultAttributesNames()
        {
            return ResumeContentAttribute.AllAttributes;
        }
	}
}
