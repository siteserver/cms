using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalInputContentAdd : BasePageCms
    {
        protected AuxiliaryControl ContentControl;

        private string _returnUrl;
        private InputInfo _inputInfo;
        private int _contentId;
        private List<int> _relatedIdentities;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId, int inputId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("添加信息", PageUtils.GetCmsUrl(nameof(ModalInputContentAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }));
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, int inputId, int contentId, string returnUrl)
        {
            return PageUtils.GetOpenWindowString("修改信息", PageUtils.GetCmsUrl(nameof(ModalInputContentAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"InputID", inputId.ToString()},
                {"ContentID", contentId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            var inputId = int.Parse(Body.GetQueryString("InputID"));
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _inputInfo = DataProvider.InputDao.GetInputInfo(inputId);

			if (Body.IsQueryExists("ContentID"))
			{
                _contentId = int.Parse(Body.GetQueryString("ContentID"));
			}
			else
			{
                _contentId = 0;
			}

            _relatedIdentities = RelatedIdentities.GetRelatedIdentities(ETableStyle.InputContent, PublishmentSystemId, inputId);

            if (!IsPostBack)
            {
                if (_contentId != 0)
                {
                    var contentInfo = DataProvider.InputContentDao.GetContentInfo(_contentId);
                    if (contentInfo != null)
                    {
                        ContentControl.SetParameters(contentInfo.Attributes, PublishmentSystemInfo, 0, _relatedIdentities, ETableStyle.InputContent, DataProvider.InputContentDao.TableName, true, IsPostBack);
                    }
                }
                else
                {
                    ContentControl.SetParameters(null, PublishmentSystemInfo, 0, _relatedIdentities, ETableStyle.InputContent, DataProvider.InputContentDao.TableName, false, IsPostBack);
                }
                
            }
            else
            {
                if (_contentId != 0)
                {
                    ContentControl.SetParameters(Request.Form, PublishmentSystemInfo, 0, _relatedIdentities, ETableStyle.InputContent, DataProvider.InputContentDao.TableName, true, IsPostBack);
                }
                else
                {
                    ContentControl.SetParameters(Request.Form, PublishmentSystemInfo, 0, _relatedIdentities, ETableStyle.InputContent, DataProvider.InputContentDao.TableName, false, IsPostBack);
                }
            }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;
				
			if (_contentId != 0)
			{
				try
				{
                    var contentInfo = DataProvider.InputContentDao.GetContentInfo(_contentId);

                    InputTypeParser.AddValuesToAttributes(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, PublishmentSystemInfo, _relatedIdentities, Page.Request.Form, contentInfo.Attributes);

                    DataProvider.InputContentDao.Update(contentInfo);

                    var builder = new StringBuilder();

                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, _relatedIdentities);
                    foreach (var styleInfo in styleInfoList)
                    {
                        if (styleInfo.IsVisible == false) continue;

                        var theValue = InputParserUtility.GetContentByTableStyle(contentInfo.GetExtendedAttribute(styleInfo.AttributeName), PublishmentSystemInfo, ETableStyle.InputContent, styleInfo);

                        builder.Append($@"{styleInfo.DisplayName}：{theValue},");
                    }

                    if (builder.Length > 0)
                    {
                        builder.Length = builder.Length - 1;
                    }

                    if (builder.Length > 60)
                    {
                        builder.Length = 60;
                    }

                    Body.AddSiteLog(PublishmentSystemId, "修改提交表单内容", $"提交表单:{_inputInfo.InputName},{builder}");
					isChanged = true;
				}
                catch (Exception ex)
				{
                    FailMessage(ex, "信息修改失败:" + ex.Message);
				}
			}
			else
			{
                try
                {
                    var ipAddress = PageUtils.GetIpAddress();

                    var contentInfo = new InputContentInfo(0, _inputInfo.InputId, 0, true, string.Empty, ipAddress, DateTime.Now, string.Empty);

                    InputTypeParser.AddValuesToAttributes(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, PublishmentSystemInfo, _relatedIdentities, Page.Request.Form, contentInfo.Attributes);

                    DataProvider.InputContentDao.Insert(contentInfo);

                    var builder = new StringBuilder();

                    var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.InputContent, DataProvider.InputContentDao.TableName, _relatedIdentities);
                    foreach (var styleInfo in styleInfoList)
                    {
                        if (styleInfo.IsVisible == false) continue;

                        var theValue = InputParserUtility.GetContentByTableStyle(contentInfo.GetExtendedAttribute(styleInfo.AttributeName), PublishmentSystemInfo, ETableStyle.InputContent, styleInfo);

                        builder.Append($@"{styleInfo.DisplayName}：{theValue},");
                    }

                    if (builder.Length > 0)
                    {
                        builder.Length = builder.Length - 1;
                    }

                    if (builder.Length > 60)
                    {
                        builder.Length = 60;
                    }

                    Body.AddSiteLog(PublishmentSystemId, "添加提交表单内容", $"提交表单:{_inputInfo.InputName},{builder}");
                    isChanged = true;
                }
                catch(Exception ex)
                {
                    FailMessage(ex, "信息添加失败:" + ex.Message);
                }
			}

			if (isChanged)
			{
                PageUtils.CloseModalPageAndRedirect(Page, _returnUrl);
			}
		}
	}
}
