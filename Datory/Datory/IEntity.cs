using System;

namespace Datory
{
    public interface IEntity
    {
        int Id { get; set; }

        string Guid { get; set; }

        DateTime? LastModifiedDate { get; set; }
    }
}
