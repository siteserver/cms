using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
	public class JobContentAttribute
	{
        protected JobContentAttribute()
		{
		}

        //system
        public const string Department = "Department";
        public const string Location = "Location";
        public const string NumberOfPeople = "NumberOfPeople";
        public const string Responsibility = "Responsibility";
        public const string Requirement = "Requirement";
        public const string IsUrgent = "IsUrgent";

        public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>(ContentAttribute.AllAttributes);
                arraylist.AddRange(SystemAttributes);
                return arraylist;
            }
        }

        private static List<string> _systemAttributes;
        public static List<string> SystemAttributes => _systemAttributes ?? (_systemAttributes = new List<string>
        {
            Department.ToLower(),
            Location.ToLower(),
            NumberOfPeople.ToLower(),
            Responsibility.ToLower(),
            Requirement.ToLower(),
            IsUrgent.ToLower()
        });

	    private static List<string> _excludeAttributes;

	    public static List<string> ExcludeAttributes
	        => _excludeAttributes ?? (_excludeAttributes = new List<string>(ContentAttribute.ExcludeAttributes)
	        {
	            IsUrgent.ToLower()
	        });
	}
}
