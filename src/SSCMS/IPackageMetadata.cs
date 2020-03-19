using System;
using System.Collections.Generic;

namespace SSCMS
{
    /// <summary>
    /// 插件元数据接口。
    /// </summary>
    public interface IPackageMetadata
    {
        /// <summary>
        /// 插件Id，作为插件的唯一标识。
        /// </summary>
        string PluginId { get; }

        /// <summary>
        /// 插件版本号。
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 插件图标访问地址。
        /// </summary>
        string IconUrl { get; }

        /// <summary>
        /// 插件官网地址。
        /// </summary>
        string ProjectUrl { get; }

        /// <summary>
        /// 插件授权许可说明地址。
        /// </summary>
        string LicenseUrl { get; }

        /// <summary>
        /// 插件版权。
        /// </summary>
        string Copyright { get; }

        /// <summary>
        /// 插件说明。
        /// </summary>
        string Description { get; }

        /// <summary>
        /// 插件版本发行说明。
        /// </summary>
        string ReleaseNotes { get; }

        /// <summary>
        /// 插件名称。
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 插件标签。
        /// </summary>
        string Tags { get; }

        /// <summary>
        /// 插件作者列表。
        /// </summary>
        string Authors { get; }

        /// <summary>
        /// 插件拥有者。
        /// </summary>
        string Owners { get; }

        /// <summary>
        /// 插件使用的语言。
        /// </summary>
        string Language { get; }
    }
}
