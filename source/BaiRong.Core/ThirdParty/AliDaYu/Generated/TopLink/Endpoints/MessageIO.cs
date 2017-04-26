using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Taobao.Top.Link.Endpoints
{
    //simple protocol impl, match protocolVersion=2
    public class MessageIO
    {
        public static Message ReadMessage(Stream input)
        {
            var buffer = new BinaryReader(input);
            Message msg = new Message();
            msg.ProtocolVersion = buffer.ReadByte();
            msg.MessageType = buffer.ReadByte();
            // read kv
            IDictionary<string, object> dict = new Dictionary<string, object>();
            short headerType = buffer.ReadInt16();
            while (headerType != MessageType.HeaderType.EndOfHeaders)
            {
                if (headerType == MessageType.HeaderType.Custom)
                    dict.Add(ReadCountedString(buffer), ReadCustomValue(buffer));
                else if (headerType == MessageType.HeaderType.StatusCode)
                    msg.StatusCode = buffer.ReadInt32();
                else if (headerType == MessageType.HeaderType.StatusPhrase)
                    msg.StatusPhase = ReadCountedString(buffer);
                else if (headerType == MessageType.HeaderType.Flag)
                    msg.Flag = buffer.ReadInt32();
                else if (headerType == MessageType.HeaderType.Token)
                    msg.Token = ReadCountedString(buffer);
                headerType = buffer.ReadInt16();
            }
            msg.Content = dict;
            return msg;
        }
        public static void WriteMessage(Stream input, Message message)
        {
            var buffer = new BinaryWriter(input);
            buffer.Write((byte)message.ProtocolVersion);
            buffer.Write((byte)message.MessageType);

            if (message.StatusCode > 0)
            {
                buffer.Write(MessageType.HeaderType.StatusCode);
                buffer.Write(message.StatusCode);
            }
            if (message.StatusPhase != null && message.StatusPhase != "")
            {
                buffer.Write(MessageType.HeaderType.StatusPhrase);
                WriteCountedString(buffer, message.StatusPhase);
            }
            if (message.Flag > 0)
            {
                buffer.Write(MessageType.HeaderType.Flag);
                buffer.Write(message.Flag);
            }
            if (message.Token != null && message.Token != "")
            {
                buffer.Write(MessageType.HeaderType.Token);
                WriteCountedString(buffer, message.Token);
            }
            if (message.Content != null)
            {
                foreach (var i in message.Content)
                    WriteCustomHeader(buffer, i.Key, i.Value);
            }
            buffer.Write(MessageType.HeaderType.EndOfHeaders);
        }
        // UTF-8 only
        private static string ReadCountedString(BinaryReader buffer)
        {
            int size = buffer.ReadInt32();
            return size > 0 ? Encoding.UTF8.GetString(buffer.ReadBytes(size)) : null;
        }
        private static void WriteCountedString(BinaryWriter buffer, string value)
        {
            int strLength = 0;
            if (value != null)
                strLength = value.Length;

            if (strLength > 0)
            {
                byte[] strBytes = Encoding.UTF8.GetBytes(value);
                buffer.Write(strBytes.Length);
                buffer.Write(strBytes);
            }
            else
                buffer.Write(0);
        }
        private static void WriteCustomHeader(BinaryWriter buffer, string name, object value)
        {
            buffer.Write(MessageType.HeaderType.Custom);
            WriteCountedString(buffer, name);
            WriteCustomValue(buffer, value);
        }
        private static object ReadCustomValue(BinaryReader buffer)
        {
            byte format = buffer.ReadByte();
            switch (format)
            {
                case MessageType.ValueFormat.Void:
                    return null;
                case MessageType.ValueFormat.Byte:
                    return buffer.ReadByte();
                case MessageType.ValueFormat.Int16:
                    return buffer.ReadInt16();
                case MessageType.ValueFormat.Int32:
                    return buffer.ReadInt32();
                case MessageType.ValueFormat.Int64:
                    return buffer.ReadInt64();
                case MessageType.ValueFormat.Date:
                    return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1).AddMilliseconds(buffer.ReadInt64()));
                case MessageType.ValueFormat.ByteArray:
                    return buffer.ReadBytes(buffer.ReadInt32());
                default:
                    return ReadCountedString(buffer);
            }
        }
        private static void WriteCustomValue(BinaryWriter buffer, object value)
        {
            if (value == null)
            {
                buffer.Write(MessageType.ValueFormat.Void);
                return;
            }
            Type type = value.GetType();
            if (typeof(byte).Equals(type) || typeof(Byte).Equals(type))
            {
                buffer.Write(MessageType.ValueFormat.Byte);
                buffer.Write((byte)value);
            }
            else if (typeof(short).Equals(type))
            {
                buffer.Write(MessageType.ValueFormat.Int16);
                buffer.Write((short)value);
            }
            else if (typeof(int).Equals(type) || typeof(Int32).Equals(type))
            {
                buffer.Write(MessageType.ValueFormat.Int32);
                buffer.Write((int)value);
            }
            else if (typeof(long).Equals(type) || typeof(Int64).Equals(type))
            {
                buffer.Write(MessageType.ValueFormat.Int64);
                buffer.Write((long)value);
            }
            else if (typeof(DateTime).Equals(type))
            {
                buffer.Write(MessageType.ValueFormat.Date);
                buffer.Write(((DateTime)value).Ticks);
            }
            else if (typeof(byte[]).Equals(type))
            {
                buffer.Write(MessageType.ValueFormat.ByteArray);
                var data = value as byte[];
                buffer.Write(data.Length);
                buffer.Write(data);
            }
            else
            {
                buffer.Write(MessageType.ValueFormat.CountedString);
                WriteCountedString(buffer, (string)value);
            }
        }
    }
}