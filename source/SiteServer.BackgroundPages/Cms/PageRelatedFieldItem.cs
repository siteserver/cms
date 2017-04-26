using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageRelatedFieldItem : BasePageCms
    {
        public DataGrid dgContents;
        public Button AddButton;
        public Button ReturnButton;

        private int _relatedFieldId;
        private int _parentId;
        private int _level;
        private int _totalLevel;

        public static string GetRedirectUrl(int publishmentSystemId, int relatedFieldId, int parentId, int level)
        {
            return PageUtils.GetCmsUrl(nameof(PageRelatedFieldItem), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString() },
                {"RelatedFieldID", relatedFieldId.ToString() },
                {"ParentID", parentId.ToString() },
                {"Level", level.ToString() }
            });
        }

        public static string GetRedirectUrl(int publishmentSystemId, int relatedFieldId, int level)
        {
            return PageUtils.GetCmsUrl(nameof(PageRelatedFieldItem), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString() },
                {"RelatedFieldID", relatedFieldId.ToString() },
                {"Level", level.ToString() }
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _relatedFieldId = Body.GetQueryInt("RelatedFieldID");
            _parentId = Body.GetQueryInt("ParentID");
            _level = Body.GetQueryInt("Level");
            _totalLevel = DataProvider.RelatedFieldDao.GetRelatedFieldInfo(_relatedFieldId).TotalLevel;

            if (Body.IsQueryExists("Delete") && Body.IsQueryExists("ID"))
            {
                var id = Body.GetQueryInt("ID");
                try
                {
                    DataProvider.RelatedFieldItemDao.Delete(id);
                    if (_level != _totalLevel)
                    {
                        AddScript($@"parent.location.href = '{PageRelatedFieldMain.GetRedirectUrl(PublishmentSystemId, _relatedFieldId, _totalLevel)}';");
                    }
                }
                catch (Exception ex)
                {
                    FailMessage($"删除字段项失败，{ex.Message}");
                }
            }
            else if ((Body.IsQueryExists("Up") || Body.IsQueryExists("Down")) && Body.IsQueryExists("ID"))
            {
                var id = Body.GetQueryInt("ID");
                var isDown = Body.IsQueryExists("Down");
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

            if (!IsPostBack)
            {
                string level;
                if (_level == 1)
                {
                    level = "一级";
                }
                else
                {
                    var itemInfo = DataProvider.RelatedFieldItemDao.GetRelatedFieldItemInfo(_parentId);
                    var levelString = "二";
                    if (_level == 3)
                    {
                        levelString = "三";
                    }
                    else if (_level == 4)
                    {
                        levelString = "四";
                    }
                    else if (_level == 5)
                    {
                        levelString = "五";
                    }

                    level = $"{levelString}级({itemInfo.ItemName})";
                }

                BreadCrumbWithItemTitle(AppManager.Cms.LeftMenu.IdConfigration, AppManager.Cms.LeftMenu.Configuration.IdConfigurationContentModel, "联动字段管理", level, AppManager.Cms.Permission.WebSite.Configration);

                BindGrid();

                AddButton.Attributes.Add("onclick", ModalRelatedFieldItemAdd.GetOpenWindowString(PublishmentSystemId, _relatedFieldId, _parentId, _level));

                if (_level == 1)
                {
                    var urlReturn = PageRelatedField.GetRedirectUrl(PublishmentSystemId);
                    ReturnButton.Attributes.Add("onclick", $"parent.location.href = '{urlReturn}';return false;");
                }
                else
                {
                    ReturnButton.Visible = false;
                }
            }
        }

        public void BindGrid()
        {
            if (_totalLevel >= 5)
            {
                dgContents.Columns[1].Visible = false;
            }
            dgContents.DataSource = DataProvider.RelatedFieldItemDao.GetDataSource(_relatedFieldId, _parentId);
            dgContents.ItemDataBound += DgContents_ItemDataBound;
            dgContents.DataBind();
        }

        private void DgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var id = SqlUtils.EvalInt(e.Item.DataItem, "ID");
            var itemName = (string)DataBinder.Eval(e.Item.DataItem, "ItemName");
            var itemValue = (string)DataBinder.Eval(e.Item.DataItem, "ItemValue");

            var ltlItemName = (Literal)e.Item.FindControl("ltlItemName");
            var ltlItemValue = (Literal)e.Item.FindControl("ltlItemValue");
            var hlUpLinkButton = (HyperLink)e.Item.FindControl("hlUpLinkButton");
            var hlDownLinkButton = (HyperLink)e.Item.FindControl("hlDownLinkButton");
            var ltlEditUrl = (Literal)e.Item.FindControl("ltlEditUrl");
            var ltlDeleteUrl = (Literal)e.Item.FindControl("ltlDeleteUrl");

            if (_level >= _totalLevel)
            {
                ltlItemName.Text = itemName;
            }
            else
            {
                ltlItemName.Text =
                    $@"<a href=""{GetRedirectUrl(PublishmentSystemId,
                        _relatedFieldId, id, _level + 1)}"" target=""level{_level + 1}"">{itemName}</a>";
            }
            ltlItemValue.Text = itemValue;
            hlUpLinkButton.NavigateUrl = GetRedirectUrl(PublishmentSystemId, _relatedFieldId, _parentId, _level) + "&Up=True&ID=" + id;

            hlDownLinkButton.NavigateUrl = GetRedirectUrl(PublishmentSystemId, _relatedFieldId, _parentId, _level) + "&Down=True&ID=" + id;

            ltlEditUrl.Text =
                $@"<a href='javascript:;' onclick=""{ModalRelatedFieldItemEdit.GetOpenWindowString(
                    PublishmentSystemId, _relatedFieldId, _parentId, _level, id)}"">编辑</a>";

            ltlDeleteUrl.Text =
                $@"<a href=""{GetRedirectUrl(PublishmentSystemId, _relatedFieldId, _parentId, _level)}&Delete=True&ID={id}"" onClick=""javascript:return confirm('此操作将删除字段项“{itemName}”及其子类，确认吗？');"">删除</a>";
        }
    }
}
