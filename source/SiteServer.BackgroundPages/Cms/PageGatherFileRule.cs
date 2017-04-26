using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageGatherFileRule : BasePageCms
    {
		public DataGrid dgContents;
        public Button Start;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageGatherFileRule), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
            });
        }

        public string GetGatherUrl(string gatherUrl)
        {
            gatherUrl = PageUtils.AddProtocolToUrl(gatherUrl);
            return
                $"<a href=\"{gatherUrl}\" target='_blank' title=\"{gatherUrl}\">{StringUtils.MaxLengthText(gatherUrl, 25)}</a>";
		}

        public string GetEditLink(string gatherRuleName)
		{
            var urlEdit = PageGatherFileRuleAdd.GetRedirectUrl(PublishmentSystemId, gatherRuleName);
            return $"<a href=\"{urlEdit}\">编辑</a>";
		}

        public string GetAutoCreateClickString()
        {
            return PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                PageUtils.GetCmsUrl(nameof(PageGatherFileRule), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"Auto", true.ToString()}
                }), "GatherFileRuleNameCollection", "GatherFileRuleNameCollection", "请选择需要打开自动生成的规则！",
                "确认要设置所选规则为自动生成吗？");
        }

        public string GetNoAutoCreateClickString()
        {
            return PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                PageUtils.GetCmsUrl(nameof(PageGatherFileRule), new NameValueCollection
                {
                    {"PublishmentSystemID", PublishmentSystemId.ToString()},
                    {"NoAuto", true.ToString()}
                }), "GatherFileRuleNameCollection", "GatherFileRuleNameCollection", "请选择需要打开自动生成的规则！",
                "确认要设置所选规则为自动生成吗？");
        }

        public string GetLastGatherDate(DateTime lastGatherDate)
		{
			if (DateUtils.SqlMinValue.Equals(lastGatherDate))
			{
				return string.Empty;
			}
			else
			{
                return DateUtils.GetDateAndTimeString(lastGatherDate);
			}
		}

        public string GetIsAutoCreate(bool isAutoCreate)
        {
            if (isAutoCreate)
            {
                return "是";
            }
            else
            {
                return "否";
            }
        }

        public string GetStartGatherUrl(string gatherRuleName)
		{
            var showPopWinString = ModalGatherFileSet.GetOpenWindowString(PublishmentSystemId, gatherRuleName);
			return $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">开始采集</a>";
		}

		public string GetTestGatherUrl(string gatherRuleName)
		{
            var showPopWinString = ModalGatherTest.GetOpenWindowString(PublishmentSystemId, gatherRuleName, true);
			return $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">测试</a>";
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
                    DataProvider.GatherFileRuleDao.Delete(gatherRuleName, PublishmentSystemId);
                    Body.AddSiteLog(PublishmentSystemId, "删除单文件页采集规则", $"采集规则:{gatherRuleName}");
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
                    var gatherFileRuleInfo = DataProvider.GatherFileRuleDao.GetGatherFileRuleInfo(gatherRuleName, PublishmentSystemId);
                    gatherFileRuleInfo.GatherRuleName = gatherFileRuleInfo.GatherRuleName + "_复件";
                    gatherFileRuleInfo.LastGatherDate = DateUtils.SqlMinValue;

                    DataProvider.GatherFileRuleDao.Insert(gatherFileRuleInfo);
                    Body.AddSiteLog(PublishmentSystemId, "复制单文件页采集规则", $"采集规则:{gatherRuleName}");
                    SuccessMessage("采集规则复制成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "采集规则复制失败！");
                }
            }

            if (Body.IsQueryExists("Auto") && Body.IsQueryExists("GatherFileRuleNameCollection"))
            {
                var gatherFileRuleNameCollection = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("GatherFileRuleNameCollection"));
                try
                {
                    DataProvider.GatherFileRuleDao.OpenAuto(PublishmentSystemId, gatherFileRuleNameCollection);
                    Body.AddSiteLog(PublishmentSystemId, "开启采集规则自动生成成功",
                        $"采集规则:{Body.GetQueryString("GatherFileRuleNameCollection")}");
                    SuccessMessage("开启采集规则自动生成成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "开启采集规则自动生成失败！");
                }
            }

            if (Body.IsQueryExists("NoAuto") && Body.IsQueryExists("GatherFileRuleNameCollection"))
            {
                var gatherFileRuleNameCollection = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("GatherFileRuleNameCollection"));
                try
                {
                    DataProvider.GatherFileRuleDao.CloseAuto(PublishmentSystemId, gatherFileRuleNameCollection);
                    Body.AddSiteLog(PublishmentSystemId, "关闭采集规则自动生成成功",
                        $"采集规则:{Body.GetQueryString("GatherFileRuleNameCollection")}");
                    SuccessMessage("关闭采集规则自动生成成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "关闭采集规则自动生成失败！");
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdGather, "单文件页采集", AppManager.Cms.Permission.WebSite.Gather);

                var showPopWinString = ModalProgressBar.GetOpenWindowStringWithGatherFile(PublishmentSystemId);
                Start.Attributes.Add("onclick", showPopWinString);

                dgContents.DataSource = DataProvider.GatherFileRuleDao.GetDataSource(PublishmentSystemId);
                dgContents.DataBind();
            }
		}
	}
}
