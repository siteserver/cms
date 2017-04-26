using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalRelatedFieldItemAdd : BasePageCms
    {
        protected TextBox ItemNames;

        private int _relatedFieldId;
        private int _parentId;
        private int _level;

        public static string GetOpenWindowString(int publishmentSystemId, int relatedFieldId, int parentId, int level)
        {
            return PageUtils.GetOpenWindowString("添加字段项", PageUtils.GetCmsUrl(nameof(ModalRelatedFieldItemAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"RelatedFieldID", relatedFieldId.ToString()},
                {"ParentID", parentId.ToString()},
                {"Level", level.ToString()}
            }), 300, 450);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedFieldId = Body.GetQueryInt("RelatedFieldID");
            _parentId = Body.GetQueryInt("ParentID");
            _level = Body.GetQueryInt("Level");
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = false;

            try
            {
                var itemNameArray = ItemNames.Text.Split('\n');
                foreach (var item in itemNameArray)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var itemName = item.Trim();
                        var itemValue = itemName;

                        if (itemName.IndexOf('|') != -1)
                        {
                            itemValue = itemName.Substring(itemName.IndexOf('|') + 1);
                            itemName = itemName.Substring(0, itemName.IndexOf('|'));
                        }

                        var itemInfo = new RelatedFieldItemInfo(0, _relatedFieldId, itemName, itemValue, _parentId, 0);
                        DataProvider.RelatedFieldItemDao.Insert(itemInfo);
                    }
                }

                isChanged = true;
            }
            catch
            {
                isChanged = false;
                FailMessage("添加字段项出错！");
            }

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, PageRelatedFieldItem.GetRedirectUrl(PublishmentSystemId, _relatedFieldId, _parentId, _level));
            }
        }
    }
}
