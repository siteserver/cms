using System;
using System.Collections.Generic;

namespace BaiRong.Core.Model
{
    [Serializable]
	public class TableStyleInfo: IComparable<TableStyleInfo>
	{
	    public TableStyleInfo()
		{
            TableStyleId = 0;
            RelatedIdentity = 0;
            TableName = string.Empty;
            AttributeName = string.Empty;
            Taxis = 0;
            DisplayName = string.Empty;
            HelpText = string.Empty;
            IsVisibleInList = false;
            InputType = InputTypeUtils.GetValue(SiteServer.Plugin.Models.InputType.Text);
            DefaultValue = string.Empty;
            IsHorizontal = true;
            ExtendValues = string.Empty;
		}

        public TableStyleInfo(int tableStyleId, int relatedIdentity, string tableName, string attributeName, int taxis, string displayName, string helpText, bool isVisibleInList, string inputType, string defaultValue, bool isHorizontal, string extendValues) 
		{
            TableStyleId = tableStyleId;
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

		public int TableStyleId { get; set; }

	    public int RelatedIdentity { get; set; }

	    public string TableName { get; set; }

	    public string AttributeName { get; set; }

	    public int Taxis { get; set; }

	    public string DisplayName { get; set; }

	    public string HelpText { get; set; }

	    public bool IsVisibleInList { get; set; }

	    public string InputType { get; set; }

	    public string DefaultValue { get; set; }

	    public bool IsHorizontal { get; set; }

	    public string ExtendValues { get; set; }

	    private TableStyleInfoExtend _additional;
        public TableStyleInfoExtend Additional => _additional ?? (_additional = new TableStyleInfoExtend(ExtendValues));

	    public List<TableStyleItemInfo> StyleItems { get; set; }

	    public int CompareTo(TableStyleInfo other)
	    {
            if (other == null) return 1;

            var x = Taxis.CompareTo(other.Taxis);
	        if (x != 0) return x;

	        return string.Compare(AttributeName, other.AttributeName, StringComparison.Ordinal);
	    }
	}
}
