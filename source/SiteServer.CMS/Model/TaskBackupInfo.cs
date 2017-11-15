using BaiRong.Core;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
	public class TaskBackupInfo : ExtendedAttributes
	{
        public TaskBackupInfo(string serviceParameters)
        {
            var nameValueCollection = TranslateUtils.ToNameValueCollection(serviceParameters);
            SetExtendedAttribute(nameValueCollection);
        }

        public EBackupType BackupType
		{
            get { return EBackupTypeUtils.GetEnumType(GetExtendedAttribute("BackupType")); }
            set { SetExtendedAttribute("BackupType", EBackupTypeUtils.GetValue(value)); }
		}

        public string PublishmentSystemIdCollection
        {
            get { return GetExtendedAttribute("PublishmentSystemIdCollection"); }
            set { SetExtendedAttribute("PublishmentSystemIdCollection", value); }
        }

        public bool IsBackupAll
        {
            get { return GetBool("IsBackupAll"); }
            set { SetExtendedAttribute("IsBackupAll", value.ToString()); }
        }
	}
}
