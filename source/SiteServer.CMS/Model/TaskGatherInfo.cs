using BaiRong.Core;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
	public class TaskGatherInfo : ExtendedAttributes
	{
        public TaskGatherInfo(string serviceParameters)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(serviceParameters);
            SetExtendedAttribute(nameValueCollection);
        }

        public string PublishmentSystemIdCollection
        {
            get { return GetExtendedAttribute("PublishmentSystemIdCollection"); }
            set { SetExtendedAttribute("PublishmentSystemIdCollection", value); }
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
