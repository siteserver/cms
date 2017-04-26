using System;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Model
{
    public class UserConfigInfo : ExtendedAttributes
    {
        public UserConfigInfo(string systemConfig)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(systemConfig);
            SetExtendedAttribute(nameValueCollection);
        }

        public override string ToString()
        {
            return TranslateUtils.NameValueCollectionToString(Attributes);
        }

        /****************用户中心显示设置********************/

        public bool IsEnable
        {
            get { return GetBool("IsEnable", true); }
            set
            {
                SetExtendedAttribute("IsEnable", value.ToString());
            }
        }

        public string Title
        {
            get { return GetString("Title", "用户中心"); }
            set { SetExtendedAttribute("Title", value); }
        }

        public string Copyright
        {
            get { return GetString("Copyright", "Copyright©" + DateTime.Now.Year + " All Rights Reserved"); }
            set { SetExtendedAttribute("Copyright", value); }
        }

        public string BeianNo
        {
            get { return GetString("BeianNo", "京ICP备10013847号"); }
            set { SetExtendedAttribute("BeianNo", value); }
        }

        public string LogoUrl
        {
            get { return GetString("LogoUrl", PageUtils.AddProtocolToUrl(PageUtils.GetUserFilesUrl(string.Empty, "home_logo.png"))); }
            set { SetExtendedAttribute("LogoUrl", value); }
        }

        /****************用户投稿设置********************/

        public bool IsWritingEnabled
        {
            get { return GetBool("IsWritingEnabled", false); }
            set { SetExtendedAttribute("IsWritingEnabled", value.ToString()); }
        }

        /****************用户中心注册设置********************/

        public bool IsRegisterAllowed
        {
            get { return GetBool("IsRegisterAllowed", true); }
            set { SetExtendedAttribute("IsRegisterAllowed", value.ToString()); }
        }

        public EPasswordFormat RegisterPasswordFormat
        {
            get { return EPasswordFormatUtils.GetEnumType(GetString("RegisterPasswordFormat", string.Empty)); }
            set { SetExtendedAttribute("RegisterPasswordFormat", EPasswordFormatUtils.GetValue(value)); }
        }

        public int RegisterPasswordMinLength
        {
            get { return GetInt("RegisterPasswordMinLength", 6); }
            set { SetExtendedAttribute("RegisterPasswordMinLength", value.ToString()); }
        }

        public EUserPasswordRestriction RegisterPasswordRestriction
        {
            get { return EUserPasswordRestrictionUtils.GetEnumType(GetString("RegisterPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(EUserPasswordRestriction.LetterAndDigit))); }
            set { SetExtendedAttribute("RegisterPasswordRestriction", EUserPasswordRestrictionUtils.GetValue(value)); }
        }

        public EUserVerifyType RegisterVerifyType
        {
            get { return EUserVerifyTypeUtils.GetEnumType(GetString("RegisterVerifyType", EUserVerifyTypeUtils.GetValue(EUserVerifyType.Mobile))); }
            set { SetExtendedAttribute("RegisterVerifyType", EUserVerifyTypeUtils.GetValue(value)); }
        }

        public int RegisterMinMinutesOfIpAddress
        {
            get
            {
                return GetInt("RegisterMinMinutesOfIpAddress", 0);
            }
            set
            {
                SetExtendedAttribute("RegisterMinMinutesOfIpAddress", value.ToString());
            }
        }

        /****************文件上传设置********************/

        public string UploadDirectoryName
        {
            get { return GetString("UploadDirectoryName", "upload"); }
            set { SetExtendedAttribute("UploadDirectoryName", value); }
        }

        public string UploadDateFormatString
        {
            get { return GetString("UploadDateFormatString", EDateFormatTypeUtils.GetValue(EDateFormatType.Month)); }
            set { SetExtendedAttribute("UploadDateFormatString", value); }
        }

        public bool IsUploadChangeFileName
        {
            get { return GetBool("IsUploadChangeFileName", true); }
            set { SetExtendedAttribute("IsUploadChangeFileName", value.ToString()); }
        }

        public int UploadMonthMaxSize
        {
            get { return GetInt("UploadMonthMaxSize", 122880); }  //120M
            set { SetExtendedAttribute("UploadMonthMaxSize", value.ToString()); }
        }

        public string UploadImageTypeCollection
        {
            get { return GetString("UploadImageTypeCollection", "gif|jpg|jpeg|bmp|png|swf|flv"); }
            set { SetExtendedAttribute("UploadImageTypeCollection", value); }
        }

        public int UploadImageTypeMaxSize
        {
            get { return GetInt("UploadImageTypeMaxSize", 2048); }
            set { SetExtendedAttribute("UploadImageTypeMaxSize", value.ToString()); }
        }

        public string UploadMediaTypeCollection
        {
            get { return GetString("UploadMediaTypeCollection", "rm|rmvb|mp3|flv|wav|mid|midi|ra|avi|mpg|mpeg|asf|asx|wma|mov"); }
            set { SetExtendedAttribute("UploadMediaTypeCollection", value); }
        }

        public int UploadMediaTypeMaxSize
        {
            get { return GetInt("UploadMediaTypeMaxSize", 5120); }
            set { SetExtendedAttribute("UploadMediaTypeMaxSize", value.ToString()); }
        }

        public string UploadFileTypeCollection
        {
            get { return GetString("UploadFileTypeCollection", "zip|rar|7z|txt|doc|docx|ppt|pptx|xls|xlsx|pdf"); }
            set { SetExtendedAttribute("UploadFileTypeCollection", value); }
        }

        public int UploadFileTypeMaxSize
        {
            get { return GetInt("UploadFileTypeMaxSize", 5120); }
            set { SetExtendedAttribute("UploadFileTypeMaxSize", value.ToString()); }
        }

        /****************用户登录设置********************/

        public bool IsRecordIp
        {
            get { return GetBool("IsRecordIp", true); }
            set { SetExtendedAttribute("IsRecordIp", value.ToString()); }
        }

        public bool IsRecordSource
        {
            get { return GetBool("IsRecordSource", true); }
            set { SetExtendedAttribute("IsRecordSource", value.ToString()); }
        }

        public bool IsLoginFailToLock
        {
            get { return GetBool("IsLoginFailToLock", false); }
            set { SetExtendedAttribute("IsLoginFailToLock", value.ToString()); }
        }

        public int LoginFailToLockCount
        {
            get { return GetInt("LoginFailToLockCount", 3); }
            set { SetExtendedAttribute("LoginFailToLockCount", value.ToString()); }
        }

        public string LoginLockingType
        {
            get { return GetString("LoginLockingType", "Hours"); }
            set { SetExtendedAttribute("LoginLockingType", value); }
        }

        public int LoginLockingHours
        {
            get { return GetInt("LoginLockingHours", 3); }
            set { SetExtendedAttribute("LoginLockingHours", value.ToString()); }
        }
    }
}
