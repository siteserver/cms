using SiteServer.Utils;

namespace SiteServer.BackgroundPages
{
    public class PageDefault : BasePage
	{
        protected override bool IsAccessable => true;

	    protected string AdminDirectoryName => WebConfigUtils.AdminDirectory;
	}
}
