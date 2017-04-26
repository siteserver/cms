using System;
using SiteServer.CMS.Wcm.GovPublic;

namespace SiteServer.BackgroundPages.Wcm
{
	public class BasePageGovPublic : BasePageCms
	{
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GovPublicManager.Initialize(PublishmentSystemInfo);
        }
	}
}
