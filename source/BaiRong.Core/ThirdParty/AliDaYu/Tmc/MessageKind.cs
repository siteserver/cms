using System;

namespace Top.Tmc
{
    //message process by kind, 0-255
    public abstract class MessageKind
    {
        public const Byte None = 0;
        public const Byte PullRequest = 1;
        public const Byte Confirm = 2;
        public const Byte Data = 3;
    }
}
