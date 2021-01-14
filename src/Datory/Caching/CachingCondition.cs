using SqlKata;

namespace Datory.Caching
{
    internal class CachingCondition : AbstractClause
    {
        public CachingAction Action { get; set; }

        public string CacheKey { get; set; }

        public string[] CacheKeysToRemove { get; set; }


        /// <inheritdoc />
        public override AbstractClause Clone()
        {
            return new CachingCondition
            {
                Engine = Engine,
                Action = Action,
                CacheKey = CacheKey,
                CacheKeysToRemove = CacheKeysToRemove,
                Component = Component,
            };
        }
    }
}
