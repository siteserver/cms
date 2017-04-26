using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageResumeView : BasePageCms
    {
        public Literal ltlRealName;
        public Literal ltlNationality;
        public Literal ltlGender;
        public Literal ltlEmail;
        public Literal ltlMobilePhone;
        public Literal ltlHomePhone;
        public Literal ltlLastSchoolName;
        public Literal ltlEducation;
        public Literal ltlIDCardType;
        public Literal ltlIDCardNo;
        public Literal ltlBirthday;
        public Literal ltlMarriage;
        public Literal ltlWorkYear;
        public Literal ltlProfession;
        public Literal ltlExpectSalary;
        public Literal ltlAvailabelTime;
        public Literal ltlLocation;
        public Literal ltlImageUrl;
        public Literal ltlSummary;

        public Repeater rptExp;
        public Repeater rptPro;
        public Repeater rptEdu;
        public Repeater rptTra;
        public Repeater rptLan;
        public Repeater rptSki;
        public Repeater rptCer;

        private ResumeContentInfo _contentInfo;

        public static string GetRedirectUrl(int publishmentSystemId, int contentId)
        {
            return PageUtils.GetCmsUrl(nameof(PageResumeView), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"contentID", contentId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            var contentID = TranslateUtils.ToInt(Request.QueryString["contentID"]);
            if (contentID > 0)
            {
                _contentInfo = DataProvider.ResumeContentDao.GetContentInfo(contentID);
                if (_contentInfo != null)
                {
                    DataProvider.ResumeContentDao.SetIsView(TranslateUtils.ToIntList(contentID), true);
                }
            }
            else
            {
                _contentInfo = DataProvider.ResumeContentDao.GetContentInfo(PublishmentSystemId, Request.Form);
            }

            ltlRealName.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.RealName);
            ltlNationality.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.Nationality);
            ltlGender.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.Gender);
            ltlEmail.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.Email);
            ltlMobilePhone.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.MobilePhone);
            ltlHomePhone.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.HomePhone);
            ltlLastSchoolName.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.LastSchoolName);
            ltlEducation.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.Education);
            ltlIDCardType.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.IdCardType);
            ltlIDCardNo.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.IdCardNo);
            ltlBirthday.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.Birthday);
            ltlMarriage.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.Marriage);
            ltlWorkYear.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.WorkYear);
            ltlProfession.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.Profession);
            ltlExpectSalary.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpectSalary);
            ltlAvailabelTime.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.AvailabelTime);
            ltlLocation.Text = _contentInfo.GetExtendedAttribute(ResumeContentAttribute.Location);
            
            if (!string.IsNullOrEmpty(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ImageUrl)))
            {
                ltlImageUrl.Text =
                    $@"<img src=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo,
                        _contentInfo.GetExtendedAttribute(ResumeContentAttribute.ImageUrl))}"" width=""120"" height=""150"" />";
            }
            else
            {
                ltlImageUrl.Text = @"<img src=""images/resume_picture.jpg"" width=""120"" height=""150"" />";
            }
            ltlSummary.Text = StringUtils.ReplaceNewlineToBr(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.Summary));

            var count = TranslateUtils.ToInt(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpCount));
            var array = new int[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = i + 1;
            }
            rptExp.DataSource = array;
            rptExp.ItemDataBound += rptExp_ItemDataBound;
            rptExp.DataBind();

            count = TranslateUtils.ToInt(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ProCount));
            array = new int[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = i + 1;
            }
            rptPro.DataSource = array;
            rptPro.ItemDataBound += rptPro_ItemDataBound;
            rptPro.DataBind();

            count = TranslateUtils.ToInt(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduCount));
            array = new int[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = i + 1;
            }
            rptEdu.DataSource = array;
            rptEdu.ItemDataBound += rptEdu_ItemDataBound;
            rptEdu.DataBind();

            count = TranslateUtils.ToInt(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraCount));
            array = new int[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = i + 1;
            }
            rptTra.DataSource = array;
            rptTra.ItemDataBound += rptTra_ItemDataBound;
            rptTra.DataBind();

            count = TranslateUtils.ToInt(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.LanCount));
            array = new int[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = i + 1;
            }
            rptLan.DataSource = array;
            rptLan.ItemDataBound += rptLan_ItemDataBound;
            rptLan.DataBind();

            count = TranslateUtils.ToInt(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.SkiCount));
            array = new int[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = i + 1;
            }
            rptSki.DataSource = array;
            rptSki.ItemDataBound += rptSki_ItemDataBound;
            rptSki.DataBind();

            count = TranslateUtils.ToInt(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.CerCount));
            array = new int[count];
            for (var i = 0; i < count; i++)
            {
                array[i] = i + 1;
            }
            rptCer.DataSource = array;
            rptCer.ItemDataBound += rptCer_ItemDataBound;
            rptCer.DataBind();
        }

        private void rptExp_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var index = (int)e.Item.DataItem;

                var ltlExp_Date = e.Item.FindControl("ltlExp_Date") as Literal;
                var ltlExp_EmployerName = e.Item.FindControl("ltlExp_EmployerName") as Literal;
                var ltlExp_Department = e.Item.FindControl("ltlExp_Department") as Literal;
                var ltlExp_EmployerPhone = e.Item.FindControl("ltlExp_EmployerPhone") as Literal;
                var ltlExp_WorkPlace = e.Item.FindControl("ltlExp_WorkPlace") as Literal;
                var ltlExp_PositionTitle = e.Item.FindControl("ltlExp_PositionTitle") as Literal;
                var ltlExp_Industry = e.Item.FindControl("ltlExp_Industry") as Literal;
                var ltlExp_Summary = e.Item.FindControl("ltlExp_Summary") as Literal;
                var ltlExp_Score = e.Item.FindControl("ltlExp_Score") as Literal;

                var fromYear = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpFromYear), index));
                var fromMonth = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpFromMonth), index));
                var toYear = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpToYear), index), DateTime.Now.Year);
                var toMonth = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpToMonth), index), DateTime.Now.Month);

                if (fromYear > 0 && fromMonth > 0)
                {
                    ltlExp_Date.Text = $"{fromYear}年{fromMonth}月 到 {toYear}年{toMonth}月";
                }

                ltlExp_EmployerName.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpEmployerName), index);
                ltlExp_Department.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpDepartment), index);
                ltlExp_EmployerPhone.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpEmployerPhone), index);
                ltlExp_WorkPlace.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpWorkPlace), index);
                ltlExp_PositionTitle.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpPositionTitle), index);
                ltlExp_Industry.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpIndustry), index);
                ltlExp_Summary.Text = StringUtils.ReplaceNewlineToBr(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpSummary), index));
                ltlExp_Score.Text = StringUtils.ReplaceNewlineToBr(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ExpScore), index));
            }
        }

        private void rptPro_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var index = (int)e.Item.DataItem;

                var ltlPro_Date = e.Item.FindControl("ltlPro_Date") as Literal;
                var ltlPro_ProjectName = e.Item.FindControl("ltlPro_ProjectName") as Literal;
                var ltlPro_Summary = e.Item.FindControl("ltlPro_Summary") as Literal;

                var fromYear = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ProFromYear), index));
                var fromMonth = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ProFromMonth), index));
                var toYear = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ProToYear), index), DateTime.Now.Year);
                var toMonth = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ProToMonth), index), DateTime.Now.Month);

                if (fromYear > 0 && fromMonth > 0)
                {
                    ltlPro_Date.Text = $"{fromYear}年{fromMonth}月 到 {toYear}年{toMonth}月";
                }

                ltlPro_ProjectName.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ProProjectName), index);
                ltlPro_Summary.Text = StringUtils.ReplaceNewlineToBr(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.ProSummary), index));
            }
        }

        private void rptEdu_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var index = (int)e.Item.DataItem;

                var ltlEdu_Date = e.Item.FindControl("ltlEdu_Date") as Literal;
                var ltlEdu_SchoolName = e.Item.FindControl("ltlEdu_SchoolName") as Literal;
                var ltlEdu_Education = e.Item.FindControl("ltlEdu_Education") as Literal;
                var ltlEdu_Profession = e.Item.FindControl("ltlEdu_Profession") as Literal;
                var ltlEdu_Summary = e.Item.FindControl("ltlEdu_Summary") as Literal;

                var fromYear = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduFromYear), index));
                var fromMonth = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduFromMonth), index));
                var toYear = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduToYear), index), DateTime.Now.Year);
                var toMonth = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduToMonth), index), DateTime.Now.Month);

                if (fromYear > 0 && fromMonth > 0)
                {
                    ltlEdu_Date.Text = $"{fromYear}年{fromMonth}月 到 {toYear}年{toMonth}月";
                }

                ltlEdu_SchoolName.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduSchoolName), index);
                ltlEdu_Education.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduEducation), index);
                ltlEdu_Profession.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduProfession), index);
                ltlEdu_Summary.Text = StringUtils.ReplaceNewlineToBr(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.EduSummary), index));
            }
        }

        private void rptTra_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var index = (int)e.Item.DataItem;

                var ltlTra_Date = e.Item.FindControl("ltlTra_Date") as Literal;
                var ltlTra_TrainerName = e.Item.FindControl("ltlTra_TrainerName") as Literal;
                var ltlTra_TrainerAddress = e.Item.FindControl("ltlTra_TrainerAddress") as Literal;
                var ltlTra_Lesson = e.Item.FindControl("ltlTra_Lesson") as Literal;
                var ltlTra_Centification = e.Item.FindControl("ltlTra_Centification") as Literal;
                var ltlTra_Summary = e.Item.FindControl("ltlTra_Summary") as Literal;

                var fromYear = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraFromYear), index));
                var fromMonth = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraFromMonth), index));
                var toYear = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraToYear), index), DateTime.Now.Year);
                var toMonth = TranslateUtils.ToInt(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraToMonth), index), DateTime.Now.Month);

                if (fromYear > 0 && fromMonth > 0)
                {
                    ltlTra_Date.Text = $"{fromYear}年{fromMonth}月 到 {toYear}年{toMonth}月";
                }

                ltlTra_TrainerName.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraTrainerName), index);
                ltlTra_TrainerAddress.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraTrainerAddress), index);
                ltlTra_Lesson.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraLesson), index);
                ltlTra_Centification.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraCentification), index);
                ltlTra_Summary.Text = StringUtils.ReplaceNewlineToBr(ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.TraSummary), index));
            }
        }

        private void rptLan_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var index = (int)e.Item.DataItem;

                var ltlLan_Language = e.Item.FindControl("ltlLan_Language") as Literal;
                var ltlLan_Level = e.Item.FindControl("ltlLan_Level") as Literal;

                ltlLan_Language.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.LanLanguage), index);
                ltlLan_Level.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.LanLevel), index);
            }
        }

        private void rptSki_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var index = (int)e.Item.DataItem;

                var ltlSki_SkillName = e.Item.FindControl("ltlSki_SkillName") as Literal;
                var ltlSki_UsedTimes = e.Item.FindControl("ltlSki_UsedTimes") as Literal;
                var ltlSki_Ability = e.Item.FindControl("ltlSki_Ability") as Literal;

                ltlSki_SkillName.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.SkiSkillName), index);
                ltlSki_UsedTimes.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.SkiUsedTimes), index);
                ltlSki_Ability.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.SkiAbility), index);
            }
        }

        private void rptCer_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var index = (int)e.Item.DataItem;

                var ltlCer_CertificationName = e.Item.FindControl("ltlCer_CertificationName") as Literal;
                var ltlCer_EffectiveDate = e.Item.FindControl("ltlCer_EffectiveDate") as Literal;

                ltlCer_CertificationName.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.CerCertificationName), index);
                ltlCer_EffectiveDate.Text = ResumeContentAttribute.GetAttributeValue(_contentInfo.GetExtendedAttribute(ResumeContentAttribute.CerEffectiveDate), index);
            }
        }
    }
}
