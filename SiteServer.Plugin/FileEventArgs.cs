using System;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 为发生在文件中的更改事件提供数据。
    /// </summary>
    public class FileEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化 <see cref="T:SiteServer.Plugin.FileEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="changeType">发生在文件中的更改类型。</param>
        /// <param name="fullPath">变动的文件的完整路径。</param>
        /// <param name="fileName">变动的文件名称。</param>
        public FileEventArgs(ChangeType changeType, string fullPath, string fileName)
        {
            ChangeType = changeType;
            FullPath = fullPath;
            FileName = fileName;
        }

        /// <summary>
        /// 站点Id。
        /// </summary>
        public ChangeType ChangeType { get; }

        /// <summary>
        /// 获取变动的文件的完整路径。
        /// </summary>
        public string FullPath { get; }

        /// <summary>
        /// 获取变动的文件名称。
        /// </summary>
        public string FileName { get; }
    }
}