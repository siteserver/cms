using BaiRong.Core.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
    public class PhotoInfo : ExtendedAttributes
    {
        public const string TableName = "siteserver_Photo";

        public PhotoInfo(object dataItem)
            : base(dataItem)
        {
        }

        public PhotoInfo()
        {
            Id = 0;
            PublishmentSystemId = 0;
            ContentId = 0;
            SmallUrl = string.Empty;
            MiddleUrl = string.Empty;
            LargeUrl = string.Empty;
            Taxis = 0;
            Description = string.Empty;
        }

        public PhotoInfo(int id, int publishmentSystemId, int contentId, string smallUrl, string middleUrl, string largeUrl, int taxis, string description)
        {
            Id = id;
            PublishmentSystemId = publishmentSystemId;
            ContentId = contentId;
            SmallUrl = smallUrl;
            MiddleUrl = middleUrl;
            LargeUrl = largeUrl;
            Taxis = taxis;
            Description = description;
        }

        public int Id
        {
            get { return GetInt("ID"); }
            set { Set("ID", value.ToString()); }
        }

        public int PublishmentSystemId
        {
            get { return GetInt("PublishmentSystemID"); }
            set { Set("PublishmentSystemID", value.ToString()); }
        }

        public int ContentId
        {
            get { return GetInt("ContentID"); }
            set { Set("ContentID", value.ToString()); }
        }

        public string SmallUrl
        {
            get { return GetString("SmallUrl"); }
            set { Set("SmallUrl", value); }
        }

        public string MiddleUrl
        {
            get { return GetString("MiddleUrl"); }
            set { Set("MiddleUrl", value); }
        }

        public string LargeUrl
        {
            get { return GetString("LargeUrl"); }
            set { Set("LargeUrl", value); }
        }

        public int Taxis
        {
            get { return GetInt("Taxis"); }
            set { Set("Taxis", value.ToString()); }
        }

        public string Description
        {
            get { return GetString("Description"); }
            set { Set("Description", value); }
        }

    }
}
