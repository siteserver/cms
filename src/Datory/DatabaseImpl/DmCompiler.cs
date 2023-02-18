using SqlKata.Compilers;

namespace Datory.DatabaseImpl
{
    public class DmCompiler : OracleCompiler
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