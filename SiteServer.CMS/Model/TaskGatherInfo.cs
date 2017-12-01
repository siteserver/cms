using BaiRong.Core.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
	public class TaskGatherInfo : ExtendedAttributes
	{
        public TaskGatherInfo(string settings) : base(settings)
        {

        }

        public string PublishmentSystemIdCollection
        {
            get { return GetString("PublishmentSystemIdCollection"); }
            set { Set("PublishmentSystemIdCollection", value); }
        }

        public string WebGatherNames
		{
            get { return GetString("WebGatherNames"); }
            set { Set("WebGatherNames", value); }
		}

        public string DatabaseGatherNames
        {
            get { return GetString("DatabaseGatherNames"); }
            set { Set("DatabaseGatherNames", value); }
        }

        public string FileGatherNames
        {
            get { return GetString("FileGatherNames"); }
            set { Set("FileGatherNames", value); }
        }
	}
}
