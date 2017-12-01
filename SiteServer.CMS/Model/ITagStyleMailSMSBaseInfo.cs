using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Model
{
    public interface ITagStyleMailSmsBaseInfo
    {
        bool IsSms { get; set; }

        ETriState SmsReceiver { get; set; }

        string SmsTo { get; set; }

        string SmsFiledName { get; set; }

        bool IsSmsTemplate { get; set; }

        string SmsContent { get; set; }
    }
}
