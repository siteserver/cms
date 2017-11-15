using BaiRong.Core;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
	public class TaskCreateInfo : ExtendedAttributes
	{
        public TaskCreateInfo(string serviceParameters)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(serviceParameters);
            SetExtendedAttribute(nameValueCollection);
        }

        public bool IsCreateAll
        {
            get { return GetBool("IsCreateAll"); }
            set { SetExtendedAttribute("IsCreateAll", value.ToString()); }
        }

        public string ChannelIdCollection
		{
            get { return GetExtendedAttribute("ChannelIdCollection"); }
            set { SetExtendedAttribute("ChannelIdCollection", value); }
		}

        public string CreateTypes
        {
            get { return GetExtendedAttribute("CreateTypes"); }
            set { SetExtendedAttribute("CreateTypes", value); }
        }
	}
}
