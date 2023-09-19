using SqlKata.Compilers;

namespace Datory.DatabaseImpl
{
    public class KingbaseESCompiler : PostgresCompiler
    {
        public override string Wrap(string value)
        {
            return KingbaseESImpl.Wrap(value);
        }

        // public override string CompileTrue()
        // {
        //     return "1";
        // }

        // public override string CompileFalse()
        // {
        //     return "0";
        // }
    }
}