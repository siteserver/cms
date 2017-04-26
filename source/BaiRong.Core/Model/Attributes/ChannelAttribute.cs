using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
	public class ChannelAttribute
	{
        private ChannelAttribute()
		{
		}
		
        //hidden
		public static string Id = "ID";
        public static string NodeId = "NodeID";
        public static string PublishmentSystemId = "PublishmentSystemID";

        private static List<string> _hiddenAttributes;

	    public static List<string> HiddenAttributes => _hiddenAttributes ?? (_hiddenAttributes = new List<string>
        {
	        Id.ToLower(),
	        NodeId.ToLower(),
	        PublishmentSystemId.ToLower()
	    });
	}
}
