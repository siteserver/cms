namespace BaiRong.Core.Model
{
	public class UserGroupInfo
	{
	    public UserGroupInfo(int groupId, string groupName, bool isDefault, string description, string extendValues) 
		{
            GroupId = groupId;
            GroupName = groupName;
            IsDefault = isDefault;
            Description = description;
            ExtendValues = extendValues;
		}

        public int GroupId { get; set; }

	    public string GroupName { get; set; }

	    public bool IsDefault { get; set; }

	    public string Description { get; set; }

	    public string ExtendValues { get; set; }

	    private UserGroupInfoExtend _additional;
        public UserGroupInfoExtend Additional
        {
            get
            {
                if (_additional == null)
                {
                    _additional = new UserGroupInfoExtend(ExtendValues);
                }
                return _additional;
            }
            set
            {
                _additional = value;
            }
        }
	}
}
