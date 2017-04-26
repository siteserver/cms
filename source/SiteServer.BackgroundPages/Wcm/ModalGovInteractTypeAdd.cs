using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.BackgroundPages.Wcm
{
	public class ModalGovInteractTypeAdd : BasePageCms
	{
        protected TextBox tbTypeName;

        private int _nodeId;
        private int _typeId;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowString("添加办件类型", PageUtils.GetWcmUrl(nameof(ModalGovInteractTypeAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()}
            }), 450, 220);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int nodeId, int typeId)
        {
            return PageUtils.GetOpenWindowString("修改办件类型", PageUtils.GetWcmUrl(nameof(ModalGovInteractTypeAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"NodeID", nodeId.ToString()},
                {"TypeID", typeId.ToString()}
            }), 450, 220);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _nodeId = TranslateUtils.ToInt(Request.QueryString["NodeID"]);
            _typeId = TranslateUtils.ToInt(Request.QueryString["TypeID"]);

			if (!IsPostBack)
			{
                if (_typeId > 0)
                {
                    var typeInfo = DataProvider.GovInteractTypeDao.GetTypeInfo(_typeId);
                    if (typeInfo != null)
                    {
                        tbTypeName.Text = typeInfo.TypeName;
                    }
                }

				
			}
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;
            GovInteractTypeInfo typeInfo = null;
				
			if (_typeId > 0)
			{
				try
				{
                    typeInfo = DataProvider.GovInteractTypeDao.GetTypeInfo(_typeId);
                    if (typeInfo != null)
                    {
                        typeInfo.TypeName = tbTypeName.Text;
                    }
                    DataProvider.GovInteractTypeDao.Update(typeInfo);

                    Body.AddSiteLog(PublishmentSystemId, "修改办件类型", $"办件类型:{typeInfo.TypeName}");

					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "办件类型修改失败！");
				}
			}
			else
			{
                var typeNameArrayList = DataProvider.GovInteractTypeDao.GetTypeNameArrayList(_nodeId);
                if (typeNameArrayList.IndexOf(tbTypeName.Text) != -1)
				{
                    FailMessage("办件类型添加失败，办件类型名称已存在！");
                }
				else
				{
					try
					{
                        typeInfo = new GovInteractTypeInfo(0, tbTypeName.Text, _nodeId, PublishmentSystemId, 0);

                        DataProvider.GovInteractTypeDao.Insert(typeInfo);

                        Body.AddSiteLog(PublishmentSystemId, "添加办件类型", $"办件类型:{typeInfo.TypeName}");

						isChanged = true;
					}
					catch(Exception ex)
					{
                        FailMessage(ex, "办件类型添加失败！");
					}
				}
			}

			if (isChanged)
			{
                PageUtils.CloseModalPageAndRedirect(Page, PageGovInteractType.GetRedirectUrl(PublishmentSystemId, _nodeId));
			}
		}
	}
}
