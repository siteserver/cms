using System.Collections.Generic;

namespace BaiRong.Core.Model.Attributes
{
	public class ResumeContentAttribute
	{
        protected ResumeContentAttribute()
		{
		}

		//hidden
		public static string Id = "ID";
        public static string PublishmentSystemId = "PublishmentSystemID";
        public static string JobContentId = "JobContentID";
        public static string UserName = "UserName";
        public static string IsView = "IsView";
        public static string AddDate = "AddDate";

        //basic
        public const string RealName = "RealName";
        public const string Nationality = "Nationality";
        public const string Gender = "Gender";
        public const string Email = "Email";
        public const string MobilePhone = "MobilePhone";
        public const string HomePhone = "HomePhone";
        public const string LastSchoolName = "LastSchoolName";
        public const string Education = "Education";
        public const string IdCardType = "IDCardType";
        public const string IdCardNo = "IDCardNo";
        public const string Birthday = "Birthday";
        public const string Marriage = "Marriage";
        public const string WorkYear = "WorkYear";
        public const string Profession = "Profession";
        public const string ExpectSalary = "ExpectSalary";
        public const string AvailabelTime = "AvailabelTime";
        public const string Location = "Location";
        public const string ImageUrl = "ImageUrl";
        public const string Summary = "Summary";

        //extend
        public const string ExpCount = "Exp_Count";
        public const string ExpFromYear = "Exp_FromYear";
        public const string ExpFromMonth = "Exp_FromMonth";
        public const string ExpToYear = "Exp_ToYear";
        public const string ExpToMonth = "Exp_ToMonth";
        public const string ExpEmployerName = "Exp_EmployerName";
        public const string ExpDepartment = "Exp_Department";
        public const string ExpEmployerPhone = "Exp_EmployerPhone";
        public const string ExpWorkPlace = "Exp_WorkPlace";
        public const string ExpPositionTitle = "Exp_PositionTitle";
        public const string ExpIndustry = "Exp_Industry";
        public const string ExpSummary = "Exp_Summary";
        public const string ExpScore = "Exp_Score";

        public const string ProCount = "Pro_Count";
        public const string ProFromYear = "Pro_FromYear";
        public const string ProFromMonth = "Pro_FromMonth";
        public const string ProToYear = "Pro_ToYear";
        public const string ProToMonth = "Pro_ToMonth";
        public const string ProProjectName = "Pro_ProjectName";
        public const string ProSummary = "Pro_Summary";
        public const string EduCount = "Edu_Count";
        public const string EduFromYear = "Edu_FromYear";
        public const string EduFromMonth = "Edu_FromMonth";
        public const string EduToYear = "Edu_ToYear";
        public const string EduToMonth = "Edu_ToMonth";
        public const string EduSchoolName = "Edu_SchoolName";
        public const string EduEducation = "Edu_Education";
        public const string EduProfession = "Edu_Profession";
        public const string EduSummary = "Edu_Summary";

        public const string TraCount = "Tra_Count";
        public const string TraFromYear = "Tra_FromYear";
        public const string TraFromMonth = "Tra_FromMonth";
        public const string TraToYear = "Tra_ToYear";
        public const string TraToMonth = "Tra_ToMonth";
        public const string TraTrainerName = "Tra_TrainerName";
        public const string TraTrainerAddress = "Tra_TrainerAddress";
        public const string TraLesson = "Tra_Lesson";
        public const string TraCentification = "Tra_Centification";
        public const string TraSummary = "Tra_Summary";

        public const string LanCount = "Lan_Count";
        public const string LanLanguage = "Lan_Language";
        public const string LanLevel = "Lan_Level";

        public const string SkiCount = "Ski_Count";
        public const string SkiSkillName = "Ski_SkillName";
        public const string SkiUsedTimes = "Ski_UsedTimes";
        public const string SkiAbility = "Ski_Ability";

        public const string CerCount = "Cer_Count";
        public const string CerCertificationName = "Cer_CertificationName";
        public const string CerEffectiveDate = "Cer_EffectiveDate";

        public static List<string> AllAttributes
        {
            get
            {
                var arraylist = new List<string>(HiddenAttributes);
                arraylist.AddRange(BasicAttributes);
                arraylist.AddRange(ExtendAttributes);
                return arraylist;
            }
        }

        private static List<string> _hiddenAttributes;
        public static List<string> HiddenAttributes => _hiddenAttributes ?? (_hiddenAttributes = new List<string>
        {
            Id.ToLower(),
            PublishmentSystemId.ToLower(),
            JobContentId.ToLower(),
            UserName.ToLower(),
            IsView.ToLower(),
            AddDate.ToLower()
        });

	    private static List<string> _basicAttributes;
        public static List<string> BasicAttributes => _basicAttributes ?? (_basicAttributes = new List<string>
        {
            RealName.ToLower(),
            Nationality.ToLower(),
            Gender.ToLower(),
            Email.ToLower(),
            MobilePhone.ToLower(),
            HomePhone.ToLower(),
            LastSchoolName.ToLower(),
            Education.ToLower(),
            IdCardType.ToLower(),
            IdCardNo.ToLower(),
            Birthday.ToLower(),
            Marriage.ToLower(),
            WorkYear.ToLower(),
            Profession.ToLower(),
            ExpectSalary.ToLower(),
            AvailabelTime.ToLower(),
            Location.ToLower(),
            ImageUrl.ToLower(),
            Summary.ToLower()
        });

	    private static List<string> _extendAttributes;
        public static List<string> ExtendAttributes => _extendAttributes ?? (_extendAttributes = new List<string>
        {
            ExpCount.ToLower(),
            ExpFromYear.ToLower(),
            ExpFromMonth.ToLower(),
            ExpToYear.ToLower(),
            ExpToMonth.ToLower(),
            ExpEmployerName.ToLower(),
            ExpDepartment.ToLower(),
            ExpEmployerPhone.ToLower(),
            ExpWorkPlace.ToLower(),
            ExpPositionTitle.ToLower(),
            ExpIndustry.ToLower(),
            ExpSummary.ToLower(),
            ExpScore.ToLower(),
            ProCount.ToLower(),
            ProFromYear.ToLower(),
            ProFromMonth.ToLower(),
            ProToYear.ToLower(),
            ProToMonth.ToLower(),
            ProProjectName.ToLower(),
            ProSummary.ToLower(),
            EduCount.ToLower(),
            EduFromYear.ToLower(),
            EduFromMonth.ToLower(),
            EduToYear.ToLower(),
            EduToMonth.ToLower(),
            EduSchoolName.ToLower(),
            EduEducation.ToLower(),
            EduProfession.ToLower(),
            EduSummary.ToLower(),
            TraCount.ToLower(),
            TraFromYear.ToLower(),
            TraFromMonth.ToLower(),
            TraToYear.ToLower(),
            TraToMonth.ToLower(),
            TraTrainerName.ToLower(),
            TraTrainerAddress.ToLower(),
            TraLesson.ToLower(),
            TraCentification.ToLower(),
            TraSummary.ToLower(),
            LanCount.ToLower(),
            LanLanguage.ToLower(),
            LanLevel.ToLower(),
            SkiCount.ToLower(),
            SkiSkillName.ToLower(),
            SkiUsedTimes.ToLower(),
            SkiAbility.ToLower(),
            CerCount.ToLower(),
            CerCertificationName.ToLower(),
            CerEffectiveDate.ToLower()
        });

	    public static string GetAttributeName(string attributeName, int index)
        {
            return attributeName + "_" + index;
        }

        public static string GetAttributeValue(string attributeValue, int index)
        {
            var collection = TranslateUtils.StringCollectionToStringCollection(attributeValue, '&');
            if (index <= collection.Count)
            {
                return collection[index - 1];
            }
            return string.Empty;
        }
	}
}