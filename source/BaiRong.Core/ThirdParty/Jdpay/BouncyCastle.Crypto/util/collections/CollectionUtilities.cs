using System;
using System.Collections;
using System.Text;

namespace Org.BouncyCastle.Utilities.Collections
{
    public abstract class CollectionUtilities
    {
        public static void AddRange(IList to, IEnumerable range)
        {
            foreach (object o in range)
            {
                to.Add(o);
            }
        }

        public static bool CheckElementsAreOfType(IEnumerable e, Type t)
        {
            foreach (object o in e)
            {
                if (!t.IsInstanceOfType(o))
                    return false;
            }
            return true;
        }

        public static IDictionary ReadOnly(IDictionary d)
        {
            return new UnmodifiableDictionaryProxy(d);
        }

        public static IList ReadOnly(IList l)
        {
            return new UnmodifiableListProxy(l);
        }

        public static ISet ReadOnly(ISet s)
        {
            return new UnmodifiableSetProxy(s);
        }

        public static string ToString(IEnumerable c)
        {
            StringBuilder sb = new StringBuilder("[");

            IEnumerator e = c.GetEnumerator();

            if (e.MoveNext())
            {
                sb.Append(e.Current.ToString());

                while (e.MoveNext())
                {
                    sb.Append(", ");
                    sb.Append(e.Current.ToString());
                }
            }

            sb.Append(']');

            return sb.ToString();
        }
    }
}
