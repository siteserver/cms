using System;

namespace Datory
{
    public interface IEntity : ICloneable
    {
        int Id { get; set; }

        string Guid { get; set; }

        DateTime? CreatedDate { get; set; }

        DateTime? LastModifiedDate { get; set; }
    }
}
