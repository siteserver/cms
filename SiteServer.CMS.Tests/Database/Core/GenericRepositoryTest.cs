using System;
using System.Linq;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Wrapper;
using SiteServer.CMS.Tests.Database.Mocks;
using SiteServer.Plugin;
using SiteServer.Utils;
using SqlKata;
using Xunit;
using Xunit.Abstractions;

namespace SiteServer.CMS.Tests.Database.Core
{
    [TestCaseOrderer("SiteServer.CMS.Tests.PriorityOrderer", "SiteServer.CMS.Tests")]
    public class GenericRepositoryTest : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }
        private readonly ITestOutputHelper _output;
        private readonly TestTableRepository _repository;

        public GenericRepositoryTest(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            _output = output;
            _repository = new TestTableRepository();
        }

        [SkippableFact, TestPriority(0)]
        public void Start()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var tableName = _repository.TableName;
            var tableColumns = _repository.TableColumns;

            Assert.Equal("TestTable", tableName);
            Assert.NotNull(tableColumns);
            Assert.Equal(10, tableColumns.Count);

            var varChar100Column = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.VarChar100));
            Assert.NotNull(varChar100Column);
            Assert.Equal(DataType.VarChar, varChar100Column.DataType);
            Assert.Equal(100, varChar100Column.DataLength);

            var varCharDefaultColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.VarCharDefault));
            Assert.NotNull(varCharDefaultColumn);
            Assert.Equal(DataType.VarChar, varCharDefaultColumn.DataType);
            Assert.Equal(2000, varCharDefaultColumn.DataLength);

            var contentColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.Content));
            Assert.NotNull(contentColumn);
            Assert.Equal(DataType.Text, contentColumn.DataType);

            var isLockedOutColumn = tableColumns.FirstOrDefault(x => x.AttributeName == "IsLockedOut");
            Assert.NotNull(isLockedOutColumn);

            var lockedColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.Locked));
            Assert.Null(lockedColumn);

            var isExists = DatabaseApi.Instance.IsTableExists(tableName);
            if (isExists)
            {
                DatabaseApi.Instance.DropTable(_repository.TableName);
            }

            var created = DatabaseApi.Instance.CreateTable(tableName, tableColumns, string.Empty, false, out _, out var sqlString);
            _output.WriteLine(sqlString);
            Assert.True(created);
        }

        [SkippableFact, TestPriority(1)]
        public void TestInsert()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var id = _repository.Insert(null);
            Assert.Equal(0, id);

            var dataInfo = new TestTableInfo();
            _repository.Insert(dataInfo);
            Assert.Equal(1, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Null(dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);
            Assert.False(dataInfo.Locked);

            dataInfo = new TestTableInfo
            {
                Guid = "wrong guid",
                VarChar100 = "string",
                Num = -100,
                Date = DateTime.Now,
                Locked = true
            };
            _repository.Insert(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Locked);

            _output.WriteLine(dataInfo.Guid);
            _output.WriteLine(DateUtils.GetDateAndTimeString(dataInfo.LastModifiedDate));
        }

        [SkippableFact, TestPriority(2)]
        public void TestGet()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var dataInfo = _repository.First(1);
            Assert.NotNull(dataInfo);
            Assert.Equal(1, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Null(dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);

            _output.WriteLine(dataInfo.Guid);
            _output.WriteLine(DateUtils.GetDateAndTimeString(dataInfo.LastModifiedDate));

            dataInfo = _repository.First(2);
            Assert.NotNull(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);
        }

        [SkippableFact, TestPriority(2)]
        public void TestGetWithParameters()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var dataInfo = _repository.First(new Query().Where(Attr.VarChar100, "string"));
            Assert.NotNull(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.First(dataInfo.Guid);
            Assert.NotNull(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.First(dataInfo.Guid);
            Assert.NotNull(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.First(new Query().Where(Attr.VarChar100, "not exists"));
            Assert.Null(dataInfo);

            dataInfo = _repository.First(new Query());
            Assert.NotNull(dataInfo);
        }

        [SkippableFact, TestPriority(2)]
        public void TestExists()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var exists = _repository.Exists(new Query()
                .Where(nameof(TestTableInfo.VarChar100), "string"));
            Assert.True(exists);

            exists = _repository.Exists(new Query().Where(nameof(TestTableInfo.VarChar100), "not exists"));
            Assert.False(exists);

            exists = _repository.Exists(new Query());
            Assert.True(exists);
        }

        [SkippableFact, TestPriority(2)]
        public void TestCount()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var count = _repository.Count();
            Assert.Equal(2, count);

            count = _repository.Count(new Query().Where("Id", 1));
            Assert.Equal(1, count);
        }

        [SkippableFact, TestPriority(2)]
        public void TestGetValue()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var guid = _repository.GetValue<string>(new Query()
                .Select(nameof(DynamicEntity.Guid)).Where("Id", 1));
            Assert.True(StringUtils.IsGuid(guid));

            var date = _repository.GetValue<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.Date)).Where("Guid", guid));
            Assert.False(date.HasValue);

            var lastModifiedDate = _repository.GetValue<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.LastModifiedDate))
                .Where("Guid", guid));
            Assert.True(lastModifiedDate.HasValue);
            _output.WriteLine(DateUtils.GetDateAndTimeString(lastModifiedDate.Value));
        }

        [SkippableFact, TestPriority(2)]
        public void TestGetValues()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var guidList = _repository.GetValueList<string>(new Query()
                .Select(nameof(TestTableInfo.Guid))
                .Where("Id", 1)).ToList();

            Assert.NotNull(guidList);
            Assert.True(StringUtils.IsGuid(guidList.First()));

            var dateList = _repository.GetValueList<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.Date))
                .Where("Guid", guidList.First())).ToList();
            Assert.False(dateList.First().HasValue);

            var lastModifiedDateList = _repository.GetValueList<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.LastModifiedDate))
                .Where("Id", 1)).ToList();
            Assert.True(lastModifiedDateList.First().HasValue);
        }

        [SkippableFact, TestPriority(2)]
        public void TestGetAll()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var allList = _repository.GetAll().ToList();
            Assert.Equal(2, allList.Count);

            var list = _repository.GetAll(new Query()
                .Where("Guid", allList[0].Guid)).ToList();

            Assert.Single(list);
        }

        [SkippableFact, TestPriority(3)]
        public void TestUpdate()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var dataInfo = _repository.First(1);
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            var lastModified = dataInfo.LastModifiedDate.Value.Ticks;

            dataInfo.Content = "new content";
            dataInfo.LastModifiedDate = DateTime.Now.AddDays(-1);

            var updated = _repository.UpdateObject(dataInfo);
            Assert.True(updated);

            Assert.Equal(1, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Null(dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Equal("new content", dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);

            var lastModified2 = dataInfo.LastModifiedDate.Value.Ticks;
            _output.WriteLine(lastModified.ToString());
            _output.WriteLine(lastModified2.ToString());
            Assert.True(lastModified2 > lastModified);

            updated = _repository.UpdateObject(null);
            Assert.False(updated);
        }

        [SkippableFact, TestPriority(3)]
        public void TestUpdateWithParameters()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var lastModified = _repository.GetValue<DateTime?>(new Query()
                .Select(nameof(DynamicEntity.LastModifiedDate)).Where("Id", 1));
            Assert.True(lastModified.HasValue);

            var updated = _repository.UpdateAll(new Query()
                .Set("Content", "new content2")
                .Set("LastModifiedDate", DateTime.Now.AddDays(-1))
                .Where(nameof(Attr.Id), 1));
            Assert.True(updated == 1);

            var dataInfo = _repository.First(1);
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            var lastModified2 = dataInfo.LastModifiedDate.Value.Ticks;

            Assert.Equal(1, dataInfo.Id);
            Assert.True(StringUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Null(dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Equal("new content2", dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);
            
            Assert.True(lastModified2 > lastModified.Value.Ticks);

            updated = _repository.UpdateAll(new Query());
            Assert.True(updated == 2);
        }

        [SkippableFact, TestPriority(3)]
        public void TestUpdateAll()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var lastModified = _repository.GetValue<DateTime?>(new Query()
                .Select(nameof(DynamicEntity.LastModifiedDate))
                .Where("Id", 1));
            Assert.True(lastModified.HasValue);

            var updatedCount = _repository.UpdateAll(new Query()
                .Set("Content", "new content2")
                .Set("LastModifiedDate", DateTime.Now.AddDays(-1))
                .Where("Id", 1));

            Assert.True(updatedCount == 1);

            updatedCount = _repository.UpdateAll(new Query()
                .Set("Content", "new content3")
                .Where("Content", "new content2"));

            Assert.True(updatedCount == 1);

            var dataInfo = _repository.First(1);
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            var lastModified2 = dataInfo.LastModifiedDate.Value.Ticks;

            Assert.True(lastModified2 > lastModified.Value.Ticks);

            updatedCount = _repository.UpdateAll(null);
            Assert.True(updatedCount == 2);
        }

        [SkippableFact, TestPriority(3)]
        public void TestIncrementAll()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var dataInfo = _repository.First(1);
            Assert.Equal(0, dataInfo.Num);

            var affected = _repository.IncrementAll(Attr.Num, new Query().Where(Attr.Id, 1));
            Assert.True(affected == 1);

            dataInfo = _repository.First(1);
            Assert.Equal(1, dataInfo.Num);

            affected = _repository.DecrementAll(Attr.Num, new Query().Where(Attr.Id, 1));
            Assert.True(affected == 1);

            dataInfo = _repository.First(1);
            Assert.Equal(0, dataInfo.Num);
        }

        [SkippableFact, TestPriority(4)]
        public void TestDelete()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            var deleted = _repository.Delete(1);
            Assert.True(deleted);

            deleted = _repository.Delete(3);
            Assert.False(deleted);
        }

        [SkippableFact, TestPriority(5)]
        public void End()
        {
            Skip.IfNot(TestEnv.IntegrationTestMachine);

            DatabaseApi.Instance.DropTable(_repository.TableName);
        }
    }
}
