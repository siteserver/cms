using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageRelatedFieldItem : BasePageCms
    {
        public Repeater RptContents;
        public Button BtnAdd;
        public Button BtnReturn;

        private int _relatedFieldId;
        private int _parentId;
        private int _level;
        private int _totalLevel;

        public static string GetRedirectUrl(int siteId, int relatedFieldId, int parentId, int level)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageRelatedFieldItem), new NameValueCollection
            {
                {"RelatedFieldID", relatedFieldId.ToString() },
                {"ParentID", parentId.ToString() },
                {"Level", level.ToString() }
            });
        }

        public static string GetRedirectUrl(int siteId, int relatedFieldId, int level)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageRelatedFieldItem), new NameValueCollection
            {
                {"RelatedFieldID", relatedFieldId.ToString() },
                {"Level", level.ToString() }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedFieldId = AuthRequest.GetQueryInt("RelatedFieldID");
            _parentId = AuthRequest.GetQueryInt("ParentID");
            _level = AuthRequest.GetQueryInt("Level");
            _totalLevel = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(_relatedFieldId).TotalLevel;

            if (AuthRequest.IsQueryExists("Delete") && AuthRequest.IsQueryExists("ID"))
            {
                var id = AuthRequest.GetQueryInt("ID");
                DataProvider.RelatedFieldItemDao.Delete(id);
                if (_level != _totalLevel)
                {
                    AddScript($@"parent.location.href = '{PageRelatedFieldMain.GetRedirectUrl(SiteId, _relatedFieldId, _totalLevel)}';");
                }
            }
            else if ((AuthRequest.IsQueryExists("Up") || AuthRequest.IsQueryExists("Down")) && AuthRequest.IsQueryExists("ID"))
            {
                var id = AuthRequest.GetQueryInt("ID");
                var isDown = AuthRequest.IsQueryExists("Down");
                if (isDown)
                {
                    DataProvider.RelatedFieldItemDao.UpdateTaxisToUp(id, _parentId);
                }
                else
                {
                    DataProvider.RelatedFieldItemDao.UpdateTaxisToDown(id, _parentId);
                }
            }
            else if (_level != _totalLevel)
            {
                InfoMessage("点击字段项名可以管理下级字段项");    
            }

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Configration);

            //if (_totalLevel >= 5)
            //{
            //    RptContents.Columns[1].Visible = false;
            //}

            RptContents.DataSource = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfoList(_relatedFieldId, _parentId);
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            RptContents.DataBind();

            BtnAdd.Attributes.Add("onclick", ModalRelatedFieldItemAdd.GetOpenWindowString(SiteId, _relatedFieldId, _parentId, _level));

            if (_level == 1)
            {
                var urlReturn = PageRelatedField.GetRedirectUrl(SiteId);
                BtnReturn.Attributes.Add("onclick", $"parent.location.href = '{urlReturn}';return false;");
            }
            else
            {
                BtnReturn.Visible = false;
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var itemInfo = (RelatedFieldItemInfo)e.Item.DataItem;

            var ltlItemName = (Literal)e.Item.FindControl("ltlItemName");
            var ltlItemValue = (Literal)e.Item.FindControl("ltlItemValue");
            var hlUp = (HyperLink)e.Item.FindControl("hlUp");
            var hlDown = (HyperLink)e.Item.FindControl("hlDown");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            if (_level >= _totalLevel)
            {
                ltlItemName.Text = itemInfo.ItemName;
            }
            else
            {
                ltlItemName.Text =
                    $@"<a href=""{GetRedirectUrl(SiteId,
                        _relatedFieldId, itemInfo.Id, _level + 1)}"" target=""level{_level + 1}"">{itemInfo.ItemName}</a>";
            }

            ltlItemValue.Text = itemInfo.ItemValue;
            hlUp.NavigateUrl = GetRedirectUrl(SiteId, _relatedFieldId, _parentId, _level) + "&Up=True&ID=" + itemInfo.Id;
            hlDown.NavigateUrl = GetRedirectUrl(SiteId, _relatedFieldId, _parentId, _level) + "&Down=True&ID=" + itemInfo.Id;

            ltlEditUrl.Text =
                $@"<a href='javascript:;' onclick=""{ModalRelatedFieldItemEdit.GetOpenWindowString(
                    SiteId, _relatedFieldId, _parentId, _level, itemInfo.Id)}"">编辑</a>";

            ltlDeleteUrl.Text =
                $@"<a href=""{GetRedirectUrl(SiteId, _relatedFieldId, _parentId, _level)}&Delete=True&ID={itemInfo.Id}"" onClick=""javascript:return confirm('此操作将删除字段项“{itemInfo.ItemName}”及其子类，确认吗？');"">删除</a>";
        }
    }
}
