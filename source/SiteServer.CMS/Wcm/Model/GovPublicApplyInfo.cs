using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Wcm.Model
{
    public class GovPublicApplyAttribute
    {
        protected GovPublicApplyAttribute()
        {
        }

        //hidden
        public const string Id = "ID";
        public const string StyleId = "StyleID";
        public const string PublishmentSystemId = "PublishmentSystemID";
        public const string IsOrganization = "IsOrganization";
        public const string Title = "Title";
        public const string DepartmentName = "DepartmentName";
        public const string DepartmentId = "DepartmentID";
        public const string AddDate = "AddDate";
        public const string QueryCode = "QueryCode";
        public const string State = "State";

        //basic
        public const string CivicName = "CivicName";
        public const string CivicOrganization = "CivicOrganization";
        public const string CivicCardType = "CivicCardType";
        public const string CivicCardNo = "CivicCardNo";
        public const string CivicPhone = "CivicPhone";
        public const string CivicPostCode = "CivicPostCode";
        public const string CivicAddress = "CivicAddress";
        public const string CivicEmail = "CivicEmail";
        public const string CivicFax = "CivicFax";
        public const string OrgName = "OrgName";
        public const string OrgUnitCode = "OrgUnitCode";
        public const string OrgLegalPerson = "OrgLegalPerson";
        public const string OrgLinkName = "OrgLinkName";
        public const string OrgPhone = "OrgPhone";
        public const string OrgPostCode = "OrgPostCode";
        public const string OrgAddress = "OrgAddress";
        public const string OrgEmail = "OrgEmail";
        public const string OrgFax = "OrgFax";
        public const string Content = "Content";
        public const string Purpose = "Purpose";
        public const string IsApplyFree = "IsApplyFree";
        public const string ProvideType = "ProvideType";
        public const string ObtainType = "ObtainType";

        public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>(HiddenAttributes);
                arraylist.AddRange(BasicAttributes);
                return arraylist;
            }
        }

        private static List<string> _hiddenAttributes;
        public static List<string> HiddenAttributes => _hiddenAttributes ?? (_hiddenAttributes = new List<string>
        {
            Id.ToLower(),
            StyleId.ToLower(),
            PublishmentSystemId.ToLower(),
            IsOrganization.ToLower(),
            Title.ToLower(),
            DepartmentName.ToLower(),
            DepartmentId.ToLower(),
            AddDate.ToLower(),
            QueryCode.ToLower(),
            State.ToLower()
        });

        private static List<string> _basicAttributes;
        public static List<string> BasicAttributes => _basicAttributes ?? (_basicAttributes = new List<string>
        {
            CivicName.ToLower(),
            CivicOrganization.ToLower(),
            CivicCardType.ToLower(),
            CivicCardNo.ToLower(),
            CivicPhone.ToLower(),
            CivicPostCode.ToLower(),
            CivicAddress.ToLower(),
            CivicEmail.ToLower(),
            CivicFax.ToLower(),
            OrgName.ToLower(),
            OrgUnitCode.ToLower(),
            OrgLegalPerson.ToLower(),
            OrgLinkName.ToLower(),
            OrgPhone.ToLower(),
            OrgPostCode.ToLower(),
            OrgAddress.ToLower(),
            OrgEmail.ToLower(),
            OrgFax.ToLower(),
            Content.ToLower(),
            Purpose.ToLower(),
            IsApplyFree.ToLower(),
            ProvideType.ToLower(),
            ObtainType.ToLower()
        });
    }

    public class GovPublicApplyInfo : ExtendedAttributes
    {
        public GovPublicApplyInfo()
        {
            Id = 0;
            StyleId = 0;
            PublishmentSystemId = 0;
            IsOrganization = false;
            Title = string.Empty;
            DepartmentName = string.Empty;
            DepartmentId = 0;
            AddDate = DateTime.Now;
            QueryCode = string.Empty;
            State = EGovPublicApplyState.New;
        }

        public GovPublicApplyInfo(object dataItem)
            : base(dataItem)
		{
		}

        public GovPublicApplyInfo(int id, int styleId, int publishmentSystemId, bool isOrganization, string title, string departmentName, int departmentId, DateTime addDate, string queryCode, EGovPublicApplyState state)
        {
            Id = id;
            StyleId = styleId;
            PublishmentSystemId = publishmentSystemId;
            IsOrganization = isOrganization;
            Title = title;
            DepartmentName = departmentName;
            DepartmentId = departmentId;
            AddDate = addDate;
            QueryCode = queryCode;
            State = state;
        }

        public int Id
        {
            get { return GetInt(GovPublicApplyAttribute.Id, 0); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.Id, value.ToString()); }
        }

        public int StyleId
        {
            get { return GetInt(GovPublicApplyAttribute.StyleId, 0); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.StyleId, value.ToString()); }
        }

        public int PublishmentSystemId
        {
            get { return GetInt(GovPublicApplyAttribute.PublishmentSystemId, 0); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.PublishmentSystemId, value.ToString()); }
        }

        public bool IsOrganization
        {
            get { return GetBool(GovPublicApplyAttribute.IsOrganization, false); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.IsOrganization, value.ToString()); }
        }

        public string Title
        {
            get { return GetExtendedAttribute(GovPublicApplyAttribute.Title); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.Title, value); }
        }

        public string DepartmentName
        {
            get { return GetExtendedAttribute(GovPublicApplyAttribute.DepartmentName); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.DepartmentName, value); }
        }

        public int DepartmentId
        {
            get { return GetInt(GovPublicApplyAttribute.DepartmentId, 0); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.DepartmentId, value.ToString()); }
        }

        public DateTime AddDate
        {
            get { return GetDateTime(GovPublicApplyAttribute.AddDate, DateTime.Now); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.AddDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public string QueryCode
        {
            get { return GetExtendedAttribute(GovPublicApplyAttribute.QueryCode); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.QueryCode, value); }
        }

        public EGovPublicApplyState State
        {
            get { return EGovPublicApplyStateUtils.GetEnumType(GetExtendedAttribute(GovPublicApplyAttribute.State)); }
            set { SetExtendedAttribute(GovPublicApplyAttribute.State, EGovPublicApplyStateUtils.GetValue(value)); }
        }

        public override List<string> GetDefaultAttributesNames()
        {
            return GovPublicApplyAttribute.AllAttributes;
        }
    }
}
