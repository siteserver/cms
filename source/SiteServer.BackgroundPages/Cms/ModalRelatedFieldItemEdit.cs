using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalRelatedFieldItemEdit : BasePageCms
    {
        protected TextBox ItemName;
        protected TextBox ItemValue;

        private int _relatedFieldId;
        private int _parentId;
        private int _level;
        private int _id;

        public static string GetOpenWindowString(int publishmentSystemId, int relatedFieldId, int parentId, int level, int id)
        {
            return PageUtils.GetOpenWindowString("编辑字段项", PageUtils.GetCmsUrl(nameof(ModalRelatedFieldItemEdit), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedFieldID", relatedFieldId.ToString()},
                {"ParentID", parentId.ToString()},
                {"Level", level.ToString()},
                {"ID", id.ToString()}
            }), 320, 260);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedFieldId = Body.GetQueryInt("RelatedFieldID");
            _parentId = Body.GetQueryInt("ParentID");
            _level = Body.GetQueryInt("Level");
            _id = Body.GetQueryInt("ID");

            if (!IsPostBack)
            {
                var itemInfo = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfo(_id);
                ItemName.Text = itemInfo.ItemName;
                ItemValue.Text = itemInfo.ItemValue;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            bool isChanged;

            try
            {
                var itemInfo = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfo(_id);
                itemInfo.ItemName = ItemName.Text;
                itemInfo.ItemValue = ItemValue.Text;
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
                PageUtils.CloseModalPageAndRedirect(Page, PageRelatedFieldItem.GetRedirectUrl(PublishmentSystemId, _relatedFieldId, _parentId, _level));
            }
        }
    }
}
