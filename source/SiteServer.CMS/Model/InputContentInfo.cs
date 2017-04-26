using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.CMS.Model
{
	public class InputContentInfo : ExtendedAttributes
	{
		public InputContentInfo()
		{
			Id = 0;
			InputId = 0;
			Taxis = 0;
			IsChecked = false;
            UserName = string.Empty;
            IpAddress = string.Empty;
			AddDate = DateTime.Now;
            Reply = string.Empty;
		}

        public InputContentInfo(int id, int inputId, int taxis, bool isChecked, string userName, string ipAddress, DateTime addDate, string reply)
		{
			Id = id;
            InputId = inputId;
			Taxis = taxis;
			IsChecked = isChecked;
            UserName = userName;
            IpAddress = ipAddress;
			AddDate = addDate;
            Reply = reply;
		}

		public int Id
		{
			get { return GetInt(InputContentAttribute.Id, 0); }
            set { SetExtendedAttribute(InputContentAttribute.Id, value.ToString()); }
		}

        public int InputId
		{
            get { return GetInt(InputContentAttribute.InputId, 0); }
            set { SetExtendedAttribute(InputContentAttribute.InputId, value.ToString()); }
		}

		public int Taxis
		{
            get { return GetInt(InputContentAttribute.Taxis, 0); }
            set { SetExtendedAttribute(InputContentAttribute.Taxis, value.ToString()); }
		}

		public bool IsChecked
		{
            get { return GetBool(InputContentAttribute.IsChecked, false); }
            set { SetExtendedAttribute(InputContentAttribute.IsChecked, value.ToString()); }
		}

        public string UserName
        {
            get { return GetExtendedAttribute(InputContentAttribute.UserName); }
            set { SetExtendedAttribute(InputContentAttribute.UserName, value); }
        }

        public string IpAddress
        {
            get { return GetExtendedAttribute(InputContentAttribute.IpAddress); }
            set { SetExtendedAttribute(InputContentAttribute.IpAddress, value); }
        }

        public DateTime AddDate
        {
            get { return GetDateTime(InputContentAttribute.AddDate, DateTime.Now); }
            set { SetExtendedAttribute(InputContentAttribute.AddDate, DateUtils.GetDateAndTimeString(value)); }
        }

        public string Reply
        {
            get { return GetExtendedAttribute(InputContentAttribute.Reply); }
            set { SetExtendedAttribute(InputContentAttribute.Reply, value); }
        }

        public override List<string> GetDefaultAttributesNames()
        {
            return InputContentAttribute.AllAttributes;
        }
	}
}
