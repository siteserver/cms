using BaiRong.Core.Model;

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
            ID = 0;
            PublishmentSystemID = 0;
            ContentID = 0;
            SmallUrl = string.Empty;
            MiddleUrl = string.Empty;
            LargeUrl = string.Empty;
            Taxis = 0;
            Description = string.Empty;
        }

        public PhotoInfo(int id, int publishmentSystemID, int contentID, string smallUrl, string middleUrl, string largeUrl, int taxis, string description)
        {
            ID = id;
            PublishmentSystemID = publishmentSystemID;
            ContentID = contentID;
            SmallUrl = smallUrl;
            MiddleUrl = middleUrl;
            LargeUrl = largeUrl;
            Taxis = taxis;
            Description = description;
        }

        public int ID
        {
            get { return GetInt("ID", 0); }
            set { SetExtendedAttribute("ID", value.ToString()); }
        }

        public int PublishmentSystemID
        {
            get { return GetInt("PublishmentSystemID", 0); }
            set { SetExtendedAttribute("PublishmentSystemID", value.ToString()); }
        }

        public int ContentID
        {
            get { return GetInt("ContentID", 0); }
            set { SetExtendedAttribute("ContentID", value.ToString()); }
        }

        public string SmallUrl
        {
            get { return GetExtendedAttribute("SmallUrl"); }
            set { SetExtendedAttribute("SmallUrl", value); }
        }

        public string MiddleUrl
        {
            get { return GetExtendedAttribute("MiddleUrl"); }
            set { SetExtendedAttribute("MiddleUrl", value); }
        }

        public string LargeUrl
        {
            get { return GetExtendedAttribute("LargeUrl"); }
            set { SetExtendedAttribute("LargeUrl", value); }
        }

        public int Taxis
        {
            get { return GetInt("Taxis", 0); }
            set { SetExtendedAttribute("Taxis", value.ToString()); }
        }

        public string Description
        {
            get { return GetExtendedAttribute("Description"); }
            set { SetExtendedAttribute("Description", value); }
        }

    }
}
