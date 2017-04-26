using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.AuxiliaryTable;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.BackgroundPages.Sys
{
	public class ModalTableStyleValidateAdd : BasePage
    {
        public RadioButtonList IsValidate;
        public PlaceHolder phValidate;
        public RadioButtonList IsRequired;
        public PlaceHolder phNum;
        public TextBox MinNum;
        public TextBox MaxNum;
        public DropDownList ValidateType;
        public PlaceHolder phRegExp;
        public TextBox RegExp;
        public TextBox ErrorMessage;

        private int _tableStyleId;
        private string _tableName;
        private string _attributeName;
        private string _redirectUrl;
        private TableStyleInfo _styleInfo;
        private ETableStyle _tableStyle;

        public static string GetOpenWindowString(int tableStyleId, string tableName, string attributeName, ETableStyle tableStyle, string redirectUrl)
        {
            return PageUtils.GetOpenWindowString("设置表单验证", PageUtils.GetSysUrl(nameof(ModalTableStyleValidateAdd), new NameValueCollection
            {
                {"TableStyleID", tableStyleId.ToString()},
                {"TableName", tableName},
                {"AttributeName", attributeName},
                {"TableStyle", ETableStyleUtils.GetValue(tableStyle)},
                {"RedirectUrl", StringUtils.ValueToUrl(redirectUrl)}
            }), 450, 460);
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            _tableStyleId = Body.GetQueryInt("TableStyleID");
            _tableName = Body.GetQueryString("TableName");
            _attributeName = Body.GetQueryString("AttributeName");
            _tableStyle = ETableStyleUtils.GetEnumType(Body.GetQueryString("TableStyle"));
            _redirectUrl = StringUtils.ValueFromUrl(Body.GetQueryString("RedirectUrl"));

            if (_tableStyleId != 0)
            {
                _styleInfo = BaiRongDataProvider.TableStyleDao.GetTableStyleInfo(_tableStyleId);
            }
            else
            {
                _styleInfo = TableStyleManager.GetTableStyleInfo(_tableStyle, _tableName, _attributeName, new List<int>{0});
            }

            if (!IsPostBack)
            {
                IsValidate.Items[0].Value = true.ToString();
                IsValidate.Items[1].Value = false.ToString();

                ControlUtils.SelectListItems(IsValidate, _styleInfo.Additional.IsValidate.ToString());

                IsRequired.Items[0].Value = true.ToString();
                IsRequired.Items[1].Value = false.ToString();

                ControlUtils.SelectListItems(IsRequired, _styleInfo.Additional.IsRequired.ToString());

                if (EInputTypeUtils.EqualsAny(_styleInfo.InputType, EInputType.Text, EInputType.TextArea))
                {
                    phNum.Visible = true;
                }
                else
                {
                    phNum.Visible = false;
                }

                MinNum.Text = _styleInfo.Additional.MinNum.ToString();
                MaxNum.Text = _styleInfo.Additional.MaxNum.ToString();

                EInputValidateTypeUtils.AddListItems(ValidateType);
                ControlUtils.SelectListItems(ValidateType, EInputValidateTypeUtils.GetValue(_styleInfo.Additional.ValidateType));

                RegExp.Text = _styleInfo.Additional.RegExp;
                ErrorMessage.Text = _styleInfo.Additional.ErrorMessage;

                Validate_SelectedIndexChanged(null, EventArgs.Empty);

                
            }
		}

        public void Validate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (EBooleanUtils.Equals(EBoolean.False, IsValidate.SelectedValue))
            {
                phValidate.Visible = false;
            }
            else
            {
                phValidate.Visible = true;
            }

            var type = EInputValidateTypeUtils.GetEnumType(ValidateType.SelectedValue);
            if (type == EInputValidateType.Custom)
            {
                phRegExp.Visible = true;
            }
            else
            {
                phRegExp.Visible = false;
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isChanged = InsertOrUpdateTableStyleInfo();

            if (isChanged)
            {
                PageUtils.CloseModalPageAndRedirect(Page, _redirectUrl);
            }
		}

        private bool InsertOrUpdateTableStyleInfo()
        {
            var isChanged = false;

            _styleInfo.Additional.IsValidate = TranslateUtils.ToBool(IsValidate.SelectedValue);
            _styleInfo.Additional.IsRequired = TranslateUtils.ToBool(IsRequired.SelectedValue);
            _styleInfo.Additional.MinNum = TranslateUtils.ToInt(MinNum.Text);
            _styleInfo.Additional.MaxNum = TranslateUtils.ToInt(MaxNum.Text);
            _styleInfo.Additional.ValidateType = EInputValidateTypeUtils.GetEnumType(ValidateType.SelectedValue);
            _styleInfo.Additional.RegExp = RegExp.Text.Trim('/');
            _styleInfo.Additional.ErrorMessage = ErrorMessage.Text;

            try
            {
                if (_tableStyleId == 0)//数据库中没有此项的表样式，但是有父项的表样式
                {
                    _styleInfo.RelatedIdentity = 0;
                    _styleInfo.TableStyleId = TableStyleManager.Insert(_styleInfo, _tableStyle);
                }

                if (_styleInfo.TableStyleId > 0)
                {
                    TableStyleManager.Update(_styleInfo);
                    Body.AddAdminLog("修改表单验证", $"类型:{ETableStyleUtils.GetText(_tableStyle)},字段:{_styleInfo.AttributeName}");
                }
                else
                {
                    TableStyleManager.Insert(_styleInfo, _tableStyle);
                    Body.AddAdminLog("新增表单验证", $"类型:{ETableStyleUtils.GetText(_tableStyle)},字段:{_styleInfo.AttributeName}");
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
