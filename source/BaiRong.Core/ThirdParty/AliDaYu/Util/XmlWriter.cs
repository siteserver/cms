using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Top.Api.Util
{
    public class XmlWriter
    {
        private StringBuilder buf = new StringBuilder();
        private Stack<object> calls = new Stack<object>();
        private string rootTagName;
        private Type stopType;
        private IDictionary<string, object> stopProps;

        public XmlWriter(string rootTagName, Type stopType)
        {
            this.rootTagName = rootTagName;
            this.stopType = stopType;
            if (stopType != null)
            {
                this.stopProps = GetStopProps(stopType);
            }
        }

        public XmlWriter()
            : this(null, null)
        {
        }

        public string Write(object obj)
        {
            buf.Length = 0;
            string tagName = rootTagName;
            if (tagName == null)
            {
                tagName = StringUtil.ToCamelStyle(obj.GetType().Name);
            }
            AddPair(tagName, obj);
            return buf.ToString();
        }

        private void Value(object obj)
        {
            if (obj == null || Cyclic(obj))
            {
                AddObject(null);
            }
            else
            {
                calls.Push(obj);
                Type objType = obj.GetType();
                if (typeof(IDictionary).IsAssignableFrom(objType))
                {
                    AddDictionary(obj as IDictionary);
                }
                else if (typeof(ICollection).IsAssignableFrom(objType))
                {
                    AddCollection(obj as ICollection);
                }
                else if (objType.IsArray)
                {
                    AddArray(obj);
                }
                else if (typeof(DateTime) == objType)
                {
                    AddDateTime((DateTime)obj);
                }
                else if (typeof(bool) == objType)
                {
                    AddBool((bool)obj);
                }
                else if (typeof(string) == objType || typeof(Type) == objType)
                {
                    AddString(obj);
                }
                else if (objType.IsPrimitive)
                {
                    AddString(obj);
                }
                else
                {
                    AddBean(obj);
                }
                calls.Pop();
            }
        }

        private bool Cyclic(object obj)
        {
            Stack<object>.Enumerator em = calls.GetEnumerator();
            while (em.MoveNext())
            {
                object called = em.Current;
                if (obj == called) return true;
            }
            return false;
        }

        private void AddBean(object obj)
        {
            bool isChildren = stopType != null && stopType.IsAssignableFrom(obj.GetType());
            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                if (isChildren && stopProps.ContainsKey(name))
                {
                    continue;
                }
                object value = prop.GetValue(obj, new object[] { });
                if (value != null)
                {
                    string newName = StringUtil.ToCamelStyle(name);
                    AddPair(newName, value);
                }
            }
        }

        private void AddDictionary(IDictionary obj)
        {
            foreach (DictionaryEntry entry in obj)
            {
                string strKey = entry.Key.ToString();
                AddPair(strKey, entry.Value);
            }
        }

        private void AddCollection(ICollection obj)
        {
            string tagName = null;
            foreach (object item in obj)
            {
                if (tagName == null)
                {
                    tagName = StringUtil.ToCamelStyle(obj.GetType().Name);
                }
                AddPair(tagName, item);
            }
        }

        private void AddArray(object obj)
        {
            string tagName = null;
            Array.ForEach(obj as object[], item =>
            {
                if (tagName == null)
                {
                    tagName = StringUtil.ToCamelStyle(obj.GetType().Name);
                }
                AddPair(tagName, item);
            });
        }

        private void AddBool(bool value)
        {
            AddObject(value ? "true" : "false");
        }

        private void AddDateTime(DateTime value)
        {
            AddObject(StringUtil.FormatDateTime(value));
        }

        private void AddString(object obj)
        {
            AddObject(StringUtil.EscapeXml(obj.ToString()));
        }

        private void AddObject(object obj)
        {
            buf.Append(obj);
        }

        private void AddPair(string name, object value)
        {
            StartTag(name);
            Value(value);
            EndTag(name);
        }

        private void StartTag(string tagName)
        {
            buf.Append("<");
            buf.Append(tagName);
            buf.Append(">");
        }

        private void EndTag(string tagName)
        {
            buf.Append("</");
            buf.Append(tagName);
            buf.Append(">");
        }

        private IDictionary<string, object> GetStopProps(Type type)
        {
            IDictionary<string, object> stopProps = new Dictionary<string, object>();
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (PropertyInfo prop in props)
            {
                stopProps.Add(prop.Name, null);
            }
            return stopProps;
        }
    }
}
