namespace BaiRong.Core.Auth.JWT
{
    /// <summary>
    /// Provides JSON Serialize and Deserialize.  Allows custom serializers used.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serialize an object to JSON string
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>JSON string</returns>
        string Serialize(object obj);

        /// <summary>
        /// Deserialize a JSON string to typed object.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="json">JSON string</param>
        /// <returns>typed object</returns>
        T Deserialize<T>(string json);
    }
}