namespace Datory
{
    public interface IRepository<T> : IRepository where T : Entity, new()
    {

    }
}
