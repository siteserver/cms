using System;
using System.Collections.Generic;
using SiteServer.Plugin.Models;

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
            _inputType = InputTypeUtils.GetValue(SiteServer.Plugin.Models.InputType.Text);
            _defaultValue = string.Empty;
            _isHorizontal = true;
            _extendValues = string.Empty;
		}

        public TableStyleInfo(int tableStyleId, int relatedIdentity, string tableName, string attributeName, int taxis, string displayName, string helpText, bool isVisible, bool isVisibleInList, string inputType, string defaultValue, bool isHorizontal, string extendValues) 
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
        public TableStyleInfoExtend(string settings) : base(settings)
        {

        }

        public int Height
        {
            get { return GetInt("Height"); }
            set { Set("Height", value.ToString()); }
        }

        public string Width
        {
            get
            {
                var width = GetString("Width");
                return width == "0" ? string.Empty : width;
            }
            set { Set("Width", value); }
        }

        public int Columns
        {
            get { return GetInt("Columns"); }
            set { Set("Columns", value.ToString()); }
        }

        public bool IsFormatString
        {
            get { return GetBool("IsFormatString"); }
            set { Set("IsFormatString", value.ToString()); }
        }

        public int RelatedFieldId
        {
            get { return GetInt("RelatedFieldID"); }
            set { Set("RelatedFieldID", value.ToString()); }
        }

        public string RelatedFieldStyle
        {
            get { return GetString("RelatedFieldStyle"); }
            set { Set("RelatedFieldStyle", value); }
        }

        public bool IsValidate
        {
            get { return GetBool("IsValidate"); }
            set { Set("IsValidate", value.ToString()); }
        }

        public bool IsRequired
        {
            get { return GetBool("IsRequired"); }
            set { Set("IsRequired", value.ToString()); }
        }

        public int MinNum
        {
            get { return GetInt("MinNum"); }
            set { Set("MinNum", value.ToString()); }
        }

        public int MaxNum
        {
            get { return GetInt("MaxNum"); }
            set { Set("MaxNum", value.ToString()); }
        }

        public ValidateType ValidateType
        {
            get { return ValidateTypeUtils.GetEnumType(GetString("ValidateType")); }
            set { Set("ValidateType", ValidateTypeUtils.GetValue(value)); }
        }

        public string RegExp
        {
            get { return GetString("RegExp"); }
            set { Set("RegExp", value); }
        }

        public string ErrorMessage
        {
            get { return GetString("ErrorMessage"); }
            set { Set("ErrorMessage", value); }
        }
    }
}
