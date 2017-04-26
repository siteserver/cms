using System;
using System.Collections;
//using System.Web.Mail;
using System.Net.Mail;

namespace BaiRong.Core.Net
{
	public class MailUtils : ISmtpMail
	{
		private string _subject;
        private string _body;
        private string _mailDomain;
		private int _mailserverport;
        private string _fromName;
		private string _username;
        private string _password;
		private bool _isHtml;
        private bool _enabledSsl;
        private MailPriority _priority = MailPriority.Normal;
		private ArrayList _recipients = new ArrayList();

        private MailUtils() { }

        private static ISmtpMail instance = null;
        public static ISmtpMail GetInstance()
        {
            if (instance == null)
            {
                instance = new MailUtils();
            }
            return instance;
        }

		/// <summary>
		/// 邮件主题
		/// </summary>
		public string Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				_subject = value;
			}
		}

		/// <summary>
		/// 邮件正文
		/// </summary>
		public string Body
		{
			get
			{
				return _body;
			}
			set
			{
				_body = value;
			}
		}

		/// <summary>
		/// 邮箱域
		/// </summary>
		public string MailDomain
		{
			get
			{
				return _mailDomain;
			}
			set
			{
				_mailDomain = value;
			}
		}

		/// <summary>
		/// 邮件服务器端口号
		/// </summary>	
		public int MailDomainPort
		{
			set
			{
				_mailserverport = value;
			}
			get
			{
				return _mailserverport;
			}
		}

        public string MailFromName
        {
            set
            {
                if (value.Trim() != string.Empty)
                {
                    _fromName = value.Trim();
                }
                else
                {
                    _fromName = string.Empty;
                }
            }
            get
            {
                return _fromName;
            }
        }


		/// <summary>
		/// SMTP认证时使用的用户名
		/// </summary>
		public string MailServerUserName
		{
			set
			{
				if(value.Trim() != string.Empty)
				{
					_username = value.Trim();
				}
				else
				{
					_username = string.Empty;
				}
			}
			get
			{
				return _username;
			}
		}

		/// <summary>
		/// SMTP认证时使用的密码
		/// </summary>
        public string MailServerPassword
		{
			set
			{
				_password = value;
			}
			get
			{
				return _password;
			}
		}	

		/// <summary>
		///  是否Html邮件
		/// </summary>
		public bool IsHtml
		{
			get
			{
				return _isHtml;
			}
			set
			{
                _isHtml = value;
			}
		}

        /// <summary>
        ///  是否SSL
        /// </summary>
        public bool EnabledSsl
        {
            get
            {
                return _enabledSsl;
            }
            set
            {
                _enabledSsl = value;
            }
        }

        public MailPriority Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }

		//收件人的邮箱地址
		public bool AddRecipient(params string[] usernames)
		{
            _recipients.Clear();
            foreach (var username in usernames)
            {
                var theUserName = username.ToLower().Trim();
                if (!_recipients.Contains(theUserName))
                {
                    _recipients.Add(theUserName);
                }
            }

			return true;
		}

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public bool Send(out string errorMessage)
        {
            var from = string.Empty;
            var to = string.Empty;
            errorMessage = string.Empty;

            // 通过SMTP服务器验证
            //myEmail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
            //myEmail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", this.MailServerUserName);
            //myEmail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpaccountname", this.MailServerUserName);
            //myEmail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", this.MailServerPassword);
            //myEmail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpserverport", this.MailDomainPort);

            //System.Web.Mail.SmtpMail.SmtpServer = this.MailDomain;

            var client = new SmtpClient(MailDomain);
            client.UseDefaultCredentials = true;//是否身份验证
            client.Credentials = new System.Net.NetworkCredential(MailServerUserName, MailServerPassword);//身份验证账号密码  主要账号无需后缀名如 123@qq.com  只需填写123 即可。
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Port = MailDomainPort;

            client.EnableSsl = EnabledSsl;

            if (_recipients == null || _recipients.Count == 0)
            {
                errorMessage = "必须设置收件人邮箱!";
                return false;
            }
            else if (_recipients.Count == 1)
            {
                try
                {
                    if (string.IsNullOrEmpty(MailFromName))
                    {
                        from = MailServerUserName;
                    }
                    else
                    {
                        from = $@"""{MailFromName}"" <{MailServerUserName}>";
                    }

                    to = _recipients[0] as string;

                    var myEmail = new MailMessage(from, to, Subject, Body);

                    myEmail.Priority = Priority;
                    myEmail.IsBodyHtml = IsHtml; //邮件形式，.Text、.Html 

                    client.Send(myEmail);
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                    {
                        errorMessage = ex.InnerException.Message;
                    }
                    else
                    {
                        errorMessage = ex.Message;
                    }
                    return false;
                }
            }
            else
            {
                foreach (string userName in _recipients)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(MailFromName))
                        {
                            from = MailServerUserName;
                        }
                        else
                        {
                            from = $@"""{MailFromName}"" <{MailServerUserName}>";
                        }

                        to = userName;

                        var myEmail = new MailMessage(from, to, Subject, Body);

                        myEmail.Priority = Priority;
                        myEmail.IsBodyHtml = IsHtml; //邮件形式，.Text、.Html 

                        client.Send(myEmail);
                        //System.Web.Mail.SmtpMail.Send(myEmail);
                    }
                    catch (Exception ex)
                    {
                        if (ex.InnerException != null)
                        {
                            errorMessage += ex.InnerException.Message + "<br>";
                        }
                        else
                        {
                            errorMessage += ex.Message + "<br>";
                        }
                    }
                }
            }

            return true;
        }
    }
}
