using System;

namespace SiteServer.Plugin
{
    /// <summary>
    /// 为发生在数据库中的更改事件提供数据。
    /// </summary>
    public class DataEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化 <see cref="T:SiteServer.Plugin.DataEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="changeType">发生在数据库中的更改类型。</param>
        /// <param name="tableName">变动的数据表名称。</param>
        /// <param name="id">变动的数据Id。</param>
        public DataEventArgs(ChangeType changeType, string tableName, int id)
        {
            ChangeType = changeType;
            TableName = tableName;
            Id = id;
        }

        /// <summary>
        /// 站点Id。
        /// </summary>
        public ChangeType ChangeType { get; }

        /// <summary>
        /// 获取变动的数据表名称。
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// 获取变动的数据Id。
        /// </summary>
        public int Id { get; }
    }
}