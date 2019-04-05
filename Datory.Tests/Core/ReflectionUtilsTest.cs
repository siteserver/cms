using Datory.Utils;
using Xunit;

namespace Datory.Tests.Core
{
    public class ReflectionUtilsTest
    {
        public class MyClass
        {
            public string Foo { get; set; }
            private string Bar { get; set; }
            protected string Pro { get; set; }
            internal string Inter { get; set; }
        }

        [Fact]
        public void TestGetAllInstancePropertyInfosEmpty()
        {
            var properties = ReflectionUtils.GetTypeProperties(typeof(int));
            Assert.Empty(properties);
        }

        [Fact]
        public void TestGetAllInstancePropertyInfosObject()
        {
            var properties = ReflectionUtils.GetTypeProperties(typeof(MyClass));
            Assert.Equal(4, properties.Count);
        }

        [Fact]
        public void TestToKeyValueListEmpty()
        {
            var kvs = ReflectionUtils.ToKeyValueList(null);
            Assert.Empty(kvs);
        }

        [Fact]
        public void TestToKeyValueListObject()
        {
            var kvs = ReflectionUtils.ToKeyValueList(new
            {
                A = "a",
                B = "b",
                C = "c",
                D = "d"
            });
            Assert.Equal(4, kvs.Count);
        }
    }
}
