using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalRelatedFieldItemAdd : BasePageCms
    {
        public TextBox TbItemNames;

        private int _relatedFieldId;
        private int _parentId;
        private int _level;

        public static string GetOpenWindowString(int siteId, int relatedFieldId, int parentId, int level)
        {
            return LayerUtils.GetOpenScript("添加字段项", PageUtils.GetCmsUrl(siteId, nameof(ModalRelatedFieldItemAdd), new NameValueCollection
            {
                {"RelatedFieldID", relatedFieldId.ToString()},
                {"ParentID", parentId.ToString()},
                {"Level", level.ToString()}
            }), 450, 450);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedFieldId = AuthRequest.GetQueryInt("RelatedFieldID");
            _parentId = AuthRequest.GetQueryInt("ParentID");
            _level = AuthRequest.GetQueryInt("Level");
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            bool isChanged;

            try
            {
                var itemNameArray = TbItemNames.Text.Split('\n');
                foreach (var item in itemNameArray)
                {
                    if (string.IsNullOrEmpty(item)) continue;

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

                isChanged = true;
            }
            catch
            {
                isChanged = false;
                FailMessage("添加字段项出错！");
            }

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, PageRelatedFieldItem.GetRedirectUrl(SiteId, _relatedFieldId, _parentId, _level));
            }
        }
    }
}
