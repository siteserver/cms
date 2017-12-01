namespace BaiRong.Core.Model
{
	public class TagInfo
	{
	    public TagInfo()
		{
            TagId = 0;
            PublishmentSystemId = 0;
            ContentIdCollection = string.Empty;
            Tag = string.Empty;
            UseNum = 0;
		}

        public TagInfo(int tagId, int publishmentSystemId, string contentIdCollection, string tag, int useNum) 
		{
            TagId = tagId;
            PublishmentSystemId = publishmentSystemId;
            ContentIdCollection = contentIdCollection;
            Tag = tag;
            UseNum = useNum;
		}

        public int TagId { get; set; }

	    public int PublishmentSystemId { get; set; }

	    public string ContentIdCollection { get; set; }

	    public string Tag { get; set; }

	    public int UseNum { get; set; }

	    public int Level { get; set; } = 0;
	}
}
