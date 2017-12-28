using System.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;

namespace SiteServer.CMS.Model
{
	public class BackgroundContentInfo : ContentInfo
	{
		public BackgroundContentInfo()
		{
            SubTitle = string.Empty;
            ImageUrl = string.Empty;
            VideoUrl = string.Empty;
            FileUrl = string.Empty;
            Summary = string.Empty;
            Author = string.Empty;
            Source = string.Empty;
            Content = string.Empty;
		}

        public BackgroundContentInfo(object dataItem)
            : base(dataItem)
		{
		}

        public BackgroundContentInfo(IDataReader rdr)
            : base(rdr)
        {
        }

        public string SubTitle
		{
            get { return GetString(BackgroundContentAttribute.SubTitle); }
            set { Set(BackgroundContentAttribute.SubTitle, value); }
		}

        public string ImageUrl
		{
            get { return GetString(BackgroundContentAttribute.ImageUrl); }
            set { Set(BackgroundContentAttribute.ImageUrl, value); }
		}

        public string VideoUrl
        {
            get { return GetString(BackgroundContentAttribute.VideoUrl); }
            set { Set(BackgroundContentAttribute.VideoUrl, value); }
        }

        public string FileUrl
        {
            get { return GetString(BackgroundContentAttribute.FileUrl); }
            set { Set(BackgroundContentAttribute.FileUrl, value); }
        }

        public string Summary
        {
            get { return GetString(BackgroundContentAttribute.Summary); }
            set { Set(BackgroundContentAttribute.Summary, value); }
        }

        public string Author
		{
            get { return GetString(BackgroundContentAttribute.Author); }
            set { Set(BackgroundContentAttribute.Author, value); }
		}

        public string Source
		{
            get { return GetString(BackgroundContentAttribute.Source); }
            set { Set(BackgroundContentAttribute.Source, value); }
		}

        public string Content
		{
            get { return GetString(BackgroundContentAttribute.Content); }
            set { Set(BackgroundContentAttribute.Content, value); }
		}
	}
}
