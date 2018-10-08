using System;
using System.Collections.Generic;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
    [Serializable]
	public class TableStyleInfo
	{
	    public TableStyleInfo()
		{
            Id = 0;
            RelatedIdentity = 0;
            TableName = string.Empty;
            AttributeName = string.Empty;
            Taxis = 0;
            DisplayName = string.Empty;
            HelpText = string.Empty;
            IsVisibleInList = false;
            InputType = InputType.Text;
            DefaultValue = string.Empty;
            IsHorizontal = true;
            ExtendValues = string.Empty;
		}

        public TableStyleInfo(int id, int relatedIdentity, string tableName, string attributeName, int taxis, string displayName, string helpText, bool isVisibleInList, InputType inputType, string defaultValue, bool isHorizontal, string extendValues) 
		{
            Id = id;
            RelatedIdentity = relatedIdentity;
            TableName = tableName;
            AttributeName = attributeName;
            Taxis = taxis;
            DisplayName = displayName;
            HelpText = helpText;
            IsVisibleInList = isVisibleInList;
			InputType = inputType;
            DefaultValue = defaultValue;
            IsHorizontal = isHorizontal;
            ExtendValues = extendValues;
		}

		public int Id { get; set; }

	    public int RelatedIdentity { get; set; }

	    public string TableName { get; set; }

	    public string AttributeName { get; set; }

	    public int Taxis { get; set; }

	    public string DisplayName { get; set; }

	    public string HelpText { get; set; }

	    public bool IsVisibleInList { get; set; }

	    public InputType InputType { get; set; }

	    public string DefaultValue { get; set; }

	    public bool IsHorizontal { get; set; }

	    private string _extendValues;

	    public string ExtendValues
	    {
	        get => _extendValues;
	        set
	        {
	            _additional = null;
	            _extendValues = value;
	        }
	    }

	    private TableStyleInfoExtend _additional;
        public TableStyleInfoExtend Additional => _additional ?? (_additional = new TableStyleInfoExtend(ExtendValues));

	    public List<TableStyleItemInfo> StyleItems { get; set; }
	}
}
