using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
	public class ContentCheckDao : DataProviderBase
	{
        public string TableName => "bairong_ContentCheck";

        private const string SqlSelect = "SELECT CheckID, TableName, PublishmentSystemID, NodeID, ContentID, IsAdmin, UserName, IsChecked, CheckedLevel, CheckDate, Reasons FROM bairong_ContentCheck WHERE CheckID = @CheckID";

        private const string SqlSelectAll = "SELECT CheckID, TableName, PublishmentSystemID, NodeID, ContentID, IsAdmin, UserName, IsChecked, CheckedLevel, CheckDate, Reasons FROM bairong_ContentCheck WHERE TableName = @TableName AND ContentID = @ContentID ORDER BY CheckID DESC";

        private const string SqlDelete = "DELETE FROM bairong_ContentCheck WHERE CheckID = @CheckID";

        private const string ParmCheckid = "@CheckID";
        private const string ParmTableName = "@TableName";
		private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmNodeid = "@NodeID";
        private const string ParmContentid = "@ContentID";
        private const string ParmIsAdmin = "@IsAdmin";
        private const string ParmUserName = "@UserName";
        private const string ParmIsChecked = "@IsChecked";
        private const string ParmCheckedLevel = "@CheckedLevel";
        private const string ParmCheckDate = "@CheckDate";
        private const string ParmReasons = "@Reasons";

		public void Insert(ContentCheckInfo checkInfo)
		{
            const string sqlString = "INSERT INTO bairong_ContentCheck (TableName, PublishmentSystemID, NodeID, ContentID, IsAdmin, UserName, IsChecked, CheckedLevel, CheckDate, Reasons) VALUES (@TableName, @PublishmentSystemID, @NodeID, @ContentID, @IsAdmin, @UserName, @IsChecked, @CheckedLevel, @CheckDate, @Reasons)";

			var parms = new IDataParameter[]
			{
                GetParameter(ParmTableName, EDataType.VarChar, 50, checkInfo.TableName),
				GetParameter(ParmPublishmentsystemid, EDataType.Integer, checkInfo.PublishmentSystemId),
                GetParameter(ParmNodeid, EDataType.Integer, checkInfo.NodeId),
                GetParameter(ParmContentid, EDataType.Integer, checkInfo.ContentId),
                GetParameter(ParmIsAdmin, EDataType.VarChar, 18, checkInfo.IsAdmin.ToString()),
                GetParameter(ParmUserName, EDataType.NVarChar, 255, checkInfo.UserName),
                GetParameter(ParmIsChecked, EDataType.VarChar, 18, checkInfo.IsChecked.ToString()),
                GetParameter(ParmCheckedLevel, EDataType.Integer, checkInfo.CheckedLevel),
                GetParameter(ParmCheckDate, EDataType.DateTime, checkInfo.CheckDate),
                GetParameter(ParmReasons, EDataType.NVarChar, 255, checkInfo.Reasons),
			};

            ExecuteNonQuery(sqlString, parms);
		}

		public void Delete(int checkId)
		{
            var parms = new IDataParameter[]
			{
				GetParameter(ParmCheckid, EDataType.Integer, checkId)
			};

            ExecuteNonQuery(SqlDelete, parms);
		}

		public ContentCheckInfo GetCheckInfo(int checkId)
		{
			ContentCheckInfo checkInfo = null;

            var parms = new IDataParameter[]
			{
				GetParameter(ParmCheckid, EDataType.Integer, checkId)
			};

            using (var rdr = ExecuteReader(SqlSelect, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    checkInfo = new ContentCheckInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

			return checkInfo;
		}

        public ContentCheckInfo GetCheckInfoByLastId(string tableName, int contentId)
        {
            ContentCheckInfo checkInfo = null;

            //var sqlString = "SELECT TOP 1 CheckID, TableName, PublishmentSystemID, NodeID, ContentID, IsAdmin, UserName, IsChecked, CheckedLevel, CheckDate, Reasons FROM bairong_ContentCheck WHERE TableName = @TableName AND ContentID = @ContentID ORDER BY CheckID DESC";
            var sqlString = SqlUtils.GetTopSqlString(TableName, "CheckID, TableName, PublishmentSystemID, NodeID, ContentID, IsAdmin, UserName, IsChecked, CheckedLevel, CheckDate, Reasons", "WHERE TableName = @TableName AND ContentID = @ContentID ORDER BY CheckID DESC", 1);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableName, EDataType.VarChar, 50, tableName),
                GetParameter(ParmContentid, EDataType.Integer, contentId)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    checkInfo = new ContentCheckInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));
                }
                rdr.Close();
            }

            return checkInfo;
        }

		public List<ContentCheckInfo> GetCheckInfoArrayList(string tableName, int contentId)
		{
			var arraylist = new List<ContentCheckInfo>();

            var parms = new IDataParameter[]
			{
				GetParameter(ParmTableName, EDataType.VarChar, 50, tableName),
                GetParameter(ParmContentid, EDataType.Integer, contentId)
			};

            using (var rdr = ExecuteReader(SqlSelectAll, parms)) 
			{
				while (rdr.Read())
				{
				    var i = 0;
                    arraylist.Add(new ContentCheckInfo(GetInt(rdr, i++), GetString(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetInt(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i)));
				}
				rdr.Close();
			}

			return arraylist;
		}
	}
}