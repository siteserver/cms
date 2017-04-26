using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageGatherDatabaseRule : BasePageCms
    {
		public DataGrid dgContents;
        public Button Start;

		public string GetDatabaseInfo(string relatedTableName)
		{
            return relatedTableName;
		}

		public string GetEidtLink(string gatherRuleName)
		{
            var urlEdit = PageGatherDatabaseRuleAdd.GetRedirectUrl(PublishmentSystemId, gatherRuleName);
            return $"<a href=\"{urlEdit}\">编辑</a>";
		}

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetCmsUrl(nameof(PageGatherDatabaseRule), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public string GetAutoCreateClickString()
        {
            return
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetCmsUrl(nameof(PageGatherDatabaseRule), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"Auto", true.ToString()}
                    }), "GatherDatabaseRuleNameCollection", "GatherDatabaseRuleNameCollection", "请选择需要打开自动生成的规则！",
                    "确认要设置所选规则为自动生成吗？");
        }

        public string GetNoAutoCreateClickString()
        {
            return
                PageUtils.GetRedirectStringWithCheckBoxValueAndAlert(
                    PageUtils.GetCmsUrl(nameof(PageGatherDatabaseRule), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()},
                        {"NoAuto", true.ToString()}
                    }), "GatherDatabaseRuleNameCollection", "GatherDatabaseRuleNameCollection", "请选择需要打开自动生成的规则！",
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
            var showPopWinString = ModalGatherDatabaseSet.GetOpenWindowString(PublishmentSystemId, gatherRuleName);
			return $@"<a href=""javascript:;"" onclick=""{showPopWinString}"">开始采集</a>";
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
                    DataProvider.GatherDatabaseRuleDao.Delete(gatherRuleName, PublishmentSystemId);
                    Body.AddSiteLog(PublishmentSystemId, "删除数据库采集规则", $"采集规则:{gatherRuleName}");
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
                    var gatherDatabaseRuleInfo = DataProvider.GatherDatabaseRuleDao.GetGatherDatabaseRuleInfo(gatherRuleName, PublishmentSystemId);
                    gatherDatabaseRuleInfo.GatherRuleName = gatherDatabaseRuleInfo.GatherRuleName + "_复件";
                    gatherDatabaseRuleInfo.LastGatherDate = DateUtils.SqlMinValue;

                    DataProvider.GatherDatabaseRuleDao.Insert(gatherDatabaseRuleInfo);
                    Body.AddSiteLog(PublishmentSystemId, "复制数据库采集规则", $"采集规则:{gatherRuleName}");
                    SuccessMessage("采集规则复制成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "采集规则复制失败！");
                }
            }

            if (Body.IsQueryExists("Auto") && Body.IsQueryExists("GatherDatabaseRuleNameCollection"))
            {
                var gatherDatabaseRuleNameCollection = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("GatherDatabaseRuleNameCollection"));
                try
                {
                    DataProvider.GatherDatabaseRuleDao.OpenAuto(PublishmentSystemId, gatherDatabaseRuleNameCollection);
                    Body.AddSiteLog(PublishmentSystemId, "开启采集规则自动生成成功",
                        $"采集规则:{Body.GetQueryString("GatherDatabaseRuleNameCollection")}");
                    SuccessMessage("开启采集规则自动生成成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "开启采集规则自动生成失败！");
                }
            }

            if (Body.IsQueryExists("NoAuto") && Body.IsQueryExists("GatherDatabaseRuleNameCollection"))
            {
                var gatherDatabaseRuleNameCollection = TranslateUtils.StringCollectionToStringList(Body.GetQueryString("GatherDatabaseRuleNameCollection"));
                try
                {
                    DataProvider.GatherDatabaseRuleDao.CloseAuto(PublishmentSystemId, gatherDatabaseRuleNameCollection);
                    Body.AddSiteLog(PublishmentSystemId, "关闭采集规则自动生成成功",
                        $"采集规则:{Body.GetQueryString("GatherDatabaseRuleNameCollection")}");
                    SuccessMessage("关闭采集规则自动生成成功！");
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "关闭采集规则自动生成失败！");
                }
            }

            if (!IsPostBack)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdGather, "数据库采集", AppManager.Cms.Permission.WebSite.Gather);

                var showPopWinString = ModalProgressBar.GetOpenWindowStringWithGatherDatabase(PublishmentSystemId);
                Start.Attributes.Add("onclick", showPopWinString);

                dgContents.DataSource = DataProvider.GatherDatabaseRuleDao.GetDataSource(PublishmentSystemId);
                dgContents.DataBind();
            }
		}
	}
}
