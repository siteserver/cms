using System;

namespace SiteServer.CMS.Model
{
	[Serializable]
	public class SeoMetaInfo
	{
		private int _seoMetaId;
		private int _publishmentSystemId;
		private string _seoMetaName;
        private bool _isDefault;
		private string _pageTitle;
        private string _keywords;
		private string _description;
        private string _copyright;
		private string _author;
		private string _email;
		private string _language;
		private string _charset;
		private string _distribution;
		private string _rating;
		private string _robots;
		private string _revisitAfter;
		private string _expires;

		public SeoMetaInfo()
		{
			_seoMetaId = 0;
			_publishmentSystemId = 0;
			_seoMetaName = string.Empty;
			_isDefault = false;
			_pageTitle = string.Empty;
            _keywords = string.Empty;
			_description = string.Empty;
            _copyright = string.Empty;
			_author = string.Empty;
			_email = string.Empty;
			_language = string.Empty;
			_charset = string.Empty;
			_distribution = string.Empty;
			_rating = string.Empty;
			_robots = string.Empty;
			_revisitAfter = string.Empty;
			_expires = string.Empty;
		}

        public SeoMetaInfo(int seoMetaId, int publishmentSystemId, string seoMetaName, bool isDefault, string pageTitle, string keywords, string description, string copyright, string author, string email, string language, string charset, string distribution, string rating, string robots, string revisitAfter, string expires)
		{
			_seoMetaId = seoMetaId;
			_publishmentSystemId = publishmentSystemId;
			_seoMetaName = seoMetaName;
			_isDefault = isDefault;
            _pageTitle = pageTitle;
            _keywords = keywords;
			_description = description;
            _copyright = copyright;
			_author = author;
			_email = email;
			_language = language;
			_charset = charset;
			_distribution = distribution;
			_rating = rating;
			_robots = robots;
			_revisitAfter = revisitAfter;
			_expires = expires;
		}

		public int SeoMetaId
		{
			get{ return _seoMetaId; }
			set{ _seoMetaId = value; }
		}

		public int PublishmentSystemId
		{
			get{ return _publishmentSystemId; }
			set{ _publishmentSystemId = value; }
		}

		public string SeoMetaName
		{
			get{ return _seoMetaName; }
			set{ _seoMetaName = value; }
		}

        public bool IsDefault
		{
			get{ return _isDefault; }
			set{ _isDefault = value; }
		}

        public string PageTitle
		{
            get { return _pageTitle; }
            set { _pageTitle = value; }
		}

        public string Keywords
        {
            get { return _keywords; }
            set { _keywords = value; }
        }

		public string Description
		{
			get{ return _description; }
			set{ _description = value; }
		}

        public string Copyright
        {
            get { return _copyright; }
            set { _copyright = value; }
        }

		public string Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public string Email
		{
			get{ return _email; }
			set{ _email = value; }
		}

		public string Language
		{
			get{ return _language; }
			set{ _language = value; }
		}

		public string Charset
		{
			get{ return _charset; }
			set{ _charset = value; }
		}

		public string Distribution
		{
			get{ return _distribution; }
			set{ _distribution = value; }
		}

		public string Rating
		{
			get{ return _rating; }
			set{ _rating = value; }
		}

		public string Robots
		{
			get{ return _robots; }
			set{ _robots = value; }
		}

		public string RevisitAfter
		{
			get{ return _revisitAfter; }
			set{ _revisitAfter = value; }
		}

		public string Expires
		{
			get{ return _expires; }
			set{ _expires = value; }
		}
	}
}
