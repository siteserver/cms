using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteServer.CMS.Tests.Database.Mocks
{
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
}
