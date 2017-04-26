using BaiRong.Core;

namespace SiteServer.BackgroundPages
{
    public class PageDefault : BasePage
	{
        protected override bool IsAccessable => true;

	    protected string AdminDirectoryName => FileConfigManager.Instance.AdminDirectoryName;
	}
}
