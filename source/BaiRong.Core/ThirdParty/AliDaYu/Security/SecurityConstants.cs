using System;

namespace Top.Api.Security
{

    public class SecurityConstants
    {
        public const long DEFAULT_INTERVAL = 300;// 5分钟
        public const long DEFAULT_MAX_INTERVAL = 600;// 10分钟

        public const string PUBLISH_STATUS = "publish_status";// 发布状态
        public const string BETA_STATUS = "0";// BETA发布

        public const string CURRENT = "current_";
        public const string PREVIOUS = "previous_";
        public const string CURRENT_PHONE_ENCRYPT_TYPE = "current_phone";// 当前手机加密类型（1：普通加密，2：检索加密）
        public const string PREVIOUS_PHONE_ENCRYPT_TYPE = "previous_phone";// 上个版本手机加密类型（1：普通加密，2：检索加密）
        public const string CURRENT_NICK_ENCRYPT_TYPE = "current_nick";// 当前nick加密类型（1：普通加密，2：检索加密）
        public const string PREVIOUS_NICK_ENCRYPT_TYPE = "previous_nick";// 上个版本nick加密类型（1：普通加密，2：检索加密）
        public const string CURRENT_RECEIVER_NAME_ENCRYPT_TYPE = "current_receiver_name";// 当前收货人加密类型（1：普通加密，2：检索加密）
        public const string PREVIOUS_RECEIVER_NAME_ENCRYPT_TYPE = "previous_receiver_name";// 上个版本收货人加密类型（1：普通加密，2：检索加密）

        public const string NORMAL_ENCRYPT_TYPE = "1";
        public const string INDEX_ENCRYPT_TYPE = "2";
        public const string ENCRYPT_INDEX_COMPRESS_LEN = "encrypt_index_compress_len";// 密文滑窗压缩长度
        public const string ENCRYPT_SLIDE_SIZE = "encrypt_slide_size";// 滑动窗口大小
        public const int DEFAULT_ENCRYPT_SLIDE_SIZE = 4;
        public const int DEFAULT_INDEX_ENCRYPT_COMPRESS_LEN = 3;

        public const string RECEIVER_NAME = "receiver_name";// 收货人
        public const string NICK = "nick";// 买家nick
        public const string PHONE = "phone";// 手机号码
        public const string NORMAL = "normal";
        public const char NICK_SEPARATOR_CHAR = '~';
        public static string NICK_SEPARATOR = Convert.ToString(NICK_SEPARATOR_CHAR);
        public const char PHONE_SEPARATOR_CHAR = '$';
        public static string PHONE_SEPARATOR = Convert.ToString(PHONE_SEPARATOR_CHAR);
        public const char NORMAL_SEPARATOR_CHAR = (char)0x01;
        public static string NORMAL_SEPARATOR = Convert.ToString(NORMAL_SEPARATOR_CHAR);

        public const string UNDERLINE = "_";
    }
}
