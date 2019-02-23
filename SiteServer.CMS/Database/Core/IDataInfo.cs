using System;

namespace SiteServer.CMS.Database.Core
{
    public interface IDataInfo
    {
        int Id { get; set; }

        string Guid { get; set; }

        DateTime? LastModifiedDate { get; set; }
    }
}
