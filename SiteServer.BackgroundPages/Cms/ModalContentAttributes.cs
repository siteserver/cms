using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentAttributes : BasePageCms
    {
        protected CheckBox CbIsRecommend;
        protected CheckBox CbIsHot;
        protected CheckBox CbIsColor;
        protected CheckBox CbIsTop;
        protected HtmlInputHidden HihType;
        protected TextBox TbHits;

        private int _nodeId;
        private string _tableName;
        private List<int> _idArrayList;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("设置内容属性", PageUtils.GetCmsUrl(nameof(ModalContentAttributes), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"nodeID", nodeId.ToString()}
            }), "ContentIDCollection", "请选择需要设置属性的内容！", 450, 350);
        }

        public static string GetOpenWindowStringWithCheckBoxValue(int publishmentSystemId, int nodeId)
        {
            return LayerUtils.GetOpenScriptWithCheckBoxValue("设置内容属性", PageUtils.GetCmsUrl(nameof(ModalContentAttributes), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"nodeID", nodeId.ToString()}
            }), "ContentIDCollection", "请选择需要设置属性的内容！", 450, 350);
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");

            _nodeId = Body.GetQueryInt("NodeID");
            _tableName = NodeManager.GetTableName(PublishmentSystemInfo, _nodeId);
            _idArrayList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ContentIDCollection"));
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
				
            try
            {
                if (HihType.Value == "1")
                {
                    if (CbIsRecommend.Checked || CbIsHot.Checked || CbIsColor.Checked || CbIsTop.Checked)
                    {
                        foreach (var contentId in _idArrayList)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, contentId);
                            if (contentInfo != null)
                            {
                                if (CbIsRecommend.Checked)
                                {
                                    contentInfo.IsRecommend = true;
                                }
                                if (CbIsHot.Checked)
                                {
                                    contentInfo.IsHot = true;
                                }
                                if (CbIsColor.Checked)
                                {
                                    contentInfo.IsColor = true;
                                }
                                if (CbIsTop.Checked)
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
                else if (HihType.Value == "2")
                {
                    if (CbIsRecommend.Checked || CbIsHot.Checked || CbIsColor.Checked || CbIsTop.Checked)
                    {
                        foreach (var contentId in _idArrayList)
                        {
                            var contentInfo = DataProvider.ContentDao.GetContentInfo(_tableName, contentId);
                            if (contentInfo != null)
                            {
                                if (CbIsRecommend.Checked)
                                {
                                    contentInfo.IsRecommend = false;
                                }
                                if (CbIsHot.Checked)
                                {
                                    contentInfo.IsHot = false;
                                }
                                if (CbIsColor.Checked)
                                {
                                    contentInfo.IsColor = false;
                                }
                                if (CbIsTop.Checked)
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
                else if (HihType.Value == "3")
                {
                    var hits = TranslateUtils.ToInt(TbHits.Text);

                    foreach (var contentId in _idArrayList)
                    {
                        DataProvider.ContentDao.SetValue(_tableName, contentId, ContentAttribute.Hits, hits.ToString());
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
                LayerUtils.Close(Page);
			}
		}

	}
}
