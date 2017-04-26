using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalContentModelAdd : BasePageCms
    {
        protected TextBox tbModelID;
        protected TextBox tbModelName;
        protected TextBox tbIconUrl;
        protected RadioButtonList rblTableType;
        protected DropDownList ddlTableName;
        protected TextBox tbDescription;

        public static string GetOpenWindowStringToAdd(int publishmentSystemId)
        {
            return PageUtils.GetOpenWindowString("添加内容模型", PageUtils.GetCmsUrl(nameof(ModalContentModelAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()}
            }), 530, 480);
        }

        public static string GetOpenWindowStringToEdit(int publishmentSystemId, string modelId)
        {
            return PageUtils.GetOpenWindowString("修改内容模型", PageUtils.GetCmsUrl(nameof(ModalContentModelAdd), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ModelID", modelId}
            }), 530, 480);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

			PageUtils.CheckRequestParameter("PublishmentSystemID");
			if (!IsPostBack)
			{
                rblTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.BackgroundContent, true));
                rblTableType.Items.Add(EAuxiliaryTableTypeUtils.GetListItem(EAuxiliaryTableType.UserDefined, false));

                if (Body.IsQueryExists("ModelID"))
                {
                    tbModelID.Enabled = false;
                    var modelId = Body.GetQueryString("ModelID");
                    var modelInfo = BaiRongDataProvider.ContentModelDao.GetContentModelInfo(modelId, PublishmentSystemId);
                    if (modelInfo != null)
                    {
                        tbModelID.Text = modelId;
                        tbModelName.Text = modelInfo.ModelName;
                        tbIconUrl.Text = modelInfo.IconUrl;
                        ControlUtils.SelectListItemsIgnoreCase(rblTableType, EAuxiliaryTableTypeUtils.GetValue(modelInfo.TableType));
                        rblTableType_SelectedIndexChanged(null, null);
                        ControlUtils.SelectListItemsIgnoreCase(ddlTableName, modelInfo.TableName);
                        tbDescription.Text = modelInfo.Description;
                    }
                }
                else
                {
                    rblTableType_SelectedIndexChanged(null, null);
                }
				
			}
		}

        public void rblTableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlTableName.Items.Clear();
            var tableType = EAuxiliaryTableTypeUtils.GetEnumType(rblTableType.SelectedValue);
            var tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(tableType);
            foreach (AuxiliaryTableInfo tableInfo in tableList)
            {
                var li = new ListItem(tableInfo.TableCnName + "(" + tableInfo.TableEnName + ")", tableInfo.TableEnName);
                ddlTableName.Items.Add(li);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
			var isChanged = false;

			ContentModelInfo modelInfo;

			if (Body.IsQueryExists("ModelID"))
			{
                var modelId = Body.GetQueryString("ModelID");
                modelInfo = BaiRongDataProvider.ContentModelDao.GetContentModelInfo(modelId, PublishmentSystemId);

                modelInfo.ModelName = tbModelName.Text;
                modelInfo.TableName = ddlTableName.SelectedValue;
                modelInfo.TableType = EAuxiliaryTableTypeUtils.GetEnumType(rblTableType.SelectedValue);
                modelInfo.IconUrl = tbIconUrl.Text;
                modelInfo.Description = tbDescription.Text;
			}
			else
			{
			    modelInfo = new ContentModelInfo
			    {
			        ModelId = tbModelID.Text,
			        SiteId = PublishmentSystemId,
			        ModelName = tbModelName.Text,
			        IsSystem = false,
			        TableName = ddlTableName.SelectedValue,
			        TableType = EAuxiliaryTableTypeUtils.GetEnumType(rblTableType.SelectedValue),
			        IconUrl = tbIconUrl.Text,
			        Description = tbDescription.Text
			    };
			}

            if (Body.IsQueryExists("ModelID"))
			{
				try
				{
                    BaiRongDataProvider.ContentModelDao.Update(modelInfo);
                    Body.AddSiteLog(PublishmentSystemId, "修改内容模型", $"内容模型:{modelInfo.ModelName}");
					isChanged = true;
				}
				catch(Exception ex)
				{
                    FailMessage(ex, "内容模型修改失败！");
				}
			}
			else
			{
                var isFail = false;
                var modelInfoList = ContentModelManager.GetContentModelInfoList(PublishmentSystemInfo);
                foreach (var contentModelInfo in modelInfoList)
                {
                    if (contentModelInfo.ModelId == tbModelID.Text)
                    {
                        FailMessage("内容模型添加失败，模型标识已存在！");
                        isFail = true;
                        break;
                    }
                    else if (contentModelInfo.ModelName == tbModelName.Text)
                    {
                        FailMessage("内容模型添加失败，模型名称已存在！");
                        isFail = true;
                        break;
                    }
                }
                if (!isFail)
				{
                    try
                    {
                        BaiRongDataProvider.ContentModelDao.Insert(modelInfo);
                        Body.AddSiteLog(PublishmentSystemId, "添加内容模型", $"内容模型:{modelInfo.ModelName}");
                        isChanged = true;
                    }
                    catch(Exception ex)
                    {
                        FailMessage(ex, "内容模型添加失败！");
                    }
				}
			}

			if (isChanged)
			{
                PageUtils.CloseModalPage(Page);
			}
		}
	}
}
