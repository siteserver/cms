using System;
using SS.CMS.Data.Utils;

namespace SS.CMS.Data.Tests.Mocks
{
    [DataTable("TestTable")]
    public class TestTableInfo : Entity
    {
        [DataColumn(Length = 100)]
        public string TypeVarChar100 { get; set; }

        [DataColumn]
        public string TypeVarCharDefault { get; set; }

        [DataColumn]
        public bool TypeBool { get; set; }

        [DataColumn(Text = true)]
        public string Content { get; set; }

        [DataColumn]
        public int Num { get; set; }

        [DataColumn]
        public decimal Currency { get; set; }

        [DataColumn]
        public DateTime? Date { get; set; }

        [DataColumn]
        private string IsLockedOut { get; set; }

        public bool Locked
        {
            get => Utilities.ToBool(IsLockedOut);
            set => IsLockedOut = value.ToString();
        }
    }
}
