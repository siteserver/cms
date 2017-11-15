namespace SiteServer.CMS.Model
{
	public class RelatedFieldInfo
	{
	    public RelatedFieldInfo()
		{
            RelatedFieldId = 0;
            RelatedFieldName = string.Empty;
			PublishmentSystemId = 0;
            TotalLevel = 0;
            Prefixes = string.Empty;
            Suffixes = string.Empty;
		}

        public RelatedFieldInfo(int relatedFieldId, string relatedFieldName, int publishmentSystemId, int totalLevel, string prefixes, string suffixes)
		{
            RelatedFieldId = relatedFieldId;
            RelatedFieldName = relatedFieldName;
            PublishmentSystemId = publishmentSystemId;
            TotalLevel = totalLevel;
            Prefixes = prefixes;
            Suffixes = suffixes;
		}

        public int RelatedFieldId { get; set; }

	    public string RelatedFieldName { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public int TotalLevel { get; set; }

	    public string Prefixes { get; set; }

	    public string Suffixes { get; set; }
	}
}
