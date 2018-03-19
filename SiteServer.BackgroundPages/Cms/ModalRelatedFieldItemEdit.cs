using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalRelatedFieldItemEdit : BasePageCms
    {
        public TextBox TbItemName;
        public TextBox TbItemValue;

        private int _relatedFieldId;
        private int _parentId;
        private int _level;
        private int _id;

        public static string GetOpenWindowString(int siteId, int relatedFieldId, int parentId, int level, int id)
        {
            return LayerUtils.GetOpenScript("编辑字段项", PageUtils.GetCmsUrl(siteId, nameof(ModalRelatedFieldItemEdit), new NameValueCollection
            {
                {"RelatedFieldID", relatedFieldId.ToString()},
                {"ParentID", parentId.ToString()},
                {"Level", level.ToString()},
                {"ID", id.ToString()}
            }), 320, 260);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedFieldId = AuthRequest.GetQueryInt("RelatedFieldID");
            _parentId = AuthRequest.GetQueryInt("ParentID");
            _level = AuthRequest.GetQueryInt("Level");
            _id = AuthRequest.GetQueryInt("ID");

            if (IsPostBack) return;

            var itemInfo = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfo(_id);
            TbItemName.Text = itemInfo.ItemName;
            TbItemValue.Text = itemInfo.ItemValue;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            bool isChanged;

            try
            {
                var itemInfo = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfo(_id);
                itemInfo.ItemName = TbItemName.Text;
                itemInfo.ItemValue = TbItemValue.Text;
                DataProvider.RelatedFieldItemDao.Update(itemInfo);

                isChanged = true;
            }
            catch(Exception ex)
            {
                isChanged = false;
                FailMessage(ex, ex.Message);
            }

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, PageRelatedFieldItem.GetRedirectUrl(SiteId, _relatedFieldId, _parentId, _level));
            }
        }
    }
}
