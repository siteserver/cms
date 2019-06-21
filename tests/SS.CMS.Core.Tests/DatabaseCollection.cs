using System;
using System.Collections.Generic;
using System.Text;
using SS.CMS.Utils.Tests;
using Xunit;

namespace SS.CMS.Core.Tests
{
    [CollectionDefinition("Database collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
