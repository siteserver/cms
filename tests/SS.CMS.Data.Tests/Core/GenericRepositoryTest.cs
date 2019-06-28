using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SqlKata;
using SS.CMS.Data.Tests.Mocks;
using SS.CMS.Data.Utils;
using SS.CMS.Utils.Tests;
using Xunit;
using Xunit.Abstractions;

namespace SS.CMS.Data.Tests.Core
{
    public class GenericRepositoryTest : IClassFixture<EnvironmentFixture>, IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly Repository<TestTableInfo> _repository;

        public GenericRepositoryTest(EnvironmentFixture fixture, ITestOutputHelper output)
        {
            _output = output;

            var db = new Database(fixture.SettingsManager.DatabaseType, fixture.SettingsManager.DatabaseConnectionString);
            _repository = new Repository<TestTableInfo>(db);

            if (!TestEnv.IsTestMachine) return;

            var isExists = db.IsTableExists(_repository.TableName);
            if (isExists)
            {
                db.DropTable(_repository.TableName);
            }

            db.CreateTable(_repository.TableName, _repository.TableColumns);
        }

        public void Dispose()
        {
            if (!TestEnv.IsTestMachine) return;

            //_fixture.Db.DropTable(_repository.TableName);
        }

        [SkippableFact]
        public void Start()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var tableName = _repository.TableName;
            var tableColumns = _repository.TableColumns;

            Assert.Equal("TestTable", tableName);
            Assert.NotNull(tableColumns);
            Assert.Equal(12, tableColumns.Count);

            var varChar100Column = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.TypeVarChar100));
            Assert.NotNull(varChar100Column);
            Assert.Equal(DataType.VarChar, varChar100Column.DataType);
            Assert.Equal(100, varChar100Column.DataLength);

            var varCharDefaultColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.TypeVarCharDefault));
            Assert.NotNull(varCharDefaultColumn);
            Assert.Equal(DataType.VarChar, varCharDefaultColumn.DataType);
            Assert.Equal(DbUtils.VarCharDefaultLength, varCharDefaultColumn.DataLength);

            var boolColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.TypeBool));
            Assert.NotNull(boolColumn);
            Assert.Equal(DataType.Boolean, boolColumn.DataType);

            var contentColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.Content));
            Assert.NotNull(contentColumn);
            Assert.Equal(DataType.Text, contentColumn.DataType);

            var isLockedOutColumn = tableColumns.FirstOrDefault(x => x.AttributeName == "IsLockedOut");
            Assert.NotNull(isLockedOutColumn);

            var lockedColumn = tableColumns.FirstOrDefault(x => x.AttributeName == nameof(TestTableInfo.Locked));
            Assert.Null(lockedColumn);
        }

        [SkippableFact]
        public async Task InsertTest()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            var id = _repository.Insert(null);
            Assert.Equal(0, id);

            id = await _repository.InsertAsync(null);
            Assert.Equal(0, id);

            var dataInfo = new TestTableInfo();
            _repository.Insert(dataInfo);
            Assert.Equal(1, dataInfo.Id);
            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Null(dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);
            Assert.False(dataInfo.Locked);

            dataInfo = new TestTableInfo();
            await _repository.InsertAsync(dataInfo);
            Assert.Equal(2, dataInfo.Id);
            Assert.True(Utilities.IsGuid(dataInfo.Guid));

            dataInfo = new TestTableInfo
            {
                Guid = "wrong guid",
                TypeVarChar100 = "string",
                Num = -100,
                Date = DateTime.Now,
                Locked = true
            };
            _repository.Insert(dataInfo);
            Assert.Equal(3, dataInfo.Id);
            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Locked);

            _output.WriteLine(dataInfo.Guid);
            _output.WriteLine(dataInfo.LastModifiedDate.ToString());

            dataInfo = _repository.Get(1);
            Assert.NotNull(dataInfo);
            Assert.Equal(1, dataInfo.Id);
            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Null(dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);

            _output.WriteLine(dataInfo.Guid);
            _output.WriteLine(dataInfo.LastModifiedDate.ToString());

            dataInfo = _repository.Get(3);
            Assert.NotNull(dataInfo);
            Assert.Equal(3, dataInfo.Id);
            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.Get(new Query().Where(Attr.TypeVarChar100, "string"));
            Assert.NotNull(dataInfo);
            Assert.Equal(3, dataInfo.Id);
            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.Get(dataInfo.Guid);
            Assert.NotNull(dataInfo);
            Assert.Equal(3, dataInfo.Id);
            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.Get(dataInfo.Guid);
            Assert.NotNull(dataInfo);
            Assert.Equal(3, dataInfo.Id);
            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.Equal("string", dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
            Assert.Null(dataInfo.Content);
            Assert.Equal(-100, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.True(dataInfo.Date.HasValue);

            dataInfo = _repository.Get(new Query().Where(Attr.TypeVarChar100, "not exists"));
            Assert.Null(dataInfo);

            var exists = _repository.Exists(new Query()
                .Where(nameof(TestTableInfo.TypeVarChar100), "string"));
            Assert.True(exists);

            exists = _repository.Exists(new Query().Where(nameof(TestTableInfo.TypeVarChar100), "not exists"));
            Assert.False(exists);

            exists = _repository.Exists(new Query());
            Assert.True(exists);

            await _repository.DeleteAsync();
        }

        [SkippableFact]
        public async Task TestCount()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "string",
                Num = -100,
                Date = DateTime.Now,
                Locked = true
            });

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "test",
                Num = -100,
                Date = DateTime.Now,
                Locked = true
            });

            var count = _repository.Count();
            Assert.Equal(2, count);

            count = _repository.Count(new Query().Where("TypeVarChar100", "test"));
            Assert.Equal(1, count);

            await _repository.DeleteAsync();
        }

        [SkippableFact]
        public async Task TestGetValue()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                Guid = "wrong guid",
                TypeVarChar100 = "string"
            });

            var guid = _repository.Get<string>(new Query()
                .Select(nameof(Entity.Guid)).Where("TypeVarChar100", "string"));
            Assert.True(Utilities.IsGuid(guid));

            var date = _repository.Get<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.Date)).Where("Guid", guid));
            Assert.False(date.HasValue);

            var lastModifiedDate = _repository.Get<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.LastModifiedDate))
                .Where("Guid", guid));
            Assert.True(lastModifiedDate.HasValue);
            _output.WriteLine(lastModifiedDate.Value.ToString(CultureInfo.InvariantCulture));

            await _repository.DeleteAsync();
        }

        [SkippableFact]
        public async Task TestGetValues()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "string"
            });

            var guidList = _repository.GetAll<string>(new Query()
                .Select(nameof(TestTableInfo.Guid))
                .Where("TypeVarChar100", "string")).ToList();

            Assert.NotNull(guidList);
            Assert.True(Utilities.IsGuid(guidList.First()));

            var dateList = _repository.GetAll<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.Date))
                .Where("Guid", guidList.First())).ToList();
            Assert.False(dateList.First().HasValue);

            var lastModifiedDateList = _repository.GetAll<DateTime?>(new Query()
                .Select(nameof(TestTableInfo.LastModifiedDate))).ToList();
            lastModifiedDateList.ForEach(x => Assert.True(x.HasValue));

            await _repository.DeleteAsync();
        }

        [SkippableFact]
        public void TestGetAll()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str1"
            });
            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str2"
            });

            var allList = _repository.GetAll().ToList();
            Assert.Equal(2, allList.Count);

            var list = _repository.GetAll(new Query()
                .Where("Guid", allList[0].Guid)).ToList();

            Assert.Single(list);
        }

        [SkippableFact]
        public async Task TestUpdate()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str1"
            });
            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str2"
            });

            var dataInfo = _repository.Get(Q.Where("TypeVarChar100", "str2"));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            var lastModified = dataInfo.LastModifiedDate.Value.Ticks;

            dataInfo.Content = "new content";
            dataInfo.LastModifiedDate = DateTime.Now.AddDays(-1);

            var updated = _repository.Update(dataInfo);
            Assert.True(updated);

            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.NotEmpty(dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
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

            await _repository.DeleteAsync();
        }

        [SkippableFact]
        public async Task TestUpdateWithParameters()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str1"
            });
            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str2"
            });

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
            Assert.True(Utilities.IsGuid(dataInfo.Guid));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            Assert.NotNull(dataInfo.TypeVarChar100);
            Assert.Null(dataInfo.TypeVarCharDefault);
            Assert.Equal("new content2", dataInfo.Content);
            Assert.Equal(0, dataInfo.Num);
            Assert.Equal(0, dataInfo.Currency);
            Assert.False(dataInfo.Date.HasValue);
            Assert.True(dataInfo.Date == null);

            Assert.True(lastModified2 > lastModified.Value.Ticks);

            updated = _repository.Update(new Query());
            Assert.True(updated == 2);

            await _repository.DeleteAsync();
        }

        [SkippableFact]
        public async Task TestUpdateAll()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str1"
            });
            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str2"
            });

            var lastModified = _repository.Get<DateTime?>(new Query()
                .Select(nameof(Entity.LastModifiedDate))
                .Where("TypeVarChar100", "str1"));
            Assert.True(lastModified.HasValue);

            var updatedCount = _repository.Update(new Query()
                .Set("Content", "new content2")
                .Set("LastModifiedDate", DateTime.Now.AddDays(-1))
                .Where("TypeVarChar100", "str1"));

            Assert.True(updatedCount == 1);

            updatedCount = _repository.Update(new Query()
                .Set("Content", "new content3")
                .Where("Content", "new content2"));

            Assert.True(updatedCount == 1);

            var dataInfo = _repository.Get(Q.Where("TypeVarChar100", "str1"));
            Assert.True(dataInfo.LastModifiedDate.HasValue);
            var lastModified2 = dataInfo.LastModifiedDate.Value.Ticks;

            Assert.True(lastModified2 > lastModified.Value.Ticks);

            await _repository.DeleteAsync();
        }

        [SkippableFact]
        public async Task TestIncrementAll()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str1"
            });

            var dataInfo = _repository.Get(Q.Where("TypeVarChar100", "str1"));
            Assert.Equal(0, dataInfo.Num);

            var affected = _repository.Increment(Attr.Num, Q.Where("TypeVarChar100", "str1"));
            Assert.True(affected == 1);

            dataInfo = _repository.Get(Q.Where("TypeVarChar100", "str1"));
            Assert.Equal(1, dataInfo.Num);

            affected = _repository.Decrement(Attr.Num, Q.Where(Attr.Id, 1));
            Assert.True(affected == 1);

            dataInfo = _repository.Get(Q.Where("TypeVarChar100", "str1"));
            Assert.Equal(0, dataInfo.Num);

            await _repository.DeleteAsync();
        }

        [SkippableFact]
        public async Task TestDelete()
        {
            Skip.IfNot(TestEnv.IsTestMachine);

            _repository.Insert(new TestTableInfo
            {
                TypeVarChar100 = "str"
            });

            var deleted = await _repository.DeleteAsync(Q.Where("TypeVarChar100", "str"));
            Assert.Equal(1, deleted);

            Assert.False(await _repository.DeleteAsync(1));

            await _repository.DeleteAsync();
        }
    }
}
