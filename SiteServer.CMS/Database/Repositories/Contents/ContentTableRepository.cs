using System.Collections.Generic;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;

namespace SiteServer.CMS.Database.Repositories.Contents
{
    public partial class ContentTableRepository : GenericRepositoryAbstract
    {
        public int SiteId { get; }
        public override string TableName { get; }
        public override List<TableColumn> TableColumns { get; }

        public static List<ContentTableRepository> GetContentRepositoryList(SiteInfo siteInfo)
        {
            var list = new List<ContentTableRepository>();

            foreach (var tableName in SiteManager.GetTableNameList(siteInfo))
            {
                list.Add(GetContentRepository(siteInfo.Id, tableName));
            }

            return list;
        }

        public static ContentTableRepository GetContentRepository(int siteId, string tableName)
        {
            return new ContentTableRepository(siteId, tableName);
        }

        private ContentTableRepository(int siteId, string tableName)
        {
            SiteId = siteId;
            TableName = tableName;
            TableColumns = ReflectionUtils.GetTableColumns(typeof(ContentInfo));

            //TableColumns = new List<TableColumn>
            //{
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.Id),
            //        DataType = DataType.Integer,
            //        IsIdentity = true,
            //        IsPrimaryKey = true
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.ChannelId),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.SiteId),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.AddUserName),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.LastEditUserName),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.LastEditDate),
            //        DataType = DataType.DateTime
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.AdminId),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.UserId),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.Taxis),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.GroupNameCollection),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.Tags),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.SourceId),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.ReferenceId),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.IsChecked),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.CheckedLevel),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.Hits),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.HitsByDay),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.HitsByWeek),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.HitsByMonth),
            //        DataType = DataType.Integer
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.LastHitsDate),
            //        DataType = DataType.DateTime
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.SettingsXml),
            //        DataType = DataType.Text
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.Title),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.IsTop),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.IsRecommend),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.IsHot),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.IsColor),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.LinkUrl),
            //        DataType = DataType.VarChar
            //    },
            //    new TableColumn
            //    {
            //        AttributeName = ContentAttribute.AddDate),
            //        DataType = DataType.DateTime
            //    }
            //};
        }
    }
}
