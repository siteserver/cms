namespace BaiRong.Core.Model
{
	public class UserBindingInfo
	{
	    public UserBindingInfo()
		{
            UserName = string.Empty;
            BindingType = string.Empty;
            BindingId = 0;
            BindingName = string.Empty;
		}

        public UserBindingInfo(string userName, string bindingType, int bindingId, string bindingName) 
		{
            UserName = userName;
            BindingType = bindingType;
            BindingId = bindingId;
            BindingName = bindingName;
		}

        public string UserName { get; set; }

	    public string BindingType { get; set; }

	    public int BindingId { get; set; }

	    public string BindingName { get; set; }
	}
}
