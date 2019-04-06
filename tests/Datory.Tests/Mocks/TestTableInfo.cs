using System;
using Datory.Utils;

namespace Datory.Tests.Mocks
{
    [Table("TestTable")]
    public class TestTableInfo : Entity
    {
        [TableColumn(Length = 100)]
        public string VarChar100 { get; set; }

        [TableColumn(Length = 100)]
        public string VarCharDefault { get; set; }

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
            get => ConvertUtils.ToBool(IsLockedOut);
            set => IsLockedOut = value.ToString();
        }
    }
}
