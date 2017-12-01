#region Header
/**
 * JsonException.cs
 *   Base class throwed by LitJSON when a parsing error occurs.
 *
 * The authors disclaim copyright to this source code. For more details, see
 * the COPYING file included with this distribution.
 **/
#endregion


using System;


namespace BaiRong.Text.LitJson
{
    public class JsonException : ApplicationException
    {
        public JsonException () : base ()
        {
        }

        internal JsonException (ParserToken token) :
            base ($"Invalid token '{token}' in input string")
        {
        }

        internal JsonException (ParserToken token,
                                Exception inner_exception) :
            base ($"Invalid token '{token}' in input string",
                inner_exception)
        {
        }

        internal JsonException (int c) :
            base ($"Invalid character '{(char) c}' in input string")
        {
        }

        internal JsonException (int c, Exception inner_exception) :
            base ($"Invalid character '{(char) c}' in input string",
                inner_exception)
        {
        }


        public JsonException (string message) : base (message)
        {
        }

        public JsonException (string message, Exception inner_exception) :
            base (message, inner_exception)
        {
        }
    }
}
