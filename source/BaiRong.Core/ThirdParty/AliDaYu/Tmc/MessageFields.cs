
namespace Top.Tmc
{
    public abstract class MessageFields
    {
        public const string KIND = "__kind";

        // public final static String PULL_AMOUNT = "amount";

        public const string CONFIRM_ID = "id";
        public const string CONFIRM_ATTACH_QUEUE = "queue";

        public const string DATA_TOPIC = "topic";
        public const string DATA_CONTENT = "content";
        public const string DATA_PUBLISH_TIME = "time";

        public const string DATA_OUTGOING_PUBLISHER = "publisher";
        public const string DATA_OUTGOING_USER_NICK = "nick";
        public const string DATA_OUTGOING_USER_ID = "userid";
        
        public const string DATA_INCOMING_USER_SESSION = "session";

        public const string DATA_ATTACH_OUTGOING_TIME = "outtime";

        public const string OUTGOING_ID = "id";

        // ATTACH means server will attch the filed to message, not passed from client
        // OUTGOING means only outgoing message to client have the field
        // INCOMING means only incoming message from client have the field
    }
}
