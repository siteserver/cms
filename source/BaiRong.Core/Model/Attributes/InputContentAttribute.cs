using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
	public class InputContentAttribute
	{
        protected InputContentAttribute()
		{
		}

		//hidden
		public static string Id = "ID";
        public static string InputId = "InputID";
		public static string Taxis = "Taxis";
		public static string IsChecked = "IsChecked";
        public static string UserName = "UserName";
        public static string IpAddress = "IPAddress";
        public static string AddDate = "AddDate";
        public static string Reply = "Reply";
        public static string SettingsXml = "SettingsXML";

        public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>(HiddenAttributes);
                return arraylist;
            }
        }

        private static List<string> _hiddenAttributes;
        public static List<string> HiddenAttributes => _hiddenAttributes ?? (_hiddenAttributes = new List<string>
        {
            Id.ToLower(),
            InputId.ToLower(),
            Taxis.ToLower(),
            IsChecked.ToLower(),
            UserName.ToLower(),
            IpAddress.ToLower(),
            AddDate.ToLower(),
            Reply.ToLower(),
            SettingsXml.ToLower()
        });
	}
}