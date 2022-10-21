using SqlKata.Compilers;

namespace Datory.DatabaseImpl
{
    public class DmCompiler : OracleCompiler
    {
        public override string Wrap(string value)
        {
            return DmImpl.Wrap(value);
        }
    }
}