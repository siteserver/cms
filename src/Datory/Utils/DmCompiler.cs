using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dapper;
using SqlKata.Compilers;
using Dm;
using Datory.DatabaseImpl;

namespace Datory.Utils
{
    internal class DmCompiler : OracleCompiler
    {
        public override string Wrap(string value)
        {
            return DmImpl.Wrap(value);
        }

        public override string CompileTrue()
        {
            return "1";
        }

        public override string CompileFalse()
        {
            return "0";
        }
    }
}