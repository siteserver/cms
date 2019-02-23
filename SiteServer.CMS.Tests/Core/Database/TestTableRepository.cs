using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;
using SqlKata;

namespace SiteServer.CMS.Tests.Core.Database
{
    [Table("TestTable")]
    public class TestTableInfo : IDataInfo
    {
        public int Id { get; set; }

        public string Guid { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        [VarChar(100)]
        public string VarChar100 { get; set; }

        public string VarCharDefault { get; set; }

        [Text]
        public string Content { get; set; }

        public int Num { get; set; }

        public decimal Currency { get; set; }

        public DateTime? Date { get; set; }

        private string IsLockedOut { get; set; }

        [Computed]
        public bool Locked
        {
            get => TranslateUtils.ToBool(IsLockedOut);
            set => IsLockedOut = value.ToString();
        }
    }

    public class Attr
    {
        public const string Id = nameof(TestTableInfo.Id);

        public const string Guid = nameof(TestTableInfo.Guid);

        public const string LastModifiedDate = nameof(TestTableInfo.LastModifiedDate);

        public const string VarChar100 = nameof(TestTableInfo.VarChar100);

        public const string VarCharDefault = nameof(TestTableInfo.VarCharDefault);

        public const string Content = nameof(TestTableInfo.Content);

        public const string Num = nameof(TestTableInfo.Num);

        public const string Currency = nameof(TestTableInfo.Currency);

        public const string Date = nameof(TestTableInfo.Date);

        public const string IsLockedOut = "IsLockedOut";
    }

    public class TestTableRepository : GenericRepository<TestTableInfo>
    {
        public new Query Q => base.Q;

        public int Insert(TestTableInfo dataInfo)
        {
            return InsertObject(dataInfo);
        }

        public new bool Exists(int id)
        {
            return base.Exists(id);
        }

        public new bool Exists(string guid)
        {
            return base.Exists(guid);
        }

        public new bool Exists(Query query = null)
        {
            return base.Exists(query);
        }
        
        public new int Count(Query query = null)
        {
            return base.Count(query);
        }

        public new TValue GetValue<TValue>(Query query)
        {
            return base.GetValue<TValue>(query);
        }

        public new int Max(string columnName, Query query)
        {
            return base.Max(columnName, query);
        }

        public new IList<TValue> GetValueList<TValue>(Query query = null)
        {
            return base.GetValueList<TValue>(query);
        }

        public TestTableInfo First(int id)
        {
            return GetObjectById(id);
        }

        public TestTableInfo First(string guid)
        {
            return GetObjectByGuid(guid);
        }


        public TestTableInfo First(Query query = null)
        {
            return GetObject(query);
        }



        public IList<TestTableInfo> GetAll(Query query = null)
        {
            return GetObjectList(query);
        }


        public bool Update(TestTableInfo dataInfo)
        {
            return UpdateObject(dataInfo);
        }


        public new int UpdateValue(IDictionary<string, object> values, Query query)
        {
            return base.UpdateValue(values, query);
        }


        public bool Delete(int id)
        {
            return DeleteById(id);
        }

        public bool Delete(string guid)
        {
            return DeleteByGuid(guid);
        }

        public new int DeleteAll(Query query = null)
        {
            return base.DeleteAll(query);
        }

        public new int IncrementAll(string columnName, Query query, int num = 1)
        {
            return base.IncrementAll(columnName, query, num);
        }

        public new int DecrementAll(string columnName, Query query, int num = 1)
        {
            return base.DecrementAll(columnName, query, num);
        }
    }
}
