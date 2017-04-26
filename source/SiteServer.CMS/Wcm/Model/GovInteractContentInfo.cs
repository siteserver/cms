using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.GovInteract;

namespace SiteServer.CMS.Wcm.Model
{
	public class GovInteractContentInfo : ContentInfo
	{
		public GovInteractContentInfo()
		{
            DepartmentName = string.Empty;
            QueryCode = GovInteractApplyManager.GetQueryCode();
            State = EGovInteractState.New;
            IpAddress = PageUtils.GetIpAddress();
            AddDate = DateTime.Now;
		}

        public GovInteractContentInfo(object dataItem)
            : base(dataItem)
		{
		}

        public string DepartmentName
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.DepartmentName); }
            set { SetExtendedAttribute(GovInteractContentAttribute.DepartmentName, value); }
        }

        public string QueryCode
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.QueryCode); }
            set { SetExtendedAttribute(GovInteractContentAttribute.QueryCode, value); }
        }

        public EGovInteractState State
        {
            get { return EGovInteractStateUtils.GetEnumType(GetExtendedAttribute(GovInteractContentAttribute.State)); }
            set { SetExtendedAttribute(GovInteractContentAttribute.State, EGovInteractStateUtils.GetValue(value)); }
        }

        public string IpAddress
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.IpAddress); }
            set { SetExtendedAttribute(GovInteractContentAttribute.IpAddress, value); }
        }

        //basic

        public string RealName
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.RealName); }
            set { SetExtendedAttribute(GovInteractContentAttribute.RealName, value); }
        }

        public string Organization
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.Organization); }
            set { SetExtendedAttribute(GovInteractContentAttribute.Organization, value); }
        }

        public string CardType
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.CardType); }
            set { SetExtendedAttribute(GovInteractContentAttribute.CardType, value); }
        }

        public string CardNo
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.CardNo); }
            set { SetExtendedAttribute(GovInteractContentAttribute.CardNo, value); }
        }

        public string Phone
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.Phone); }
            set { SetExtendedAttribute(GovInteractContentAttribute.Phone, value); }
        }

        public string PostCode
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.PostCode); }
            set { SetExtendedAttribute(GovInteractContentAttribute.PostCode, value); }
        }

        public string Address
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.Address); }
            set { SetExtendedAttribute(GovInteractContentAttribute.Address, value); }
        }

        public string Email
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.Email); }
            set { SetExtendedAttribute(GovInteractContentAttribute.Email, value); }
        }

        public string Fax
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.Fax); }
            set { SetExtendedAttribute(GovInteractContentAttribute.Fax, value); }
        }

        public int TypeId
        {
            get { return GetInt(GovInteractContentAttribute.TypeId, 0); }
            set { SetExtendedAttribute(GovInteractContentAttribute.TypeId, value.ToString()); }
        }

        public bool IsPublic
        {
            get { return GetBool(GovInteractContentAttribute.IsPublic, true); }
            set { SetExtendedAttribute(GovInteractContentAttribute.IsPublic, value.ToString()); }
        }

        public string Content
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.Content); }
            set { SetExtendedAttribute(GovInteractContentAttribute.Content, value); }
        }

        public string FileUrl
        {
            get { return GetExtendedAttribute(GovInteractContentAttribute.FileUrl); }
            set { SetExtendedAttribute(GovInteractContentAttribute.FileUrl, value); }
        }

        public int DepartmentId
        {
            get { return GetInt(GovInteractContentAttribute.DepartmentId, 0); }
            set { SetExtendedAttribute(GovInteractContentAttribute.DepartmentId, value.ToString()); }
        }

        public override List<string> GetDefaultAttributesNames()
        {
            return GovInteractContentAttribute.AllAttributes;
        }
	}
}
