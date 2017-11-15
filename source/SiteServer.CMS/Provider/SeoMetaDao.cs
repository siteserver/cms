using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
	public class SeoMetaDao : DataProviderBase
	{
        public override string TableName => "siteserver_SeoMeta";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.SeoMetaId),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.PublishmentSystemId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.SeoMetaName),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.IsDefault),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.PageTitle),
                DataType = DataType.VarChar,
                Length = 80
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Keywords),
                DataType = DataType.VarChar,
                Length = 100
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Description),
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Copyright),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Author),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Email),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Language),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Charset),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Distribution),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Rating),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Robots),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.RevisitAfter),
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = nameof(SeoMetaInfo.Expires),
                DataType = DataType.VarChar,
                Length = 50
            }
        };

        private const string SqlSelectSeoMeta = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, IsDefault, PageTitle, Keywords, Description, Copyright, Author, Email, Language, Charset, Distribution, Rating, Robots, RevisitAfter, Expires FROM siteserver_SeoMeta WHERE SeoMetaID = @SeoMetaID";

        private const string SqlSelectAllSeoMeta = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, IsDefault, PageTitle, Keywords, Description, Copyright, Author, Email, Language, Charset, Distribution, Rating, Robots, RevisitAfter, Expires FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY SeoMetaID";

        private const string SqlSelectSeoMetaBySeoMetaName = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, IsDefault, PageTitle, Keywords, Description, Copyright, Author, Email, Language, Charset, Distribution, Rating, Robots, RevisitAfter, Expires FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID AND SeoMetaName = @SeoMetaName";

        private const string SqlSelectDefaultSeoMeta = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, IsDefault, PageTitle, Keywords, Description, Copyright, Author, Email, Language, Charset, Distribution, Rating, Robots, RevisitAfter, Expires FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID AND IsDefault = @IsDefault";

        private const string SqlSelectDefaultSeoMetaId = "SELECT SeoMetaID FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID AND IsDefault = @IsDefault";

        private const string SqlSelectAllSeoMetaByPublishmentSystemId = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, IsDefault, PageTitle, Keywords, Description, Copyright, Author, Email, Language, Charset, Distribution, Rating, Robots, RevisitAfter, Expires FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY SeoMetaID";

        private const string SqlSelectSeoMetaNames = "SELECT SeoMetaName FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectSeoMetaCount = "SELECT COUNT(SeoMetaID) FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlSelectSeoMetaName = "SELECT SeoMetaName FROM siteserver_SeoMeta WHERE SeoMetaID = @SeoMetaID";

        private const string SqlSelectSeoMetaIdBySeoMetaName = "SELECT SeoMetaID FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID AND SeoMetaName = @SeoMetaName";

        private const string SqlUpdateSeoMeta = "UPDATE siteserver_SeoMeta SET SeoMetaName = @SeoMetaName, IsDefault = @IsDefault, PageTitle = @PageTitle, Keywords = @Keywords, Description = @Description, Copyright = @Copyright, Author = @Author, Email = @Email, Language = @Language, Charset = @Charset, Distribution = @Distribution, Rating = @Rating, Robots = @Robots, RevisitAfter = @RevisitAfter, Expires = @Expires WHERE SeoMetaID = @SeoMetaID";

        private const string SqlUpdateAllIsDefault = "UPDATE siteserver_SeoMeta SET IsDefault = @IsDefault WHERE PublishmentSystemID = @PublishmentSystemID";

        private const string SqlUpdateIsDefault = "UPDATE siteserver_SeoMeta SET IsDefault = @IsDefault WHERE SeoMetaID = @SeoMetaID";

        private const string SqlDeleteSeoMeta = "DELETE FROM siteserver_SeoMeta WHERE SeoMetaID = @SeoMetaID";

		private const string ParmSeoMetaId = "@SeoMetaID";
		private const string ParmPublishmentSystemId = "@PublishmentSystemID";
		private const string ParmSeoMetaName = "@SeoMetaName";
		private const string ParmIsDefault = "@IsDefault";
		private const string ParmPageTitle = "@PageTitle";
		private const string ParmKeywords = "@Keywords";
		private const string ParmDescription = "@Description";
		private const string ParmCopyright = "@Copyright";
        private const string ParmAuthor = "@Author";
        private const string ParmEmail = "@Email";
		private const string ParmLanguage = "@Language";
		private const string ParmCharset = "@Charset";
		private const string ParmDistribution = "@Distribution";
		private const string ParmRating = "@Rating";
		private const string ParmRobots = "@Robots";
		private const string ParmRevisitAfter = "@RevisitAfter";
		private const string ParmExpires = "@Expires";

		public void Insert(SeoMetaInfo info)
		{
			if (info.IsDefault)
			{
				SetAllIsDefaultToFalse(info.PublishmentSystemId);
			}

            var sqlString = "INSERT INTO siteserver_SeoMeta (PublishmentSystemID, SeoMetaName, [IsDefault], [PageTitle], [Keywords], [Description], [Copyright], [Author], [Email], [Language], [Charset], [Distribution], [Rating], [Robots], [RevisitAfter], [Expires]) VALUES (@PublishmentSystemID, @SeoMetaName, @IsDefault, @PageTitle, @Keywords, @Description, @Copyright, @Author, @Email, @Language, @Charset, @Distribution, @Rating, @Robots, @RevisitAfter, @Expires)";

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, info.PublishmentSystemId),
				GetParameter(ParmSeoMetaName, DataType.VarChar, 50, info.SeoMetaName),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmPageTitle, DataType.VarChar, 80, info.PageTitle),
				GetParameter(ParmKeywords, DataType.VarChar, 100, info.Keywords),
				GetParameter(ParmDescription, DataType.VarChar, 200, info.Description),
				GetParameter(ParmCopyright, DataType.VarChar, 255, info.Copyright),
				GetParameter(ParmAuthor, DataType.VarChar, 50, info.Author),
				GetParameter(ParmEmail, DataType.VarChar, 50, info.Email),
				GetParameter(ParmLanguage, DataType.VarChar, 50, info.Language),
				GetParameter(ParmCharset, DataType.VarChar, 50, info.Charset),
				GetParameter(ParmDistribution, DataType.VarChar, 50, info.Distribution),
				GetParameter(ParmRating, DataType.VarChar, 50, info.Rating),
				GetParameter(ParmRobots, DataType.VarChar, 50, info.Robots),
				GetParameter(ParmRevisitAfter, DataType.VarChar, 50, info.RevisitAfter),
				GetParameter(ParmExpires, DataType.VarChar, 50, info.Expires)
			};

            ExecuteNonQuery(sqlString, insertParms);
		}

		public void Update(SeoMetaInfo info)
		{
			if (info.IsDefault)
			{
				SetAllIsDefaultToFalse(info.PublishmentSystemId);
			}

			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmSeoMetaName, DataType.VarChar, 50, info.SeoMetaName),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmPageTitle, DataType.VarChar, 80, info.PageTitle),
				GetParameter(ParmKeywords, DataType.VarChar, 100, info.Keywords),
				GetParameter(ParmDescription, DataType.VarChar, 200, info.Description),
				GetParameter(ParmCopyright, DataType.VarChar, 255, info.Copyright),
				GetParameter(ParmAuthor, DataType.VarChar, 50, info.Author),
				GetParameter(ParmEmail, DataType.VarChar, 50, info.Email),
				GetParameter(ParmLanguage, DataType.VarChar, 50, info.Language),
				GetParameter(ParmCharset, DataType.VarChar, 50, info.Charset),
				GetParameter(ParmDistribution, DataType.VarChar, 50, info.Distribution),
				GetParameter(ParmRating, DataType.VarChar, 50, info.Rating),
				GetParameter(ParmRobots, DataType.VarChar, 50, info.Robots),
				GetParameter(ParmRevisitAfter, DataType.VarChar, 50, info.RevisitAfter),
				GetParameter(ParmExpires, DataType.VarChar, 50, info.Expires),
				GetParameter(ParmSeoMetaId, DataType.Integer, info.SeoMetaId)
			};
							
			ExecuteNonQuery(SqlUpdateSeoMeta, updateParms);
		}

		private void SetAllIsDefaultToFalse(int publishmentSystemId)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, DataType.VarChar, 18, false.ToString()),
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			ExecuteNonQuery(SqlUpdateAllIsDefault, updateParms);
		}

        public void SetDefault(int publishmentSystemId, int seoMetaId, bool defaultValue)
		{
			SetAllIsDefaultToFalse(publishmentSystemId);

			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, DataType.VarChar, 18, defaultValue.ToString()),
				GetParameter(ParmSeoMetaId, DataType.Integer, seoMetaId)
			};
							
			ExecuteNonQuery(SqlUpdateIsDefault, updateParms);
		}

		public void Delete(int seoMetaId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmSeoMetaId, DataType.Integer, seoMetaId)
			};
							
			ExecuteNonQuery(SqlDeleteSeoMeta, parms);
		}

		public SeoMetaInfo GetSeoMetaInfo(int seoMetaId)
		{
			SeoMetaInfo info = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmSeoMetaId, DataType.Integer, seoMetaId)
			};
			
			using (var rdr = ExecuteReader(SqlSelectSeoMeta, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new SeoMetaInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
				}
				rdr.Close();
			}

			return info;
		}

		public SeoMetaInfo GetSeoMetaInfoBySeoMetaName(int publishmentSystemId, string seoMetaName)
		{
			SeoMetaInfo info = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmSeoMetaName, DataType.VarChar, 50, seoMetaName)
			};
			
			using (var rdr = ExecuteReader(SqlSelectSeoMetaBySeoMetaName, parms))
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new SeoMetaInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
				rdr.Close();
			}

			return info;
		}

		public string GetImportSeoMetaName(int publishmentSystemId, string seoMetaName)
		{
			string importSeoMetaName;
			if (seoMetaName.IndexOf("_", StringComparison.Ordinal) != -1)
			{
				var seoMetaNameCount = 0;
				var lastSeoMetaName = seoMetaName.Substring(seoMetaName.LastIndexOf("_", StringComparison.Ordinal) + 1);
				var firstSeoMetaName = seoMetaName.Substring(0, seoMetaName.Length - lastSeoMetaName.Length);
				try
				{				
					seoMetaNameCount = int.Parse(lastSeoMetaName);
				}
			    catch
			    {
			        // ignored
			    }
			    seoMetaNameCount++;
				importSeoMetaName = firstSeoMetaName + seoMetaNameCount;
			}
			else
			{
				importSeoMetaName = seoMetaName + "_1";
			}

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmSeoMetaName, DataType.VarChar, 50, importSeoMetaName)
			};

			using (var rdr = ExecuteReader(SqlSelectSeoMetaBySeoMetaName, parms))
			{
				if (rdr.Read())
				{
					importSeoMetaName = GetImportSeoMetaName(publishmentSystemId, importSeoMetaName);
				}
				rdr.Close();
			}

			return importSeoMetaName;
		}

		public int GetCount(int publishmentSystemId)
		{
			var count = 0;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectSeoMetaCount, parms)) 
			{
				if (rdr.Read()) 
				{
                    count = GetInt(rdr, 0);
                }
				rdr.Close();
			}

			return count;
		}

		public SeoMetaInfo GetDefaultSeoMetaInfo(int publishmentSystemId)
		{
			SeoMetaInfo info = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, true.ToString())
			};
			
			using (var rdr = ExecuteReader(SqlSelectDefaultSeoMeta, parms)) 
			{
				if (rdr.Read())
				{
				    var i = 0;
                    info = new SeoMetaInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                }
				rdr.Close();
			}

			return info;
		}

        public int GetDefaultSeoMetaId(int publishmentSystemId)
        {
            var seoMetaId = 0;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmIsDefault, DataType.VarChar, 18, true.ToString())
			};

            using (var rdr = ExecuteReader(SqlSelectDefaultSeoMetaId, parms))
            {
                if (rdr.Read())
                {
                    seoMetaId = GetInt(rdr, 0);
                }
				rdr.Close();
            }

            return seoMetaId;
        }

		public IEnumerable GetDataSource(int publishmentSystemId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllSeoMeta, parms);
			return enumerable;
		}

		public ArrayList GetSeoMetaInfoArrayListByPublishmentSystemId(int publishmentSystemId)
		{
			var arraylist = new ArrayList();

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectAllSeoMetaByPublishmentSystemId, parms))
			{
				while (rdr.Read())
				{
				    var i = 0;
                    var info = new SeoMetaInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    arraylist.Add(info);
				}
				rdr.Close();
			}
			return arraylist;
		}

		public string GetSeoMetaName(int seoMetaId)
		{
			var seoMetaName = string.Empty;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmSeoMetaId, DataType.Integer, seoMetaId)
			};

			using (var rdr = ExecuteReader(SqlSelectSeoMetaName, parms)) 
			{
				if (rdr.Read()) 
				{					
					seoMetaName = GetString(rdr, 0);
				}
				rdr.Close();
			}

			return seoMetaName;
		}

		public int GetSeoMetaIdBySeoMetaName(int publishmentSystemId, string seoMetaName)
		{
			var seoMetaId = 0;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId),
				GetParameter(ParmSeoMetaName, DataType.VarChar, 50, seoMetaName)
			};

			using (var rdr = ExecuteReader(SqlSelectSeoMetaIdBySeoMetaName, parms)) 
			{
				if (rdr.Read())
				{
                    seoMetaId = GetInt(rdr, 0);
                }
				rdr.Close();
			}

			return seoMetaId;
		}

		public ArrayList GetSeoMetaNameArrayList(int publishmentSystemId)
		{
			var list = new ArrayList();

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, DataType.Integer, publishmentSystemId)
			};

			using (var rdr = ExecuteReader(SqlSelectSeoMetaNames, parms)) 
			{
				while (rdr.Read()) 
				{
                    list.Add(GetString(rdr, 0));
				}
				rdr.Close();
			}

			return list;
		}
	}
}
