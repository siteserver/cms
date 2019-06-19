using System;

namespace SS.CMS.Data
{
    public interface IEntity
    {
        int Id { get; set; }

        string Guid { get; set; }

        DateTimeOffset? CreationDate { get; set; }

        DateTimeOffset? LastModifiedDate { get; set; }
    }
}
