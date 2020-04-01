using System.Web.UI.WebControls;
using SiteServer.Plugin;

namespace SiteServer.Utils
{
    public class InputParserUtils
    {
        private InputParserUtils()
        {
        }

        //private static string GetValidateCheckMethod(string attributeName, string displayName, InputValidateInfo validateInfo)
        //{
        //    if (validateInfo != null)
        //    {
        //        return
        //            $"checkAttributeValue('{attributeName}', '{displayName}', {validateInfo.IsRequire.ToString().ToLower()}, {validateInfo.MinNum}, {validateInfo.MaxNum}, '{validateInfo.RegExp}', '{validateInfo.ErrorMessage}');";
        //    }
        //    return string.Empty;
        //}

        public static string GetValidateAttributes(bool isValidate, string displayName, bool isRequire, int minNum, int maxNum, ValidateType validateType, string regExp, string errorMessage)
        {
            if (isValidate)
            {
                return
                    $@"isValidate=""{true.ToString().ToLower()}"" displayName=""{displayName}"" isRequire=""{isRequire
                        .ToString().ToLower()}"" minNum=""{minNum}"" maxNum=""{maxNum}"" validateType=""{validateType.Value}"" regExp=""{regExp}"" errorMessage=""{errorMessage}""";
            }
            return string.Empty;
        }

        public static void GetValidateAttributesForListItem(ListControl control, bool isValidate, string displayName, bool isRequire, int minNum, int maxNum, ValidateType validateType, string regExp, string errorMessage)
        {
            if (!isValidate) return;

            control.Attributes.Add("isValidate", true.ToString().ToLower());
            control.Attributes.Add("displayName", displayName);
            control.Attributes.Add("isRequire", isRequire.ToString().ToLower());
            control.Attributes.Add("minNum", minNum.ToString());
            control.Attributes.Add("maxNum", maxNum.ToString());
            control.Attributes.Add("validateType", validateType.Value);
            control.Attributes.Add("regExp", regExp);
            control.Attributes.Add("errorMessage", errorMessage);
            control.Attributes.Add("isListItem", true.ToString().ToLower());
        }

        public static string GetValidateSubmitOnClickScript(string formId)
        {
            return $"return checkFormValueById('{formId}');";
        }

        /// <summary>
        /// 带有提示的确认操作
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="isConfirm"></param>
        /// <param name="confirmFunction"></param>
        /// <returns></returns>
        public static string GetValidateSubmitOnClickScript(string formId, bool isConfirm, string confirmFunction)
        {
            return !isConfirm ? GetValidateSubmitOnClickScript(formId) : $"return checkFormValueById('{formId}') && {confirmFunction};";
        }

        //public static string GetAdditionalAttributes(string whereUsed, InputType inputType)
        //{
        //    var additionalAttributes = string.Empty;
        //    if (string.IsNullOrEmpty(whereUsed))
        //    {
        //        //if (inputType == InputType.Text || inputType == InputType.Image || inputType == InputType.File)
        //        //{
        //        //    additionalAttributes = @"class=""colorblur"" onfocus=""this.className='colorfocus';"" onblur=""this.className='colorblur';"" size=""60""";
        //        //}
        //        //else if (inputType == InputType.TextArea)
        //        //{
        //        //    additionalAttributes = @"class=""colorblur"" onfocus=""this.className='colorfocus';"" onblur=""this.className='colorblur';"" cols=""60"" rows=""5""";
        //        //}
        //        //else if (inputType == InputType.Date || inputType == InputType.DateTime)
        //        //{
        //        //    additionalAttributes = @"class=""colorblur Wdate"" size=""25""";
        //        //}
        //    }
        //    else if (whereUsed == "usercenter")
        //    {
        //        if (inputType == InputType.Text || inputType == InputType.Image || inputType == InputType.Video || inputType == InputType.File)
        //        {
        //            additionalAttributes = @"class=""input-txt"" style=""width:320px""";
        //        }
        //        else if (inputType == InputType.TextArea)
        //        {
        //            additionalAttributes = @"class=""input-area area-s5"" cols=""60"" rows=""5""";
        //        }
        //        else if (inputType == InputType.Date || inputType == InputType.DateTime)
        //        {
        //            additionalAttributes = @"class=""input-txt Wdate"" style=""width:120px""";
        //        }
        //    }
        //    return additionalAttributes;
        //}

        //public static string GetInnerAdditionalAttributes(InputType inputType, EAuxiliaryTableType tableType, string attributeName)
        //{
        //    string additionalAttributes = string.Empty;
        //    if (inputType == InputType.Default)
        //    {
        //        inputType = InputTypeUtils.GetDefaultInputType(tableType, attributeName);
        //    }

        //    if (inputType == InputType.Text)
        //    {
        //        additionalAttributes = @"class=""colorblur"" onfocus=""this.className='colorfocus';"" onblur=""this.className='colorblur';"" size=""60""";
        //    }
        //    else if (inputType == InputType.TextArea)
        //    {
        //        additionalAttributes = @"class=""colorblur"" onfocus=""this.className='colorfocus';"" onblur=""this.className='colorblur';"" cols=""60"" rows=""5""";
        //    }
        //    else if (inputType == InputType.Date || inputType == InputType.DateTime)
        //    {
        //        additionalAttributes = @"class=""colorblur"" size=""30""";
        //    }
        //    else if (inputType == InputType.Image || inputType == InputType.File)
        //    {
        //        additionalAttributes = @"size=""50""";
        //    }
        //    return additionalAttributes;
        //}
    }
}
