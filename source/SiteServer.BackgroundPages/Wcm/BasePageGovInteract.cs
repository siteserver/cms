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

        private int _publishmentSystemId = -1;
        public override int PublishmentSystemId
        {
            get
            {
                if (_publishmentSystemId == -1)
                {
                    _publishmentSystemId = Body.GetQueryInt("siteId");
                }
                return _publishmentSystemId;
            }
        }
    }
}
