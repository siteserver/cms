using System.Threading;


namespace Top.Api.Security
{

    /// <summary>
    /// 计数器
    /// </summary>
    public class SecurityCounter : SecurityConstants
    {

        private static long EncryptPhoneNum = 0;
        private static long EncryptNickNum = 0;
        private static long EncryptReceiverNameNum = 0;

        private static long DecryptPhoneNum = 0;
        private static long DecryptNickNum = 0;
        private static long DecryptReceiverNameNum = 0;

        private static long SearchPhoneNum = 0;
        private static long SearchNickNum = 0;
        private static long SearchReceiverNameNum = 0;

        public static long GetEncryptPhoneNum()
        {
            return EncryptPhoneNum;
        }

        public static long GetEncryptNickNum()
        {
            return EncryptNickNum;
        }

        public static long GetEncryptReceiverNameNum()
        {
            return EncryptReceiverNameNum;
        }

        public static long GetDecryptPhoneNum()
        {
            return DecryptPhoneNum;
        }

        public static long GetDecryptNickNum()
        {
            return DecryptNickNum;
        }

        public static long GetDecryptReceiverNameNum()
        {
            return DecryptReceiverNameNum;
        }

        public static long GetSearchPhoneNum()
        {
            return SearchPhoneNum;
        }

        public static long GetSearchNickNum()
        {
            return SearchNickNum;
        }

        public static long GetSearchReceiverNameNum()
        {
            return SearchReceiverNameNum;
        }

        public static void Reset()
        {
            EncryptPhoneNum = 0;
            EncryptNickNum = 0;
            EncryptReceiverNameNum = 0;

            DecryptPhoneNum = 0;
            DecryptNickNum = 0;
            DecryptReceiverNameNum = 0;

            SearchPhoneNum = 0;
            SearchNickNum = 0;
            SearchReceiverNameNum = 0;
        }

        public static void AddEncryptCount(string type)
        {
            if (PHONE.Equals(type))
            {
                Interlocked.Increment(ref EncryptPhoneNum);
            }
            else if (NICK.Equals(type))
            {
                Interlocked.Increment(ref EncryptNickNum);
            }
            else if (RECEIVER_NAME.Equals(type))
            {
                Interlocked.Increment(ref EncryptReceiverNameNum);
            }
        }

        public static void AddDecryptCount(string type)
        {
            if (PHONE.Equals(type))
            {
                Interlocked.Increment(ref DecryptPhoneNum);
            }
            else if (NICK.Equals(type))
            {
                Interlocked.Increment(ref DecryptNickNum);
            }
            else if (RECEIVER_NAME.Equals(type))
            {
                Interlocked.Increment(ref DecryptReceiverNameNum);
            }
        }

        public static void AddSearchCount(string type)
        {
            if (PHONE.Equals(type))
            {
                Interlocked.Increment(ref SearchPhoneNum);
            }
            else if (NICK.Equals(type))
            {
                Interlocked.Increment(ref SearchNickNum);
            }
            else if (RECEIVER_NAME.Equals(type))
            {
                Interlocked.Increment(ref SearchReceiverNameNum);
            }
        }
    }
}
