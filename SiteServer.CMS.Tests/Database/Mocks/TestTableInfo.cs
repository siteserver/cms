using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Database.Core;
using SiteServer.Utils;

namespace SiteServer.CMS.Tests.Database.Mocks
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
}
