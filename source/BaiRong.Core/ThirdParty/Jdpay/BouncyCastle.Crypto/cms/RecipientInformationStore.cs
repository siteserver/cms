using System;
using System.Collections;

using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Cms
{
	public class RecipientInformationStore
	{
		private readonly IList all; //ArrayList[RecipientInformation]
		private readonly IDictionary table = Platform.CreateHashtable(); // Hashtable[RecipientID, ArrayList[RecipientInformation]]

		public RecipientInformationStore(
			ICollection recipientInfos)
		{
			foreach (RecipientInformation recipientInformation in recipientInfos)
			{
				RecipientID rid = recipientInformation.RecipientID;
                IList list = (IList)table[rid];

				if (list == null)
				{
					table[rid] = list = Platform.CreateArrayList(1);
				}

				list.Add(recipientInformation);
			}

            this.all = Platform.CreateArrayList(recipientInfos);
		}

		public RecipientInformation this[RecipientID selector]
		{
			get { return GetFirstRecipient(selector); }
		}

		/**
		* Return the first RecipientInformation object that matches the
		* passed in selector. Null if there are no matches.
		*
		* @param selector to identify a recipient
		* @return a single RecipientInformation object. Null if none matches.
		*/
		public RecipientInformation GetFirstRecipient(
			RecipientID selector)
		{
			IList list = (IList) table[selector];

			return list == null ? null : (RecipientInformation) list[0];
		}

		/**
		* Return the number of recipients in the collection.
		*
		* @return number of recipients identified.
		*/
		public int Count
		{
			get { return all.Count; }
		}

		/**
		* Return all recipients in the collection
		*
		* @return a collection of recipients.
		*/
		public ICollection GetRecipients()
		{
			return Platform.CreateArrayList(all);
		}

		/**
		* Return possible empty collection with recipients matching the passed in RecipientID
		*
		* @param selector a recipient id to select against.
		* @return a collection of RecipientInformation objects.
		*/
		public ICollection GetRecipients(
			RecipientID selector)
		{
            IList list = (IList)table[selector];

            return list == null ? Platform.CreateArrayList() : Platform.CreateArrayList(list);
		}
	}
}
