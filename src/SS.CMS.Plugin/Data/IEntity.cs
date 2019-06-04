using System;

namespace SS.CMS.Plugin.Data
{
    public interface IEntity
    {
        int Id { get; set; }

        string Guid { get; set; }

        DateTime? LastModifiedDate { get; set; }
    }
}
