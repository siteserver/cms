using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Aop.Api.Util
{
    public class StringUtil
    {
        public static string ToString(object obj)
        {
            if (obj == null)
            {
                return "null";
            }

            Type type = obj.GetType();
            if (string.Equals("System", type.Namespace))
            {
                return "\"" + obj.ToString() + "\"";
            }

            // class
            string result = "{";

            PropertyInfo[] pis = type.GetProperties();
            for (int i = 0; i < pis.Length; i++)
            {

                PropertyInfo pi = pis[i];
                Type pType = pi.PropertyType;

                MethodInfo getMethod = pi.GetGetMethod();
                object value = getMethod.Invoke(obj, null);
                if (value == null)
                {
                    continue;
                }

                string valueString = "";

                if (string.Equals("System", pType.Namespace))
                {
                    valueString = "\"" + value.ToString() + "\"";
                }
                else if (string.Equals("System.Collections.Generic", pType.Namespace))
                {
                    valueString = List2String(value);
                }
                else
                {
                    valueString = ToString(value);
                }

                if (i != 0)
                {
                    result += ",";
                }
                result += "\"" + pi.Name + "\":" + valueString + "";
            }
            result += "}";

            return result;
        }

        public static string List2String(object listObjects)
        {
            if (listObjects == null)
            {
                return "[]";
            }

            string result = "[";

            IList list = (IList)listObjects;
            for (int i = 0; i < list.Count; i++)
            {
                if (i != 0)
                {
                    result += ",";
                }
                result += ToString(list[i]);
            }
            result += "]";
            return result;
        }

    }
}
