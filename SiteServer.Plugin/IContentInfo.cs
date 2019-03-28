using System;

namespace SiteServer.Plugin
{
    /// <inheritdoc />
    /// <summary>
    /// 内容实体接口。
    /// 内容表默认名称为model_Content，SiteServer CMS 后台可以新建新的内容表，或者修改现有内容表的字段。
    /// </summary>
    public interface IContentInfo
    {
        /// <summary>
        /// 内容Id。
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 栏目Id。
        /// </summary>
        int ChannelId { get; set; }

        /// <summary>
        /// 站点Id。
        /// </summary>
        int SiteId { get; set; }

        /// <summary>
        /// 添加人。
        /// </summary>
        string AddUserName { get; set; }

        /// <summary>
        /// 修改人。
        /// </summary>
        string LastEditUserName { get; set; }

        /// <summary>
        /// 修改时间。
        /// </summary>
        DateTime? LastEditDate { get; set; }

        /// <summary>
        /// 新增内容的管理员Id。
        /// </summary>
        int AdminId { get; set; }

        /// <summary>
        /// 新增投稿的用户Id。
        /// </summary>
        int UserId { get; set; }

        /// <summary>
        /// 排序。
        /// </summary>
        int Taxis { get; set; }

        /// <summary>
        /// 来源内容Id。
        /// </summary>
        int SourceId { get; set; }

        /// <summary>
        /// 引用内容Id。
        /// </summary>
        int ReferenceId { get; set; }

        /// <summary>
        /// 总点击量。
        /// </summary>
        int Hits { get; set; }

        /// <summary>
        /// 日点击。
        /// </summary>
        int HitsByDay { get; set; }

        /// <summary>
        /// 周点击。
        /// </summary>
        int HitsByWeek { get; set; }

        /// <summary>
        /// 月点击。
        /// </summary>
        int HitsByMonth { get; set; }

        /// <summary>
        /// 最后点击时间。
        /// </summary>
        DateTime? LastHitsDate { get; set; }

        /// <summary>
        /// 内容标题。
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// 是否置顶内容。
        /// </summary>
        bool Top { get; set; }

        /// <summary>
        /// 是否推荐内容。
        /// </summary>
        bool Recommend { get; set; }

        /// <summary>
        /// 是否热点内容。
        /// </summary>
        bool Hot { get; set; }

        /// <summary>
        /// 是否醒目内容。
        /// </summary>
        bool Color { get; set; }

        /// <summary>
        /// 内容组。
        /// 多个内容组以英文逗号隔开。
        /// </summary>
        string GroupNameCollection { get; set; }

        /// <summary>
        /// 标签。
        /// 多个标签以英文逗号隔开。
        /// </summary>
        string Tags { get; set; }

        /// <summary>
        /// 是否终审通过通过。
        /// </summary>
        bool Checked { get; set; }

        /// <summary>
        /// 未终审通过情况下，处于第几级审核状态。
        /// </summary>
        int CheckedLevel { get; set; }

        /// <summary>
        /// 外部链接，设置后链接将指向此地址。
        /// </summary>
        string LinkUrl { get; set; }

        /// <summary>
        /// 添加时间。
        /// </summary>
        DateTime? AddDate { get; set; }
    }
}