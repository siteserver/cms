using BaiRong.Core;
using BaiRong.Core.Model;

namespace SiteServer.CMS.Model
{
	public class TaskGatherInfo : ExtendedAttributes
	{
        public TaskGatherInfo(string serviceParameters)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(serviceParameters);
            SetExtendedAttribute(nameValueCollection);
        }

        public string PublishmentSystemIDCollection
        {
            get { return GetExtendedAttribute("PublishmentSystemIDCollection"); }
            set { SetExtendedAttribute("PublishmentSystemIDCollection", value); }
        }

        public string WebGatherNames
		{
            get { return GetExtendedAttribute("WebGatherNames"); }
            set { SetExtendedAttribute("WebGatherNames", value); }
		}

        public string DatabaseGatherNames
        {
            get { return GetExtendedAttribute("DatabaseGatherNames"); }
            set { SetExtendedAttribute("DatabaseGatherNames", value); }
        }

        public string FileGatherNames
        {
            get { return GetExtendedAttribute("FileGatherNames"); }
            set { SetExtendedAttribute("FileGatherNames", value); }
        }
	}
}
