using System;
using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_Form")]
    public class Form : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string Title { get; set; }

        [DataColumn(Length = 2000)]
        public string Description { get; set; }

        [DataColumn]
        public int Taxis { get; set; }

        [DataColumn]
        public bool IsReply { get; set; }

        [DataColumn]
        public int RepliedCount { get; set; }

        [DataColumn]
        public int TotalCount { get; set; }

        public bool IsClosed { get; set; }

        public string ListAttributeNames { get; set; }

        public string SuccessMessage { get; set; }

        public string SuccessCallback { get; set; }

        public bool IsReplyListAll { get; set; }

        public bool IsCaptcha { get; set; }
        
        public bool IsSms { get; set; }

        public int PageSize { get; set; }

        public bool IsTimeout { get; set; }

        public DateTime TimeToStart { get; set; }

        public DateTime TimeToEnd { get; set; }

        //向管理员发送短信通知
        public bool IsAdministratorSmsNotify { get; set; }

        public string AdministratorSmsNotifyTplId { get; set; }

        public string AdministratorSmsNotifyKeys { get; set; }

        public string AdministratorSmsNotifyMobile { get; set; }

        //向管理员发送邮件通知
        public bool IsAdministratorMailNotify { get; set; }

        public string AdministratorMailTitle { get; set; }

        public string AdministratorMailNotifyAddress { get; set; }

        //向用户发送短信通知
        public bool IsUserSmsNotify { get; set; }

        public string UserSmsNotifyTplId { get; set; }

        public string UserSmsNotifyKeys { get; set; }

        public string UserSmsNotifyMobileName { get; set; }
    }
}