using BaiRong.Core.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Model
{
	public class TaskBackupInfo : ExtendedAttributes
	{
        public TaskBackupInfo(string settings) : base(settings)
        {

        }

        public EBackupType BackupType
		{
            get { return EBackupTypeUtils.GetEnumType(GetString("BackupType")); }
            set { Set("BackupType", EBackupTypeUtils.GetValue(value)); }
		}

        public string PublishmentSystemIdCollection
        {
            get { return GetString("PublishmentSystemIdCollection"); }
            set { Set("PublishmentSystemIdCollection", value); }
        }

        public bool IsBackupAll
        {
            get { return GetBool("IsBackupAll"); }
            set { Set("IsBackupAll", value.ToString()); }
        }
	}
}
