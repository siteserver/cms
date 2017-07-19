using System.Collections.Generic;
using BaiRong.Core.Model.Enumerations;
using SiteServer.Plugin;

namespace SiteServer.CMS.Model
{
	public class ContentModelInfo
	{
	    public ContentModelInfo()
		{
            PluginId = string.Empty;
            ModelId = string.Empty;
            ModelName = string.Empty;
            TableName = string.Empty;
            TableType = EAuxiliaryTableType.BackgroundContent;
            IconUrl = string.Empty;
		    Links = null;
		}

        public ContentModelInfo(string pluginId, string modelId, string modelName,string tableName, EAuxiliaryTableType tableType, string iconUrl, List<PluginContentLink> links)
		{
            PluginId = pluginId;
            ModelId = modelId;
            ModelName = modelName;
            TableName = tableName;
            TableType = tableType;
            IconUrl = iconUrl;
		    Links = links;
		}

        public string PluginId { get; set; }

        public string ModelId { get; set; }

	    public string ModelName { get; set; }

	    public string TableName { get; set; }

	    public EAuxiliaryTableType TableType { get; set; }

        public string IconUrl { get; set; }

        public List<PluginContentLink> Links { get; set; }
    }
}
