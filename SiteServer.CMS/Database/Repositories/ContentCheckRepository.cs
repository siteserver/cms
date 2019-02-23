using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;

namespace SiteServer.CMS.Database.Repositories
{
    public class ContentCheckRepository : GenericRepository<ContentCheckInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(ContentCheckInfo.Id);
            public const string TableName = nameof(ContentCheckInfo.TableName);
            public const string ChannelId = nameof(ContentCheckInfo.ChannelId);
        }

        public void Insert(ContentCheckInfo checkInfo)
        {
            //const string sqlString = "INSERT INTO siteserver_ContentCheck (TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons) VALUES (@TableName, @SiteId, @ChannelId, @ContentId, @UserName, @IsChecked, @CheckedLevel, @CheckDate, @Reasons)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTableName, checkInfo.TableName),
            //    GetParameter(ParamSiteId, checkInfo.SiteId),
            //    GetParameter(ParamChannelId, checkInfo.ChannelId),
            //    GetParameter(ParamContentId, checkInfo.ContentId),
            //    GetParameter(ParamUserName, checkInfo.UserName),
            //    GetParameter(ParamIsChecked, checkInfo.IsChecked.ToString()),
            //    GetParameter(ParamCheckedLevel, checkInfo.CheckedLevel),
            //    GetParameter(ParamCheckDate,checkInfo.CheckDate),
            //    GetParameter(ParamReasons, checkInfo.Reasons),
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
            InsertObject(checkInfo);
        }

        //public void Delete(int checkId)
        //{
        //    //IDataParameter[] parameters =
        //    //{
        //    //    GetParameter(ParamId, checkId)
        //    //};
        //    //"DELETE FROM siteserver_ContentCheck WHERE Id = @Id"
        //    //DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);
        //    DeleteById(checkId);
        //}

        public IList<ContentCheckInfo> GetCheckInfoList(string tableName, int contentId)
        {
            //var list = new List<ContentCheckInfo>();

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTableName, tableName),
            //    GetParameter(ParamContentId, contentId)
            //};
            //"SELECT Id, TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons FROM siteserver_ContentCheck WHERE TableName = @TableName AND ContentId = @ContentId ORDER BY Id DESC"
            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAll, parameters))
            //{
            //    while (rdr.Read())
            //    {
            //        var i = 0;
            //        list.Add(new ContentCheckInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetString(rdr, i)));
            //    }
            //    rdr.Close();
            //}

            //return list;

            return GetObjectList(Q
                .Where(Attr.TableName, tableName)
                .Where(Attr.ChannelId, contentId)
                .OrderByDesc(Attr.Id));
        }
    }
}

//using System.Collections.Generic;
 //using System.Data;
 //using SiteServer.CMS.Database.Core;
 //using SiteServer.CMS.Database.Models;
 //using SiteServer.Plugin;
 //using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//	public class ContentCheck : DataProviderBase
//	{
//        public override string TableName => "siteserver_ContentCheck";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.TableName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.ChannelId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.ContentId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.UserName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.IsChecked),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.CheckedLevel),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.CheckDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ContentCheckInfo.Reasons),
//                DataType = DataType.VarChar
//            }
//        };

//        private const string SqlSelectAll = "SELECT Id, TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons FROM siteserver_ContentCheck WHERE TableName = @TableName AND ContentId = @ContentId ORDER BY Id DESC";

//        private const string SqlDelete = "DELETE FROM siteserver_ContentCheck WHERE Id = @Id";

//        private const string ParamId = "@Id";
//        private const string ParamTableName = "@TableName";
//		private const string ParamSiteId = "@SiteId";
//        private const string ParamChannelId = "@ChannelId";
//        private const string ParamContentId = "@ContentId";
//        private const string ParamUserName = "@UserName";
//        private const string ParamIsChecked = "@IsChecked";
//        private const string ParamCheckedLevel = "@CheckedLevel";
//        private const string ParamCheckDate = "@CheckDate";
//        private const string ParamReasons = "@Reasons";

//		public void InsertObject(ContentCheckInfo checkInfo)
//		{
//            const string sqlString = "INSERT INTO siteserver_ContentCheck (TableName, SiteId, ChannelId, ContentId, UserName, IsChecked, CheckedLevel, CheckDate, Reasons) VALUES (@TableName, @SiteId, @ChannelId, @ContentId, @UserName, @IsChecked, @CheckedLevel, @CheckDate, @Reasons)";

//			IDataParameter[] parameters =
//			{
//                GetParameter(ParamTableName, checkInfo.TableName),
//				GetParameter(ParamSiteId, checkInfo.SiteId),
//                GetParameter(ParamChannelId, checkInfo.ChannelId),
//                GetParameter(ParamContentId, checkInfo.ContentId),
//                GetParameter(ParamUserName, checkInfo.UserName),
//                GetParameter(ParamIsChecked, checkInfo.IsChecked.ToString()),
//                GetParameter(ParamCheckedLevel, checkInfo.CheckedLevel),
//                GetParameter(ParamCheckDate,checkInfo.CheckDate),
//                GetParameter(ParamReasons, checkInfo.Reasons),
//			};

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//		}

//		public void DeleteById(int checkId)
//		{
//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamId, checkId)
//			};

//		    DatabaseApi.ExecuteNonQuery(ConnectionString, SqlDelete, parameters);
//		}

//		public List<ContentCheckInfo> GetCheckInfoList(string tableName, int contentId)
//		{
//			var list = new List<ContentCheckInfo>();

//            IDataParameter[] parameters =
//			{
//				GetParameter(ParamTableName, tableName),
//                GetParameter(ParamContentId, contentId)
//			};

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, SqlSelectAll, parameters)) 
//			{
//				while (rdr.Read())
//				{
//				    var i = 0;
//                    list.Add(new ContentCheckInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), TranslateUtils.ToBool(DatabaseApi.GetString(rdr, i++)), DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetDateTime(rdr, i++), DatabaseApi.GetString(rdr, i)));
//				}
//				rdr.Close();
//			}

//			return list;
//		}
//	}
//}