using System;
using System.Collections;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
    /**
     * The extendedKeyUsage object.
     * <pre>
     *      extendedKeyUsage ::= Sequence SIZE (1..MAX) OF KeyPurposeId
     * </pre>
     */
    public class ExtendedKeyUsage
        : Asn1Encodable
    {
        internal readonly IDictionary usageTable = Platform.CreateHashtable();
        internal readonly Asn1Sequence seq;

        public static ExtendedKeyUsage GetInstance(
            Asn1TaggedObject	obj,
            bool				explicitly)
        {
            return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
        }

        public static ExtendedKeyUsage GetInstance(
            object obj)
        {
            if (obj is ExtendedKeyUsage)
            {
                return (ExtendedKeyUsage) obj;
            }

            if (obj is Asn1Sequence)
            {
                return new ExtendedKeyUsage((Asn1Sequence) obj);
            }

            if (obj is X509Extension)
            {
                return GetInstance(X509Extension.ConvertValueToObject((X509Extension) obj));
            }

            throw new ArgumentException("Invalid ExtendedKeyUsage: " + Platform.GetTypeName(obj));
        }

        private ExtendedKeyUsage(
            Asn1Sequence seq)
        {
            this.seq = seq;

            foreach (object o in seq)
            {
                if (!(o is DerObjectIdentifier))
                    throw new ArgumentException("Only DerObjectIdentifier instances allowed in ExtendedKeyUsage.");

                this.usageTable[o] = o;
            }
        }

        public ExtendedKeyUsage(
            params KeyPurposeID[] usages)
        {
            this.seq = new DerSequence(usages);

            foreach (KeyPurposeID usage in usages)
            {
                this.usageTable[usage] = usage;
            }
        }

#if !(SILVERLIGHT || PORTABLE)
        [Obsolete]
        public ExtendedKeyUsage(
            ArrayList usages)
            : this((IEnumerable)usages)
        {
        }
#endif

        public ExtendedKeyUsage(
            IEnumerable usages)
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

            foreach (object usage in usages)
            {
                Asn1Encodable o = KeyPurposeID.GetInstance(usage);

                v.Add(o);
                this.usageTable[o] = o;
            }

            this.seq = new DerSequence(v);
        }

        public bool HasKeyPurposeId(
            KeyPurposeID keyPurposeId)
        {
            return usageTable.Contains(keyPurposeId);
        }

#if !(SILVERLIGHT || PORTABLE)
        [Obsolete("Use 'GetAllUsages'")]
        public ArrayList GetUsages()
        {
            return new ArrayList(usageTable.Values);
        }
#endif

        /**
         * Returns all extended key usages.
         * The returned ArrayList contains DerObjectIdentifier instances.
         * @return An ArrayList with all key purposes.
         */
        public IList GetAllUsages()
        {
            return Platform.CreateArrayList(usageTable.Values);
        }

        public int Count
        {
            get { return usageTable.Count; }
        }

        public override Asn1Object ToAsn1Object()
        {
            return seq;
        }
    }
}
