using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;
using SiteServer.Plugin;
using TableStyle = SiteServer.CMS.Model.TableStyle;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalTableStyleValidateAdd : BasePageCms
    {
        public DropDownList DdlIsValidate;
        public PlaceHolder PhValidate;
        public DropDownList DdlIsRequired;
        public PlaceHolder PhNum;
        public TextBox TbMinNum;
        public TextBox TbMaxNum;
        public DropDownList DdlValidateType;
        public PlaceHolder PhRegExp;
        public TextBox TbRegExp;
        public TextBox TbErrorMessage;

        private int _tableStyleId;
        private List<int> _relatedIdentities;
        private string _tableName;
        private string _attributeName;
        private string _redirectUrl;
        private TableStyle _style;

        public static string GetOpenWindowString(int siteId, int tableStyleId, List<int> relatedIdentities, string tableName, string attributeName, string redirectUrl)
        {
            return LayerUtils.GetOpenScript("设置表单验证", PageUtils.GetCmsUrl(siteId, nameof(ModalTableStyleValidateAdd), new NameValueCollection
            {
                {"TableStyleID", tableStyleId.ToString()},
                {"RelatedIdentities", TranslateUtils.ObjectCollectionToString(relatedIdentities)},
                {"TableName", tableName},
                {"AttributeName", attributeName},
                {"RedirectUrl", StringUtils.ValueToUrl(redirectUrl)}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableStyleId = AuthRequest.GetQueryInt("TableStyleID");
            _relatedIdentities = TranslateUtils.StringCollectionToIntList(AuthRequest.GetQueryString("RelatedIdentities"));
            if (_relatedIdentities.Count == 0)
            {
                _relatedIdentities.Add(0);
            }
            _tableName = AuthRequest.GetQueryString("TableName");
            _attributeName = AuthRequest.GetQueryString("AttributeName");
            _redirectUrl = StringUtils.ValueFromUrl(AuthRequest.GetQueryString("RedirectUrl"));

            _style = _tableStyleId != 0
                ? TableStyleManager.GetTableStyleAsync(_tableStyleId).GetAwaiter().GetResult()
                : TableStyleManager.GetTableStyleAsync(_tableName, _attributeName, _relatedIdentities).GetAwaiter().GetResult();

            if (IsPostBack) return;

            DdlIsValidate.Items[0].Value = true.ToString();
            DdlIsValidate.Items[1].Value = false.ToString();

            ControlUtils.SelectSingleItem(DdlIsValidate, _style.IsValidate.ToString());

            DdlIsRequired.Items[0].Value = true.ToString();
            DdlIsRequired.Items[1].Value = false.ToString();

            ControlUtils.SelectSingleItem(DdlIsRequired, _style.IsRequired.ToString());

            PhNum.Visible = InputTypeUtils.EqualsAny(_style.Type, InputType.Text, InputType.TextArea);

            TbMinNum.Text = _style.MinNum.ToString();
            TbMaxNum.Text = _style.MaxNum.ToString();

            ValidateTypeUtils.AddListItems(DdlValidateType);
            ControlUtils.SelectSingleItem(DdlValidateType, _style.ValidateType.Value);

            TbRegExp.Text = _style.RegExp;
            TbErrorMessage.Text = _style.ErrorMessage;

            DdlValidate_SelectedIndexChanged(null, EventArgs.Empty);
        }

        public void DdlValidate_SelectedIndexChanged(object sender, EventArgs e)
        {
            PhValidate.Visible = !EBooleanUtils.Equals(EBoolean.False, DdlIsValidate.SelectedValue);
            var type = ValidateTypeUtils.GetEnumType(DdlValidateType.SelectedValue);
            PhRegExp.Visible = type == ValidateType.RegExp;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = InsertOrUpdateTableStyle();

            if (isChanged)
            {
                LayerUtils.CloseAndRedirect(Page, _redirectUrl);
            }
		}

        private bool InsertOrUpdateTableStyle()
        {
            var isChanged = false;

            _style.IsValidate = TranslateUtils.ToBool(DdlIsValidate.SelectedValue);
            _style.IsRequired = TranslateUtils.ToBool(DdlIsRequired.SelectedValue);
            _style.MinNum = TranslateUtils.ToInt(TbMinNum.Text);
            _style.MaxNum = TranslateUtils.ToInt(TbMaxNum.Text);
            _style.ValidateType = ValidateTypeUtils.GetEnumType(DdlValidateType.SelectedValue);
            _style.RegExp = TbRegExp.Text.Trim('/');
            _style.ErrorMessage = TbErrorMessage.Text;

            try
            {
                if (_tableStyleId == 0)//数据库中没有此项的表样式，但是有父项的表样式
                {
                    var relatedIdentity = _relatedIdentities[0];
                    _style.RelatedIdentity = relatedIdentity;
                    _style.Id = DataProvider.TableStyleDao.InsertAsync(_style).GetAwaiter().GetResult();
                }

                if (_style.Id > 0)
                {
                    DataProvider.TableStyleDao.UpdateAsync(_style).GetAwaiter().GetResult();
                    AuthRequest.AddSiteLogAsync(SiteId, "修改表单验证", $"字段:{_style.AttributeName}").GetAwaiter().GetResult();
                }
                else
                {
                    DataProvider.TableStyleDao.InsertAsync(_style).GetAwaiter().GetResult();
                    AuthRequest.AddSiteLogAsync(SiteId, "新增表单验证", $"字段:{_style.AttributeName}").GetAwaiter().GetResult();
                }
                isChanged = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, "设置表单验证失败：" + ex.Message);
            }
            return isChanged;
        }
	}
}
