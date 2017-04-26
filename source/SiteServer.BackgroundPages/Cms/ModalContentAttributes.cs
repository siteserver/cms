using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentAttributes : BasePageCms
    {
        protected CheckBox IsRecommend;
        protected CheckBox IsHot;
        protected CheckBox IsColor;
        protected CheckBox IsTop;
        protected HtmlInputHidden hdType;
        protected TextBox Hits;

        private int _nodeId;
        private ETableStyle _tableStyle;
        private string _tableName;
        private List<int> _idArrayList;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("设置内容属性", PageUtils.GetCmsUrl(nameof(ModalContentAttributes), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"nodeID", nodeId.ToString()}
            }), "ContentIDCollection", "请选择需要设置属性的内容！", 300, 240);
        }

        public static string GetOpenWindowStringWithCheckBoxValue(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowStringWithCheckBoxValue("设置内容属性", PageUtils.GetCmsUrl(nameof(ModalContentAttributes), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"nodeID", nodeId.ToString()}
            }), "ContentIDCollection", "请选择需要设置属性的内容！", 300, 240);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");

            _nodeId = Body.GetQueryInt("NodeID");
            _tableStyle = NodeManager.GetTableStyle(PublishmentSystemInfo, _nodeId); 
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);
            _idArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
				
            try
            {
                if (hdType.Value == "1")
                {
                    if (IsRecommend.Checked || IsHot.Checked || IsColor.Checked || IsTop.Checked)
                    {
                        foreach (int contentID in _idArrayList)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableStyle, _tableName, contentID) as BackgroundContentInfo;
                            if (contentInfo != null)
                            {
                                if (IsRecommend.Checked)
                                {
                                    contentInfo.IsRecommend = true;
                                }
                                if (IsHot.Checked)
                                {
                                    contentInfo.IsHot = true;
                                }
                                if (IsColor.Checked)
                                {
                                    contentInfo.IsColor = true;
                                }
                                if (IsTop.Checked)
                                {
                                    contentInfo.IsTop = true;
                                }
                                DataProvider.ContentDao.Update(_tableName, PublishmentSystemInfo, contentInfo);
                            }
                        }

                        Body.AddSiteLog(PublishmentSystemId, "设置内容属性");

                        isChanged = true;
                    }
                }
                else if (hdType.Value == "2")
                {
                    if (IsRecommend.Checked || IsHot.Checked || IsColor.Checked || IsTop.Checked)
                    {
                        foreach (int contentID in _idArrayList)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableStyle, _tableName, contentID) as BackgroundContentInfo;
                            if (contentInfo != null)
                            {
                                if (IsRecommend.Checked)
                                {
                                    contentInfo.IsRecommend = false;
                                }
                                if (IsHot.Checked)
                                {
                                    contentInfo.IsHot = false;
                                }
                                if (IsColor.Checked)
                                {
                                    contentInfo.IsColor = false;
                                }
                                if (IsTop.Checked)
                                {
                                    contentInfo.IsTop = false;
                                }
                                DataProvider.ContentDao.Update(_tableName, PublishmentSystemInfo, contentInfo);
                            }
                        }

                        Body.AddSiteLog(PublishmentSystemId, "取消内容属性");

                        isChanged = true;
                    }
                }
                else if (hdType.Value == "3")
                {
                    var hits = TranslateUtils.ToInt(Hits.Text);

                    foreach (int contentID in _idArrayList)
                    {
                        BaiRongDataProvider.ContentDao.SetValue(_tableName, contentID, ContentAttribute.Hits, hits.ToString());
                    }

                    Body.AddSiteLog(PublishmentSystemId, "设置内容点击量");

                    isChanged = true;
                }
            }
			catch(Exception ex)
			{
                FailMessage(ex, ex.Message);
			    isChanged = false;
			}

			if (isChanged)
			{
				PageUtils.CloseModalPage(Page);
			}
		}

	}
}
