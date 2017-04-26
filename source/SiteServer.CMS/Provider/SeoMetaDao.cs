using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
	public class SeoMetaDao : DataProviderBase
	{
        private const string SqlSelectSeoMeta = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, [IsDefault], [PageTitle], [Keywords], [Description], [Copyright], [Author], [Email], [Language], [Charset], [Distribution], [Rating], [Robots], [RevisitAfter], [Expires] FROM siteserver_SeoMeta WHERE SeoMetaID = @SeoMetaID";

		private const string SqlSelectAllSeoMeta = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, [IsDefault], [PageTitle], [Keywords], [Description], [Copyright], [Author], [Email], [Language], [Charset], [Distribution], [Rating], [Robots], [RevisitAfter], [Expires] FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY SeoMetaID";

        private const string SqlSelectSeoMetaBySeoMetaName = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, [IsDefault], [PageTitle], [Keywords], [Description], [Copyright], [Author], [Email], [Language], [Charset], [Distribution], [Rating], [Robots], [RevisitAfter], [Expires] FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID AND SeoMetaName = @SeoMetaName";

        private const string SqlSelectDefaultSeoMeta = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, [IsDefault], [PageTitle], [Keywords], [Description], [Copyright], [Author], [Email], [Language], [Charset], [Distribution], [Rating], [Robots], [RevisitAfter], [Expires] FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID AND [IsDefault] = @IsDefault";

        private const string SqlSelectDefaultSeoMetaId = "SELECT SeoMetaID FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID AND [IsDefault] = @IsDefault";

        private const string SqlSelectAllSeoMetaByPublishmentSystemId = "SELECT SeoMetaID, PublishmentSystemID, SeoMetaName, [IsDefault], [PageTitle], [Keywords], [Description], [Copyright], [Author], [Email], [Language], [Charset], [Distribution], [Rating], [Robots], [RevisitAfter], [Expires] FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID ORDER BY SeoMetaID";

		private const string SqlSelectSeoMetaNames = "SELECT SeoMetaName FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID";

		private const string SqlSelectSeoMetaCount = "SELECT COUNT(SeoMetaID) FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID";

		private const string SqlSelectSeoMetaName = "SELECT SeoMetaName FROM siteserver_SeoMeta WHERE SeoMetaID = @SeoMetaID";

		private const string SqlSelectSeoMetaIdBySeoMetaName = "SELECT SeoMetaID FROM siteserver_SeoMeta WHERE PublishmentSystemID = @PublishmentSystemID AND SeoMetaName = @SeoMetaName";

        private const string SqlUpdateSeoMeta = "UPDATE siteserver_SeoMeta SET SeoMetaName = @SeoMetaName, [IsDefault] = @IsDefault, [PageTitle] = @PageTitle, [Keywords] = @Keywords, [Description] = @Description, [Copyright] = @Copyright, [Author] = @Author, [Email] = @Email, [Language] = @Language, [Charset] = @Charset, [Distribution] = @Distribution, [Rating] = @Rating, [Robots] = @Robots, [RevisitAfter] = @RevisitAfter, [Expires] = @Expires WHERE SeoMetaID = @SeoMetaID";

		private const string SqlUpdateAllIsDefault = "UPDATE siteserver_SeoMeta SET [IsDefault] = @IsDefault WHERE PublishmentSystemID = @PublishmentSystemID";

		private const string SqlUpdateIsDefault = "UPDATE siteserver_SeoMeta SET [IsDefault] = @IsDefault WHERE SeoMetaID = @SeoMetaID";

		private const string SqlDeleteSeoMeta = "DELETE FROM siteserver_SeoMeta WHERE SeoMetaID = @SeoMetaID";

		//siteserver_SeoMetasInNodes
		private const string SqlInsertMatch = "INSERT INTO siteserver_SeoMetasInNodes (NodeID, IsChannel, SeoMetaID, PublishmentSystemID) VALUES (@NodeID, @IsChannel, @SeoMetaID, @PublishmentSystemID)";

		private const string SqlDeleteMatchByNodeId = "DELETE FROM siteserver_SeoMetasInNodes WHERE NodeID = @NodeID AND IsChannel = @IsChannel";

		private const string SqlSelectSeoMetaIdByNodeId = "SELECT SeoMetaID FROM siteserver_SeoMetasInNodes WHERE NodeID = @NodeID AND IsChannel = @IsChannel";

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

		//siteserver_SeoMetasInNodes
		private const string ParmNodeId = "@NodeID";
		private const string ParmIsChannel = "@IsChannel";

		public void Insert(SeoMetaInfo info)
		{
			if (info.IsDefault)
			{
				SetAllIsDefaultToFalse(info.PublishmentSystemId);
			}

            var sqlString = "INSERT INTO siteserver_SeoMeta (PublishmentSystemID, SeoMetaName, [IsDefault], [PageTitle], [Keywords], [Description], [Copyright], [Author], [Email], [Language], [Charset], [Distribution], [Rating], [Robots], [RevisitAfter], [Expires]) VALUES (@PublishmentSystemID, @SeoMetaName, @IsDefault, @PageTitle, @Keywords, @Description, @Copyright, @Author, @Email, @Language, @Charset, @Distribution, @Rating, @Robots, @RevisitAfter, @Expires)";

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, info.PublishmentSystemId),
				GetParameter(ParmSeoMetaName, EDataType.VarChar, 50, info.SeoMetaName),
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmPageTitle, EDataType.NVarChar, 80, info.PageTitle),
				GetParameter(ParmKeywords, EDataType.NVarChar, 100, info.Keywords),
				GetParameter(ParmDescription, EDataType.NVarChar, 200, info.Description),
				GetParameter(ParmCopyright, EDataType.NVarChar, 255, info.Copyright),
				GetParameter(ParmAuthor, EDataType.NVarChar, 50, info.Author),
				GetParameter(ParmEmail, EDataType.NVarChar, 50, info.Email),
				GetParameter(ParmLanguage, EDataType.VarChar, 50, info.Language),
				GetParameter(ParmCharset, EDataType.VarChar, 50, info.Charset),
				GetParameter(ParmDistribution, EDataType.VarChar, 50, info.Distribution),
				GetParameter(ParmRating, EDataType.VarChar, 50, info.Rating),
				GetParameter(ParmRobots, EDataType.VarChar, 50, info.Robots),
				GetParameter(ParmRevisitAfter, EDataType.VarChar, 50, info.RevisitAfter),
				GetParameter(ParmExpires, EDataType.VarChar, 50, info.Expires)
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
				GetParameter(ParmSeoMetaName, EDataType.VarChar, 50, info.SeoMetaName),
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, info.IsDefault.ToString()),
				GetParameter(ParmPageTitle, EDataType.NVarChar, 80, info.PageTitle),
				GetParameter(ParmKeywords, EDataType.NVarChar, 100, info.Keywords),
				GetParameter(ParmDescription, EDataType.NVarChar, 200, info.Description),
				GetParameter(ParmCopyright, EDataType.NVarChar, 255, info.Copyright),
				GetParameter(ParmAuthor, EDataType.NVarChar, 50, info.Author),
				GetParameter(ParmEmail, EDataType.NVarChar, 50, info.Email),
				GetParameter(ParmLanguage, EDataType.VarChar, 50, info.Language),
				GetParameter(ParmCharset, EDataType.VarChar, 50, info.Charset),
				GetParameter(ParmDistribution, EDataType.VarChar, 50, info.Distribution),
				GetParameter(ParmRating, EDataType.VarChar, 50, info.Rating),
				GetParameter(ParmRobots, EDataType.VarChar, 50, info.Robots),
				GetParameter(ParmRevisitAfter, EDataType.VarChar, 50, info.RevisitAfter),
				GetParameter(ParmExpires, EDataType.VarChar, 50, info.Expires),
				GetParameter(ParmSeoMetaId, EDataType.Integer, info.SeoMetaId)
			};
							
			ExecuteNonQuery(SqlUpdateSeoMeta, updateParms);
		}

		private void SetAllIsDefaultToFalse(int publishmentSystemId)
		{
			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, false.ToString()),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

			ExecuteNonQuery(SqlUpdateAllIsDefault, updateParms);
		}

        public void SetDefault(int publishmentSystemId, int seoMetaId, bool defaultValue)
		{
			SetAllIsDefaultToFalse(publishmentSystemId);

			var updateParms = new IDataParameter[]
			{
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, defaultValue.ToString()),
				GetParameter(ParmSeoMetaId, EDataType.Integer, seoMetaId)
			};
							
			ExecuteNonQuery(SqlUpdateIsDefault, updateParms);
		}

		public void Delete(int seoMetaId)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmSeoMetaId, EDataType.Integer, seoMetaId)
			};
							
			ExecuteNonQuery(SqlDeleteSeoMeta, parms);
		}

		public SeoMetaInfo GetSeoMetaInfo(int seoMetaId)
		{
			SeoMetaInfo info = null;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmSeoMetaId, EDataType.Integer, seoMetaId)
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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmSeoMetaName, EDataType.VarChar, 50, seoMetaName)
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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmSeoMetaName, EDataType.VarChar, 50, importSeoMetaName)
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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, true.ToString())
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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmIsDefault, EDataType.VarChar, 18, true.ToString())
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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
			};

			var enumerable = (IEnumerable)ExecuteReader(SqlSelectAllSeoMeta, parms);
			return enumerable;
		}

		public ArrayList GetSeoMetaInfoArrayListByPublishmentSystemId(int publishmentSystemId)
		{
			var arraylist = new ArrayList();

			var parms = new IDataParameter[]
			{
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
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
				GetParameter(ParmSeoMetaId, EDataType.Integer, seoMetaId)
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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
				GetParameter(ParmSeoMetaName, EDataType.VarChar, 50, seoMetaName)
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
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
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

		//siteserver_SeoMetasInNodes
        public void InsertMatch(int publishmentSystemId, int nodeId, int seoMetaId, bool isChannel)
		{
			var lastSeoMetaId = GetSeoMetaIdByNodeId(nodeId, isChannel);
			if (lastSeoMetaId != 0)
			{
                DeleteMatch(publishmentSystemId, nodeId, isChannel);
			}

			var insertParms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId),
				GetParameter(ParmIsChannel, EDataType.VarChar, 18, isChannel.ToString()),
				GetParameter(ParmSeoMetaId, EDataType.Integer, seoMetaId),
				GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId),
			};
							
			ExecuteNonQuery(SqlInsertMatch, insertParms);
            SeoManager.RemoveCache(publishmentSystemId);
		}

        public void DeleteMatch(int publishmentSystemId, int nodeId, bool isChannel)
		{
			var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId),
				GetParameter(ParmIsChannel, EDataType.VarChar, 18, isChannel.ToString()),
			};
							
			ExecuteNonQuery(SqlDeleteMatchByNodeId, parms);
            SeoManager.RemoveCache(publishmentSystemId);
		}


        public int GetSeoMetaIdByNodeId(int nodeId, bool isChannel)
		{
			var seoMetaId = 0;

			var parms = new IDataParameter[]
			{
				GetParameter(ParmNodeId, EDataType.Integer, nodeId),
				GetParameter(ParmIsChannel, EDataType.VarChar, 18, isChannel.ToString())
			};

			using (var rdr = ExecuteReader(SqlSelectSeoMetaIdByNodeId, parms))
			{
				if (rdr.Read())
				{
                    seoMetaId = GetInt(rdr, 0);
                }
				rdr.Close();
			}

			return seoMetaId;
		}

        public List<int>[] GetSeoMetaLists(int publishmentSystemId)
        {
            var sqlString = "SELECT NodeID, IsChannel FROM siteserver_SeoMetasInNodes WHERE PublishmentSystemID = " + publishmentSystemId;

            var list1 = new List<int>();
            var list2 = new List<int>();

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var nodeId = GetInt(rdr, 0);
                    var isChannel = GetBool(rdr, 1);

                    if (isChannel)
                    {
                        if (!list1.Contains(nodeId))
                        {
                            list1.Add(nodeId);
                        }
                    }
                    else
                    {
                        if (!list2.Contains(nodeId))
                        {
                            list2.Add(nodeId);
                        }
                    }
                }
                rdr.Close();
            }

            return new[] { list1, list2 };
        }

	}
}
