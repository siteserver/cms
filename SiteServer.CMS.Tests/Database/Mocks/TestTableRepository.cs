using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteServer.CMS.Database.Core;
using SqlKata;

namespace SiteServer.CMS.Tests.Database.Mocks
{
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


        public new bool UpdateObject(TestTableInfo dataInfo)
        {
            return base.UpdateObject(dataInfo);
        }


        public new int UpdateAll(Query query)
        {
            return base.UpdateAll(query);
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
