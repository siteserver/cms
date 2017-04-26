using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public interface ITagStyleMailSMSBaseInfo
    {
        bool IsSMS { get; set; }

        ETriState SMSReceiver { get; set; }

        string SMSTo { get; set; }

        string SMSFiledName { get; set; }

        bool IsSMSTemplate { get; set; }

        string SMSContent { get; set; }
    }
}
