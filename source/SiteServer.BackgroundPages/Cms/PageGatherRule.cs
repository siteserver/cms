using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageGatherRule : BasePageCms
    {
        public DataGrid dgContents;
        public Button Start;
        public Button Export;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageGatherRule), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public string GetImportClickString()
        {
            return ModalImport.GetOpenWindowString(PublishmentSystemId, ModalImport.TypeGatherrule);
        }

        public string GetAutoCreateClickString()
        {
            return PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                PageUtils.GetCmsUrl(nameof(PageGatherRule), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"Auto", true.ToString()}
                }), "GatherRuleNameCollection", "GatherRuleNameCollection", "请选择需要打开自动生成的规则！", "确认要设置所选规则为自动生成吗？");
        }

        public string GetNoAutoCreateClickString()
        {
            return PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                PageUtils.GetCmsUrl(nameof(PageGatherRule), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"NoAuto", true.ToString()}
                }), "GatherRuleNameCollection", "GatherRuleNameCollection", "请选择需要打开自动生成的规则！", "确认要设置所选规则为自动生成吗？");
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (Body.IsQueryExists("Delete"))
            {
                var gatherRuleName = Body.GetQueryString("GatherRuleName");
                try
                {
                    DataProvider.GatherRuleDao.Delete(gatherRuleName, PublishmentSystemId);
                    Body.AddSiteLog(PublishmentSystemId, "删除Web页面采集规则", $"采集规则:{gatherRuleName}");
                    SuccessDeleteMessage();
                }
                catch (Exception ex)
                {
                    FailDeleteMessage(ex);
                }
            }

            if (Body.IsQueryExists("Copy"))
            {
                var gatherRuleName = Body.GetQueryString("GatherRuleName");
                try
                {
                    var gatherRuleInfo = DataProvider.GatherRuleDao.GetGatherRuleInfo(gatherRuleName, PublishmentSystemId);
                    gatherRuleInfo.GatherRuleName = gatherRuleInfo.GatherRuleName + "_复件";
                    gatherRuleInfo.LastGatherDate = DateUtils.SqlMinValue;

                    DataProvider.GatherRuleDao.Insert(gatherRuleInfo);
                    Body.AddSiteLog(PublishmentSystemId, "复制Web页面采集规则", $"采集规则:{gatherRuleName}");
                    SuccessMessage("采集规则复制成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "采集规则复制失败！");
                }
            }

            if (Body.IsQueryExists("Auto") && Body.IsQueryExists("GatherRuleNameCollection"))
            {
                var gatherRuleNameCollection = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("GatherRuleNameCollection"));
                try
                {
                    foreach (string item in gatherRuleNameCollection)
                    {
                        var gatherRuleInfoTmp = DataProvider.GatherRuleDao.GetGatherRuleInfo(item, PublishmentSystemId);
                        gatherRuleInfoTmp.Additional.IsAutoCreate = true;
                        DataProvider.GatherRuleDao.Update(gatherRuleInfoTmp);
                    }

                    Body.AddSiteLog(PublishmentSystemId, "开启采集规则自动生成成功", $"采集规则:{gatherRuleNameCollection}");
                    SuccessMessage("开启采集规则自动生成成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "开启采集规则自动生成失败！");
                }
            }

            if (Body.IsQueryExists("NoAuto") && Body.IsQueryExists("GatherRuleNameCollection"))
            {
                var gatherRuleNameCollection = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("GatherRuleNameCollection"));
                try
                {
                    foreach (string item in gatherRuleNameCollection)
                    {
                        var gatherRuleInfoTmp = DataProvider.GatherRuleDao.GetGatherRuleInfo(item, PublishmentSystemId);
                        gatherRuleInfoTmp.Additional.IsAutoCreate = false;
                        DataProvider.GatherRuleDao.Update(gatherRuleInfoTmp);
                    }
                    Body.AddSiteLog(PublishmentSystemId, "关闭采集规则自动生成成功", $"采集规则:{gatherRuleNameCollection}");
                    SuccessMessage("关闭采集规则自动生成成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "关闭采集规则自动生成失败！");
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdGather, "Web页面信息采集", AppManager.Cms.Permission.WebSite.Gather);

                var showPopWinString = ModalProgressBar.GetOpenWindowStringWithGather(PublishmentSystemId);
                Start.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalExportMessage.GetOpenWindowStringToGatherRule(PublishmentSystemId, "GatherRuleNameCollection", "请选择需要导出的规则！");
                Export.Attributes.Add("onclick", showPopWinString);

                dgContents.DataSource = DataProvider.GatherRuleDao.GetGatherRuleInfoArrayList(PublishmentSystemId);
                dgContents.ItemDataBound += dgContents_ItemDataBound;
                dgContents.DataBind();
            }
        }

        void dgContents_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var gatherRuleInfo = e.Item.DataItem as GatherRuleInfo;

                var ltlGatherRuleName = (Literal)e.Item.FindControl("ltlGatherRuleName");
                var ltlGatherUrl = (Literal)e.Item.FindControl("ltlGatherUrl");
                var ltlLastGatherDate = (Literal)e.Item.FindControl("ltlLastGatherDate");
                var ltlIsAutoCreate = (Literal)e.Item.FindControl("ltlIsAutoCreate");
                var ltlTestGatherUrl = (Literal)e.Item.FindControl("ltlTestGatherUrl");
                var ltlStartGatherUrl = (Literal)e.Item.FindControl("ltlStartGatherUrl");
                var ltlEditLink = (Literal)e.Item.FindControl("ltlEditLink");
                var ltlCopyLink = (Literal)e.Item.FindControl("ltlCopyLink");
                var ltlDeleteLink = (Literal)e.Item.FindControl("ltlDeleteLink");

                ltlGatherRuleName.Text = gatherRuleInfo.GatherRuleName;
                var gatherUrlArrayList = GatherUtility.GetGatherUrlArrayList(gatherRuleInfo);
                if (gatherUrlArrayList != null && gatherUrlArrayList.Count > 0)
                {
                    var url = (string)gatherUrlArrayList[0];
                    url = PageUtils.AddProtocolToUrl(url);
                    ltlGatherUrl.Text =
                        $@"<a href=""{url}"" target=""_blank"" title=""{url}"">{StringUtils.MaxLengthText(url, 25)}</a>";
                }
                if (!DateUtils.SqlMinValue.Equals(gatherRuleInfo.LastGatherDate))
                {
                    ltlLastGatherDate.Text = DateUtils.GetDateAndTimeString(gatherRuleInfo.LastGatherDate);
                }
                if (gatherRuleInfo.Additional.IsAutoCreate)
                    ltlIsAutoCreate.Text = "是";
                else
                    ltlIsAutoCreate.Text = "否";
                var showPopWinString = ModalGatherTest.GetOpenWindowString(PublishmentSystemId, gatherRuleInfo.GatherRuleName, false);
                ltlTestGatherUrl.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">测试</a>";

                showPopWinString = ModalGatherSet.GetOpenWindowString(PublishmentSystemId, gatherRuleInfo.GatherRuleName);
                ltlStartGatherUrl.Text = $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">开始采集</a>";

                var urlEdit = PageGatherRuleAdd.GetRedirectUrl(PublishmentSystemId, gatherRuleInfo.GatherRuleName);
                ltlEditLink.Text = $"<a href=\"{urlEdit}\">编辑</a>";

                var urlCopy = PageUtils.GetCmsUrl(nameof(PageGatherRule), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"GatherRuleName", gatherRuleInfo.GatherRuleName},
                    {"Copy", true.ToString()}
                });
                ltlCopyLink.Text = $@"<a href=""{urlCopy}"">复制</a>";

                var urlDelete = PageUtils.GetCmsUrl(nameof(PageGatherRule), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"GatherRuleName", gatherRuleInfo.GatherRuleName},
                    {"Delete", true.ToString()}
                });
                ltlDeleteLink.Text =
                    $@"<a href=""{urlDelete}"" onClick=""javascript:return confirm('此操作将删除采集规则“{gatherRuleInfo
                        .GatherRuleName}”，确认吗？');"">删除</a>";
            }
        }

    }
}
