using System;
using SiteServer.CMS.Wcm.GovInteract;

namespace SiteServer.BackgroundPages.Wcm
{
	public class BasePageGovInteract : BasePageCms
	{
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GovInteractManager.Initialize(PublishmentSystemInfo);
        }
	}
}
