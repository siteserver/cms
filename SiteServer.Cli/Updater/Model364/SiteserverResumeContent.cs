using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverResumeContent
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("styleID")]
        public long StyleId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("jobContentID")]
        public long JobContentId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("isView")]
        public string IsView { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("realName")]
        public string RealName { get; set; }

        [JsonProperty("nationality")]
        public string Nationality { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("mobilePhone")]
        public string MobilePhone { get; set; }

        [JsonProperty("homePhone")]
        public string HomePhone { get; set; }

        [JsonProperty("lastSchoolName")]
        public string LastSchoolName { get; set; }

        [JsonProperty("education")]
        public string Education { get; set; }

        [JsonProperty("idCardType")]
        public string IdCardType { get; set; }

        [JsonProperty("idCardNo")]
        public string IdCardNo { get; set; }

        [JsonProperty("birthday")]
        public string Birthday { get; set; }

        [JsonProperty("marriage")]
        public string Marriage { get; set; }

        [JsonProperty("workYear")]
        public string WorkYear { get; set; }

        [JsonProperty("profession")]
        public string Profession { get; set; }

        [JsonProperty("expectSalary")]
        public string ExpectSalary { get; set; }

        [JsonProperty("availabelTime")]
        public string AvailabelTime { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("exp_Count")]
        public long ExpCount { get; set; }

        [JsonProperty("exp_FromYear")]
        public string ExpFromYear { get; set; }

        [JsonProperty("exp_FromMonth")]
        public string ExpFromMonth { get; set; }

        [JsonProperty("exp_ToYear")]
        public string ExpToYear { get; set; }

        [JsonProperty("exp_ToMonth")]
        public string ExpToMonth { get; set; }

        [JsonProperty("exp_EmployerName")]
        public string ExpEmployerName { get; set; }

        [JsonProperty("exp_Department")]
        public string ExpDepartment { get; set; }

        [JsonProperty("exp_EmployerPhone")]
        public string ExpEmployerPhone { get; set; }

        [JsonProperty("exp_WorkPlace")]
        public string ExpWorkPlace { get; set; }

        [JsonProperty("exp_PositionTitle")]
        public string ExpPositionTitle { get; set; }

        [JsonProperty("exp_Industry")]
        public string ExpIndustry { get; set; }

        [JsonProperty("exp_Summary")]
        public string ExpSummary { get; set; }

        [JsonProperty("exp_Score")]
        public string ExpScore { get; set; }

        [JsonProperty("pro_Count")]
        public long ProCount { get; set; }

        [JsonProperty("pro_FromYear")]
        public string ProFromYear { get; set; }

        [JsonProperty("pro_FromMonth")]
        public string ProFromMonth { get; set; }

        [JsonProperty("pro_ToYear")]
        public string ProToYear { get; set; }

        [JsonProperty("pro_ToMonth")]
        public string ProToMonth { get; set; }

        [JsonProperty("pro_ProjectName")]
        public string ProProjectName { get; set; }

        [JsonProperty("pro_Summary")]
        public string ProSummary { get; set; }

        [JsonProperty("edu_Count")]
        public long EduCount { get; set; }

        [JsonProperty("edu_FromYear")]
        public string EduFromYear { get; set; }

        [JsonProperty("edu_FromMonth")]
        public string EduFromMonth { get; set; }

        [JsonProperty("edu_ToYear")]
        public string EduToYear { get; set; }

        [JsonProperty("edu_ToMonth")]
        public string EduToMonth { get; set; }

        [JsonProperty("edu_SchoolName")]
        public string EduSchoolName { get; set; }

        [JsonProperty("edu_Education")]
        public string EduEducation { get; set; }

        [JsonProperty("edu_Profession")]
        public string EduProfession { get; set; }

        [JsonProperty("edu_Summary")]
        public string EduSummary { get; set; }

        [JsonProperty("tra_Count")]
        public long TraCount { get; set; }

        [JsonProperty("tra_FromYear")]
        public string TraFromYear { get; set; }

        [JsonProperty("tra_FromMonth")]
        public string TraFromMonth { get; set; }

        [JsonProperty("tra_ToYear")]
        public string TraToYear { get; set; }

        [JsonProperty("tra_ToMonth")]
        public string TraToMonth { get; set; }

        [JsonProperty("tra_TrainerName")]
        public string TraTrainerName { get; set; }

        [JsonProperty("tra_TrainerAddress")]
        public string TraTrainerAddress { get; set; }

        [JsonProperty("tra_Lesson")]
        public string TraLesson { get; set; }

        [JsonProperty("tra_Centification")]
        public string TraCentification { get; set; }

        [JsonProperty("tra_Summary")]
        public string TraSummary { get; set; }

        [JsonProperty("lan_Count")]
        public long LanCount { get; set; }

        [JsonProperty("lan_Language")]
        public string LanLanguage { get; set; }

        [JsonProperty("lan_Level")]
        public string LanLevel { get; set; }

        [JsonProperty("ski_Count")]
        public long SkiCount { get; set; }

        [JsonProperty("ski_SkillName")]
        public string SkiSkillName { get; set; }

        [JsonProperty("ski_UsedTimes")]
        public string SkiUsedTimes { get; set; }

        [JsonProperty("ski_Ability")]
        public string SkiAbility { get; set; }

        [JsonProperty("cer_Count")]
        public long CerCount { get; set; }

        [JsonProperty("cer_CertificationName")]
        public string CerCertificationName { get; set; }

        [JsonProperty("cer_EffectiveDate")]
        public string CerEffectiveDate { get; set; }
    }

    public partial class SiteserverResumeContent
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
