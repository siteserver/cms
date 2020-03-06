namespace SS.CMS.Abstractions
{
    /// <summary>
    /// 插件初始化类，此类的方法由系统调用，在插件开发时请勿直接使用。
    /// </summary>
    public class Initializer
    {
        /// <summary>
        /// 初始化插件类。
        /// </summary>
        /// <remarks>
        /// 此方法将由 SS CMS 系统载入插件时调用。
        /// </remarks>
        /// <param name="metadata">插件元数据接口。</param>
        public virtual void Initialize(IPackageMetadata metadata)
        {

        }
    }
}