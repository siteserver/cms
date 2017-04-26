using System;

namespace Top.Api.Parser
{
    /// <summary>
    /// TOP API响应解释器接口。响应格式可以是XML, JSON等等。
    /// </summary>
    public interface ITopParser<T>
    {
        /// <summary>
        /// 把响应字符串解释成相应的领域对象。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="body">响应字符串</param>
        /// <returns>领域对象</returns>
        T Parse(string body);

        /// <summary>
        /// 把响应字符串解释成相应的领域对象。
        /// </summary>
        /// <param name="body">响应字符串</param>
        /// <param name="type">领域对象</param>
        /// <returns>领域对象</returns>
        T Parse(string body, Type type);
    }
}
