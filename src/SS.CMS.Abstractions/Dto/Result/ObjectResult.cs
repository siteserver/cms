namespace SS.CMS.Abstractions.Dto.Result
{
    public class ObjectResult<T> where T : class
    {
        public T Value { get; set; }
    }
}