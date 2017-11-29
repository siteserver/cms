using BaiRong.Core.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
	public class TaskCreateInfo : ExtendedAttributes
	{
        public TaskCreateInfo(string settings) : base(settings)
        {

        }

        public bool IsCreateAll
        {
            get { return GetBool("IsCreateAll"); }
            set { Set("IsCreateAll", value.ToString()); }
        }

        public string ChannelIdCollection
		{
            get { return GetString("ChannelIdCollection"); }
            set { Set("ChannelIdCollection", value); }
		}

        public string CreateTypes
        {
            get { return GetString("CreateTypes"); }
            set { Set("CreateTypes", value); }
        }
	}
}
