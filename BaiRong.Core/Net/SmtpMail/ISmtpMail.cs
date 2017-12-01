//using System.Web.Mail;
using System.Net.Mail;

namespace BaiRong.Core.Net
{
    public interface ISmtpMail
	{
        int MailDomainPort { set;}

        bool IsHtml { set;get; }

        bool EnabledSsl { set; get; }

        string Subject { set;get;}

        string Body { set;get;}

        string MailDomain { set;}

        string MailFromName { set; }

        string MailServerUserName { set;}

        string MailServerPassword { set;}

        MailPriority Priority { set;}

        bool AddRecipient(params string[] usernames);

        bool Send(out string errorMessage);
	}
}
