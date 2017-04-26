using System.Collections.Generic;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.CMS.Model
{
	public class JobContentInfo : ContentInfo
	{
		public JobContentInfo()
		{
            Department = string.Empty;
            Location = string.Empty;
            NumberOfPeople = string.Empty;
            IsUrgent = false;
            Responsibility = string.Empty;
            Requirement = string.Empty;
		}

        public JobContentInfo(object dataItem)
            : base(dataItem)
		{
		}

        public string Department
		{
            get { return GetExtendedAttribute(JobContentAttribute.Department); }
            set { SetExtendedAttribute(JobContentAttribute.Department, value); }
		}

        public string Location
		{
            get { return GetExtendedAttribute(JobContentAttribute.Location); }
            set { SetExtendedAttribute(JobContentAttribute.Location, value); }
		}

        public string NumberOfPeople
		{
            get { return GetExtendedAttribute(JobContentAttribute.NumberOfPeople); }
            set { SetExtendedAttribute(JobContentAttribute.NumberOfPeople, value); }
		}

        public bool IsUrgent
		{
            get { return GetBool(JobContentAttribute.IsUrgent, false); }
            set { SetExtendedAttribute(JobContentAttribute.IsUrgent, value.ToString()); }
		}

        public string Responsibility
        {
            get { return GetExtendedAttribute(JobContentAttribute.Responsibility); }
            set { SetExtendedAttribute(JobContentAttribute.Responsibility, value); }
        }

        public string Requirement
        {
            get { return GetExtendedAttribute(JobContentAttribute.Requirement); }
            set { SetExtendedAttribute(JobContentAttribute.Requirement, value); }
        }

        public override List<string> GetDefaultAttributesNames()
        {
            return JobContentAttribute.AllAttributes;
        }
	}
}
