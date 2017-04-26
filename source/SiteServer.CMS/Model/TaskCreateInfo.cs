using BaiRong.Core;
using BaiRong.Core.Model;

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
            get { return GetBool("IsCreateAll", false); }
            set { SetExtendedAttribute("IsCreateAll", value.ToString()); }
        }

        public string ChannelIDCollection
		{
            get { return GetExtendedAttribute("ChannelIDCollection"); }
            set { SetExtendedAttribute("ChannelIDCollection", value); }
		}

        public string CreateTypes
        {
            get { return GetExtendedAttribute("CreateTypes"); }
            set { SetExtendedAttribute("CreateTypes", value); }
        }
	}
}
