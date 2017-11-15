using System;
using System.Text;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Asn1.X509
{
	public class GeneralNames
		: Asn1Encodable
	{
		private readonly GeneralName[] names;

		public static GeneralNames GetInstance(
			object obj)
		{
			if (obj == null || obj is GeneralNames)
			{
				return (GeneralNames) obj;
			}

			if (obj is Asn1Sequence)
			{
				return new GeneralNames((Asn1Sequence) obj);
			}

            throw new ArgumentException("unknown object in factory: " + Platform.GetTypeName(obj), "obj");
		}

		public static GeneralNames GetInstance(
			Asn1TaggedObject	obj,
			bool				explicitly)
		{
			return GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		/// <summary>Construct a GeneralNames object containing one GeneralName.</summary>
		/// <param name="name">The name to be contained.</param>
		public GeneralNames(
			GeneralName name)
		{
			names = new GeneralName[]{ name };
		}

        public GeneralNames(
            GeneralName[] names)
        {
            this.names = (GeneralName[])names.Clone();
        }

		private GeneralNames(
			Asn1Sequence seq)
		{
			this.names = new GeneralName[seq.Count];

			for (int i = 0; i != seq.Count; i++)
			{
				names[i] = GeneralName.GetInstance(seq[i]);
			}
		}

		public GeneralName[] GetNames()
		{
			return (GeneralName[]) names.Clone();
		}

		/**
		 * Produce an object suitable for an Asn1OutputStream.
		 * <pre>
		 * GeneralNames ::= Sequence SIZE {1..MAX} OF GeneralName
		 * </pre>
		 */
		public override Asn1Object ToAsn1Object()
		{
			return new DerSequence(names);
		}

		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			string sep = Platform.NewLine;

			buf.Append("GeneralNames:");
			buf.Append(sep);

			foreach (GeneralName name in names)
			{
				buf.Append("    ");
				buf.Append(name);
				buf.Append(sep);
			}

			return buf.ToString();
		}
	}
}
