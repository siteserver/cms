using System.Collections.Generic;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Configuration
{
    /// <summary>
    /// 表示表单的输入样式。
    /// </summary>
    //public class InputStyle
    //{
    //    /// <summary>
    //    /// 表单的字段名称。
    //    /// </summary>
    //    public string AttributeName { get; set; }

    //    /// <summary>
    //    /// 表单的输入类型。
    //    /// </summary>
    //    public InputType InputType { get; set; }

    //    /// <summary>
    //    /// 表单输入的显示名称。
    //    /// </summary>
    //    public string DisplayName { get; set; }

    //    /// <summary>
    //    /// 表单输入的提示信息。
    //    /// </summary>
    //    public string HelpText { get; set; }

    //    /// <summary>
    //    /// 表单输入的列表项。
    //    /// 当表单的输入类型（InputType）为列表项（Checkbox、Radio、Select）时启用。
    //    /// </summary>
    //    public List<InputListItem> ListItems { get; set; }

    //    /// <summary>
    //    /// 表单输入的默认值。
    //    /// </summary>
    //    public string DefaultValue { get; set; }

    //    /// <summary>
    //    /// 表单输入是否必填项。
    //    /// </summary>
    //    public bool IsRequired { get; set; }

    //    /// <summary>
    //    /// 表单输入的验证规则。
    //    /// </summary>
    //    public ValidateType ValidateType { get; set; }

    //    /// <summary>
    //    /// 表单输入的最小字符数。
    //    /// 0 代表不限制。
    //    /// </summary>
    //    public int MinNum { get; set; }

    //    /// <summary>
    //    /// 表单输入的最大字符数。
    //    /// 0 代表不限制。
    //    /// </summary>
    //    public int MaxNum { get; set; }

    //    /// <summary>
    //    /// 表单输入验证的正则表达式。
    //    /// 当表单输入的验证规则（ValidateType）为正则表达式验证（RegExp）时启用。
    //    /// </summary>
    //    public string RegExp { get; set; }

    //    /// <summary>
    //    /// 表单输入的显示宽度。
    //    /// </summary>
    //    public string Width { get; set; }

    //    /// <summary>
    //    /// 表单输入的显示高度。
    //    /// </summary>
    //    public string Height { get; set; }
    //}

    /// <summary>
    /// 表单字段的输入样式。
    /// </summary>
    public class InputStyle
    {
        public InputStyle()
        {

        }

        public InputStyle(TableStyle style)
        {
            Id = style.Id;
            AttributeName = style.AttributeName;
            DisplayName = style.DisplayName;
            InputType = style.InputType;
            Rules = TranslateUtils.JsonDeserialize<List<InputStyleRule>>(style.RuleValues);
            Items = style.Items;
            Taxis = style.Taxis;
            DefaultValue = style.DefaultValue;
        }

        /// <summary>
        /// 样式的输入类型。
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 字段名称。
        /// </summary>
        public string AttributeName { get; set; }
        public string DisplayName { get; set; }
        /// <summary>
        /// 表单输入的提示信息。
        /// </summary>
        public string HelpText { get; set; }
        /// <summary>
        /// 样式的输入类型。
        /// </summary>
        public InputType InputType { get; set; }
        public List<InputStyleRule> Rules { get; set; }
        public List<InputStyleItem> Items { get; set; }
        public int Taxis { get; set; }
        public bool IsSystem { get; set; }
        /// <summary>
        /// 表单输入的默认值。
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 样式的显示宽度。
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 样式的显示高度。
        /// </summary>
        public string Height { get; set; }
    }
}