using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Model
{
    public class AuxiliaryTableInfo
	{
	    public AuxiliaryTableInfo()
		{
			TableEnName = string.Empty;
			TableCnName = string.Empty;
			AttributeNum = 0;
			AuxiliaryTableType = EAuxiliaryTableType.BackgroundContent;
			IsCreatedInDb = false;
			IsChangedAfterCreatedInDb = false;
            IsDefault = false;
			Description = string.Empty;
		}

        public AuxiliaryTableInfo(string tableEnName, string tableCnName, int attributeNum, EAuxiliaryTableType auxiliaryTableType, bool isCreatedInDb, bool isChangedAfterCreatedInDb, bool isDefault, string description) 
		{
			TableEnName = tableEnName;
			TableCnName = tableCnName;
			AttributeNum = attributeNum;
			AuxiliaryTableType = auxiliaryTableType;
			IsCreatedInDb = isCreatedInDb;
			IsChangedAfterCreatedInDb = isChangedAfterCreatedInDb;
            IsDefault = isDefault;
			Description = description;
		}

		public string TableEnName { get; set; }

	    public string TableCnName { get; set; }

	    public int AttributeNum { get; set; }

	    public EAuxiliaryTableType AuxiliaryTableType { get; set; }

	    public bool IsCreatedInDb { get; set; }

	    public bool IsChangedAfterCreatedInDb { get; set; }

	    public bool IsDefault { get; set; }

	    public string Description { get; set; }
	}
}
