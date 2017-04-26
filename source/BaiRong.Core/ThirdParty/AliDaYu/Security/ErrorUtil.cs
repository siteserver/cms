namespace Top.Api.Security
{

    public class ErrorUtil
    {
        private static readonly string[] InvalidSessionCodes = { "26", "27", "53" };
        private static readonly string[] InvalidSubUserCodes = { "12" };
        private static readonly string InvalidSessionkey = "invalid-sessionkey";

        public static bool IsInvalidSession(SecretException secretException) {
            foreach (string code in InvalidSessionCodes)
            {
                if (code.Equals(secretException.ErrorCode) && InvalidSessionkey.Equals(secretException.SubErrorCode)) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsInvalidSubCode(SecretException secretException) {
            foreach (string code in InvalidSubUserCodes)
            {
                if (code.Equals(secretException.ErrorCode)) {
                    return true;
                }
            }
            return false;
        }
    }
}
