using System;
using SS.CMS.Plugin.Data;
using SS.CMS.Plugin.Data.Utils;

namespace SS.CMS.Plugin.Tests.Mocks
{
    [Table("TestTable")]
    public class TestTableInfo : Entity
    {
        [TableColumn(Length = 100)]
        public string TypeVarChar100 { get; set; }

        [TableColumn]
        public string TypeVarCharDefault { get; set; }

        [TableColumn]
        public bool TypeBool { get; set; }

        [TableColumn(Text = true)]
        public string Content { get; set; }

        [TableColumn]
        public int Num { get; set; }

        [TableColumn]
        public decimal Currency { get; set; }

        [TableColumn]
        public DateTime? Date { get; set; }

        [TableColumn]
        private string IsLockedOut { get; set; }

        public bool Locked
        {
            get => Utilities.ToBool(IsLockedOut);
            set => IsLockedOut = value.ToString();
        }
    }
}
