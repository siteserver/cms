using System;
using System.Globalization;
using System.Linq;
using Datory.Tests.Mocks;
using Datory.Utils;
using SqlKata;
using Xunit;
using Xunit.Abstractions;

namespace Datory.Tests.Core
{
    [TestCaseOrderer("Datory.Tests.PriorityOrderer", "Datory.Tests")]
    public class GenericRepositoryTest : IClassFixture<EnvironmentFixture>
    {
        public EnvironmentFixture Fixture { get; }
        private readonly ITestOutputHelper _output;
        private readonly Repository<TestTableInfo> _repository;

        public GenericRepositoryTest(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            Fixture = fixture;
            _output = output;
            _repository = new Repository<TestTableInfo>(EnvUtils.DatabaseType, EnvUtils.ConnectionString);
        }

        [SkippableFact, TestPriority(0)]
        public void Start()
        {
             Skip.IfNot(EnvUtils.IntegrationTestMachine);

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
            Assert.Equal(100, varCharDefaultColumn.DataLength);

            var contentColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.Content));
            Assert.NotNull(contentColumn);
            Assert.Equal(DataType.Text, contentColumn.DataType);

            var isLockedOutColumn = tableColumns.FirstOrDefault(x => x.AttributeName == "IsLockedOut");
            Assert.NotNull(isLockedOutColumn);

            var lockedColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.Locked));
            Assert.Null(lockedColumn);

            var isExists = DatoryUtils.IsTableExists(EnvUtils.DatabaseType, EnvUtils.ConnectionString, tableName);
            if (isExists)
            {
                DatoryUtils.DropTable(EnvUtils.DatabaseType, EnvUtils.ConnectionString, tableName);
            }

            DatoryUtils.CreateTable(EnvUtils.DatabaseType, EnvUtils.ConnectionString, tableName, tableColumns);
        }

        [SkippableFact, TestPriority(1)]
        public void TestInsert()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var id = _repository.Insert(null);
            Assert.Equal(0, id);

            var dataInfo = new TestTableInfo();
            _repository.Insert(dataInfo);
            Assert.Equal(1, dataInfo.Id);
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
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
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Locked);

            _output.WriteLine(dataInfo.Guid);
            _output.WriteLine(dataInfo.LastModifiedDate.ToString());
        }

        [SkippableFact, TestPriority(2)]
        public void TestGet()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var dataInfo = _repository.Get(1);
            Assert.NotNull(dataInfo);
            Assert.Equal(1, dataInfo.Id);
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Null(dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);

            _output.WriteLine(dataInfo.Guid);
            _output.WriteLine(dataInfo.LastModifiedDate.ToString());

            dataInfo = _repository.Get(2);
            Assert.NotNull(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
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
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var dataInfo = _repository.Get(new Query().Where(Attr.VarChar100, "string"));
            Assert.NotNull(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.Get(dataInfo.Guid);
            Assert.NotNull(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.Get(dataInfo.Guid);
            Assert.NotNull(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.Get(new Query().Where(Attr.VarChar100, "not exists"));
            Assert.Null(dataInfo);

            dataInfo = _repository.Get(new Query());
            Assert.NotNull(dataInfo);
        }

        [SkippableFact, TestPriority(2)]
        public void TestExists()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

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
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var count = _repository.Count();
            Assert.Equal(2, count);

            count = _repository.Count(new Query().Where("Id", 1));
            Assert.Equal(1, count);
        }

        [SkippableFact, TestPriority(2)]
        public void TestGetValue()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var guid = _repository.Get<string>(new Query()
                .Select(nameof(Entity.Guid)).Where("Id", 1));
            Assert.True(ConvertUtils.IsGuid(guid));

            var date = _repository.Get<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.Date)).Where("Guid", guid));
            Assert.False(date.HasValue);

            var lastModifiedDate = _repository.Get<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.LastModifiedDate))
                .Where("Guid", guid));
            Assert.True(lastModifiedDate.HasValue);
            _output.WriteLine(lastModifiedDate.Value.ToString(CultureInfo.InvariantCulture));
        }

        [SkippableFact, TestPriority(2)]
        public void TestGetValues()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var guidList = _repository.GetAll<string>(new Query()
                .Select(nameof(TestTableInfo.Guid))
                .Where("Id", 1)).ToList();

            Assert.NotNull(guidList);
            Assert.True(ConvertUtils.IsGuid(guidList.First()));

            var dateList = _repository.GetAll<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.Date))
                .Where("Guid", guidList.First())).ToList();
            Assert.False(dateList.First().HasValue);

            var lastModifiedDateList = _repository.GetAll<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.LastModifiedDate))
                .Where("Id", 1)).ToList();
            Assert.True(lastModifiedDateList.First().HasValue);
        }

        [SkippableFact, TestPriority(2)]
        public void TestGetAll()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var allList = _repository.GetAll().ToList();
            Assert.Equal(2, allList.Count);

            var list = _repository.GetAll(new Query()
                .Where("Guid", allList[0].Guid)).ToList();

            Assert.Single(list);
        }

        [SkippableFact, TestPriority(3)]
        public void TestUpdate()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var dataInfo = _repository.Get(1);
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            var lastModified = dataInfo.LastModifiedDate.Value.Ticks;

            dataInfo.Content = "new content";
            dataInfo.LastModifiedDate = DateTime.Now.AddDays(-1);

            var updated = _repository.Update(dataInfo);
            Assert.True(updated);

            Assert.Equal(1, dataInfo.Id);
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
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

            updated = _repository.Update((TestTableInfo)null);
            Assert.False(updated);
        }

        [SkippableFact, TestPriority(3)]
        public void TestUpdateWithParameters()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var lastModified = _repository.Get<DateTime?>(new Query()
                .Select(nameof(Entity.LastModifiedDate)).Where("Id", 1));
            Assert.True(lastModified.HasValue);

            var updated = _repository.Update(new Query()
                .Set("Content", "new content2")
                .Set("LastModifiedDate", DateTime.Now.AddDays(-1))
                .Where(nameof(Attr.Id), 1));
            Assert.True(updated == 1);

            var dataInfo = _repository.Get(1);
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            var lastModified2 = dataInfo.LastModifiedDate.Value.Ticks;

            Assert.Equal(1, dataInfo.Id);
            Assert.True(ConvertUtils.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Null(dataInfo.VarChar100);
            Assert.Null(dataInfo.VarCharDefault);
            Assert.Equal("new content2", dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);
            
            Assert.True(lastModified2 > lastModified.Value.Ticks);

            updated = _repository.Update(new Query());
            Assert.True(updated == 2);
        }

        [SkippableFact, TestPriority(3)]
        public void TestUpdateAll()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var lastModified = _repository.Get<DateTime?>(new Query()
                .Select(nameof(Entity.LastModifiedDate))
                .Where("Id", 1));
            Assert.True(lastModified.HasValue);

            var updatedCount = _repository.Update(new Query()
                .Set("Content", "new content2")
                .Set("LastModifiedDate", DateTime.Now.AddDays(-1))
                .Where("Id", 1));

            Assert.True(updatedCount == 1);

            updatedCount = _repository.Update(new Query()
                .Set("Content", "new content3")
                .Where("Content", "new content2"));

            Assert.True(updatedCount == 1);

            var dataInfo = _repository.Get(1);
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            var lastModified2 = dataInfo.LastModifiedDate.Value.Ticks;

            Assert.True(lastModified2 > lastModified.Value.Ticks);
        }

        [SkippableFact, TestPriority(3)]
        public void TestIncrementAll()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var dataInfo = _repository.Get(1);
            Assert.Equal(0, dataInfo.Num);

            var affected = _repository.Increment(new Query().Select(Attr.Num).Where(Attr.Id, 1));
            Assert.True(affected == 1);

            dataInfo = _repository.Get(1);
            Assert.Equal(1, dataInfo.Num);

            affected = _repository.Decrement(new Query().Select(Attr.Num).Where(Attr.Id, 1));
            Assert.True(affected == 1);

            dataInfo = _repository.Get(1);
            Assert.Equal(0, dataInfo.Num);
        }

        [SkippableFact, TestPriority(4)]
        public void TestDelete()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            var deleted = _repository.Delete(1);
            Assert.True(deleted);

            deleted = _repository.Delete(3);
            Assert.False(deleted);
        }

        [SkippableFact, TestPriority(5)]
        public void End()
        {
            Skip.IfNot(EnvUtils.IntegrationTestMachine);

            DatoryUtils.DropTable(EnvUtils.DatabaseType, EnvUtils.ConnectionString, _repository.TableName);
        }
    }
}
