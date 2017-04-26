using System;
using BaiRong.Core.Model.Enumerations;
using System.Collections.Generic;

namespace BaiRong.Core.Model
{
	[Serializable]
	public class TableStyleInfo
	{
		private int _tableStyleId;
        private int _relatedIdentity;
        private string _tableName;
        private string _attributeName;
        private int _taxis;
        private string _displayName;
        private string _helpText;
        private bool _isVisible;
        private bool _isVisibleInList;
        private bool _isSingleLine;
		private string _inputType;
        private string _defaultValue;
        private bool _isHorizontal;
        private string _extendValues;

		public TableStyleInfo()
		{
            _tableStyleId = 0;
            _relatedIdentity = 0;
            _tableName = string.Empty;
            _attributeName = string.Empty;
            _taxis = 0;
            _displayName = string.Empty;
            _helpText = string.Empty;
            _isVisible = true;
            _isVisibleInList = false;
            _isSingleLine = true;
            _inputType = EInputTypeUtils.GetValue(EInputType.Text);
            _defaultValue = string.Empty;
            _isHorizontal = true;
            _extendValues = string.Empty;
		}

        public TableStyleInfo(int tableStyleId, int relatedIdentity, string tableName, string attributeName, int taxis, string displayName, string helpText, bool isVisible, bool isVisibleInList, bool isSingleLine, string inputType, string defaultValue, bool isHorizontal, string extendValues) 
		{
            _tableStyleId = tableStyleId;
            _relatedIdentity = relatedIdentity;
            _tableName = tableName;
            _attributeName = attributeName;
            _taxis = taxis;
            _displayName = displayName;
            _helpText = helpText;
            _isVisible = isVisible;
            _isVisibleInList = isVisibleInList;
            _isSingleLine = isSingleLine;
			_inputType = inputType;
            _defaultValue = defaultValue;
            _isHorizontal = isHorizontal;
            _extendValues = extendValues;
		}

		public int TableStyleId
		{
            get { return _tableStyleId; }
            set { _tableStyleId = value; }
		}

        public int RelatedIdentity
		{
            get { return _relatedIdentity; }
            set { _relatedIdentity = value; }
		}

        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        public string AttributeName
        {
            get { return _attributeName; }
            set { _attributeName = value; }
        }

        public int Taxis
        {
            get { return _taxis; }
            set { _taxis = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string HelpText
        {
            get { return _helpText; }
            set { _helpText = value; }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public bool IsVisibleInList
        {
            get { return _isVisibleInList; }
            set { _isVisibleInList = value; }
        }

        public bool IsSingleLine
        {
            get { return _isSingleLine; }
            set { _isSingleLine = value; }
        }

        public string InputType
        {
            get { return _inputType; }
            set { _inputType = value; }
        }

        public string DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }

        public bool IsHorizontal
        {
            get { return _isHorizontal; }
            set { _isHorizontal = value; }
        }

        public string ExtendValues
        {
            get { return _extendValues; }
            set { _extendValues = value; }
        }

	    private TableStyleInfoExtend _additional;
        public TableStyleInfoExtend Additional => _additional ?? (_additional = new TableStyleInfoExtend(_extendValues));

	    private List<TableStyleItemInfo> _styleItems;
        public List<TableStyleItemInfo> StyleItems
        {
            get { return _styleItems; }
            set { _styleItems = value; }
        }
	}

    public class TableStyleInfoExtend : ExtendedAttributes
    {
        public TableStyleInfoExtend(string extendValues)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(extendValues);
            SetExtendedAttribute(nameValueCollection);
        }

        public int Height
        {
            get { return GetInt("Height", 0); }
            set { SetExtendedAttribute("Height", value.ToString()); }
        }

        public string Width
        {
            get
            {
                var width = GetExtendedAttribute("Width");
                return width == "0" ? string.Empty : width;
            }
            set { SetExtendedAttribute("Width", value); }
        }

        public int Columns
        {
            get { return GetInt("Columns", 0); }
            set { SetExtendedAttribute("Columns", value.ToString()); }
        }

        public bool IsFormatString
        {
            get { return GetBool("IsFormatString", false); }
            set { SetExtendedAttribute("IsFormatString", value.ToString()); }
        }

        public int RelatedFieldId
        {
            get { return GetInt("RelatedFieldID", 0); }
            set { SetExtendedAttribute("RelatedFieldID", value.ToString()); }
        }

        public string RelatedFieldStyle
        {
            get { return GetExtendedAttribute("RelatedFieldStyle"); }
            set { SetExtendedAttribute("RelatedFieldStyle", value); }
        }

        public bool IsValidate
        {
            get { return GetBool("IsValidate", false); }
            set { SetExtendedAttribute("IsValidate", value.ToString()); }
        }

        public bool IsRequired
        {
            get { return GetBool("IsRequired", false); }
            set { SetExtendedAttribute("IsRequired", value.ToString()); }
        }

        public int MinNum
        {
            get { return GetInt("MinNum", 0); }
            set { SetExtendedAttribute("MinNum", value.ToString()); }
        }

        public int MaxNum
        {
            get { return GetInt("MaxNum", 0); }
            set { SetExtendedAttribute("MaxNum", value.ToString()); }
        }

        public EInputValidateType ValidateType
        {
            get { return EInputValidateTypeUtils.GetEnumType(GetExtendedAttribute("ValidateType")); }
            set { SetExtendedAttribute("ValidateType", EInputValidateTypeUtils.GetValue(value)); }
        }

        public string RegExp
        {
            get { return GetExtendedAttribute("RegExp"); }
            set { SetExtendedAttribute("RegExp", value); }
        }

        public string ErrorMessage
        {
            get { return GetExtendedAttribute("ErrorMessage"); }
            set { SetExtendedAttribute("ErrorMessage", value); }
        }

        /// <summary>
        /// 是否启用统计
        /// </summary>
        public bool IsUseStatistics
        {
            get { return  GetBool("IsUseStatistics",false); }
            set { SetExtendedAttribute("IsUseStatistics", value.ToString()); }
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }
    }
}
