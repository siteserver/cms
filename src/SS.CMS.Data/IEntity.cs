using System;

namespace SS.CMS.Data
{
    public interface IEntity
    {
        int Id { get; set; }

        string Guid { get; set; }

        DateTime? CreatedDate { get; set; }

        DateTime? LastModifiedDate { get; set; }
    }
}
