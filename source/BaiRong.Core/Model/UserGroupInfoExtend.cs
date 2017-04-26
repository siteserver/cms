using System.Collections.Specialized;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Model
{
    public class UserGroupInfoExtend : ExtendedAttributes
    {
        public UserGroupInfoExtend(string extendValues)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(extendValues);
            SetExtendedAttribute(nameValueCollection);
        }

        public UserGroupInfoExtend(bool isAllowVisit, bool isAllowHide, bool isAllowSignature, ETriState searchType, int searchInterval, bool isAllowRead, bool isAllowPost, bool isAllowReply, bool isAllowPoll, int maxPostPerDay, int postInterval, ETriState uploadType, ETriState downloadType, bool isAllowSetAttachmentPermissions, int maxSize, int maxSizePerDay, int maxNumPerDay, string attachmentExtensions)
        {
            var nameValueCollection = new NameValueCollection();
            SetExtendedAttribute(nameValueCollection);

            IsAllowVisit = isAllowVisit;
            IsAllowHide = isAllowHide;
            IsAllowSignature = isAllowSignature;
            SearchType = ETriStateUtils.GetValue(searchType);
            SearchInterval = searchInterval;
            IsAllowRead = isAllowRead;
            IsAllowPost = isAllowPost;
            IsAllowReply = isAllowReply;
            IsAllowPoll = isAllowPoll;
            MaxPostPerDay = maxPostPerDay;
            PostInterval = postInterval;
            UploadType = ETriStateUtils.GetValue(uploadType);
            DownloadType = ETriStateUtils.GetValue(downloadType);
            IsAllowSetAttachmentPermissions = isAllowSetAttachmentPermissions;
            MaxSize = maxSize;
            MaxSizePerDay = maxSizePerDay;
            MaxNumPerDay = maxNumPerDay;
            AttachmentExtensions = attachmentExtensions;
        }

        //投稿权限

        public bool IsWritingEnabled
        {
            get { return GetBool("IsWritingEnabled", false); }
            set { SetExtendedAttribute("IsWritingEnabled", value.ToString()); }
        }

        public string WritingAdminUserName
        {
            get { return GetString("WritingAdminUserName", string.Empty); }
            set { SetExtendedAttribute("WritingAdminUserName", value); }
        }

        //基本权限

        public bool IsAllowVisit
        {
            get { return GetBool("IsAllowVisit", true); }
            set { SetExtendedAttribute("IsAllowVisit", value.ToString()); }
        }

        public bool IsAllowHide
        {
            get { return GetBool("IsAllowHide", true); }
            set { SetExtendedAttribute("IsAllowHide", value.ToString()); }
        }

        public bool IsAllowSignature
        {
            get { return GetBool("IsAllowSignature", true); }
            set { SetExtendedAttribute("IsAllowSignature", value.ToString()); }
        }

        public string SearchType
        {
            get
            {
                return GetString("SearchType", ETriStateUtils.GetValue(ETriState.True));
            }
            set
            {
                SetExtendedAttribute("SearchType", value);
            }
        }

        public int SearchInterval
        {
            get { return GetInt("SearchInterval", 10); }
            set { SetExtendedAttribute("SearchInterval", value.ToString()); }
        }

        //帖子权限

        public bool IsAllowRead
        {
            get { return GetBool("IsAllowRead", true); }
            set { SetExtendedAttribute("IsAllowRead", value.ToString()); }
        }

        public bool IsAllowPost
        {
            get { return GetBool("IsAllowPost", true); }
            set { SetExtendedAttribute("IsAllowPost", value.ToString()); }
        }

        public bool IsAllowReply
        {
            get { return GetBool("IsAllowReply", true); }
            set { SetExtendedAttribute("IsAllowReply", value.ToString()); }
        }

        public bool IsAllowPoll
        {
            get { return GetBool("IsAllowPoll", true); }
            set { SetExtendedAttribute("IsAllowPoll", value.ToString()); }
        }

        public int MaxPostPerDay
        {
            get { return GetInt("MaxPostPerDay", 0); }
            set { SetExtendedAttribute("MaxPostPerDay", value.ToString()); }
        }

        public int PostInterval
        {
            get { return GetInt("PostInterval", 0); }
            set { SetExtendedAttribute("PostInterval", value.ToString()); }
        }

        //附件权限

        public string UploadType
        {
            get
            {
                return GetString("UploadType", ETriStateUtils.GetValue(ETriState.True));
            }
            set
            {
                SetExtendedAttribute("UploadType", value);
            }
        }

        public string DownloadType
        {
            get
            {
                return GetString("DownloadType", ETriStateUtils.GetValue(ETriState.True));
            }
            set
            {
                SetExtendedAttribute("DownloadType", value);
            }
        }

        // 允许设置附件权限
        public bool IsAllowSetAttachmentPermissions
        {
            get {
                return GetBool("IsAllowSetAttachmentPermissions", true);
            }
            set {
                SetExtendedAttribute("IsAllowSetAttachmentPermissions", value.ToString());
            }
        }

        // 最大附件大小
        public int MaxSize
        {
            get
            {
                return GetInt("MaxSize", 0);
            }
            set
            {
                SetExtendedAttribute("MaxSize", value.ToString());
            }
        }

        // 每天总附件大小
        public int MaxSizePerDay
        {
            get
            {
                return GetInt("MaxSizePerDay", 0);
            }
            set
            {
                SetExtendedAttribute("MaxSizePerDay", value.ToString());
            }
        }

        // 每天最大附件数量
        public int MaxNumPerDay
        {
            get
            {
                return GetInt("MaxNumPerDay", 0);
            }
            set
            {
                SetExtendedAttribute("MaxNumPerDay", value.ToString());
            }
        }

        // 允许附件类型
        public string AttachmentExtensions
        {
            get
            {
                return GetString("AttachmentExtensions", "chm,pdf,zip,rar,tar,gz,7z,gif,jpg,jpeg,png,doc,docx,xls,xlsx,ppt,pptx");
            }
            set
            {
                SetExtendedAttribute("AttachmentExtensions", value);
            }
        }
    }
}
