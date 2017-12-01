using System;

namespace Org.BouncyCastle.Tsp
{
	/**
	 * Exception thrown if a TSP request or response fails to validate.
	 * <p>
	 * If a failure code is associated with the exception it can be retrieved using
	 * the getFailureCode() method.</p>
	 */
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    public class TspValidationException
		: TspException
	{
		private int failureCode;

		public TspValidationException(
			string message)
			: base(message)
		{
			this.failureCode = -1;
		}

		public TspValidationException(
			string	message,
			int		failureCode)
			: base(message)
		{
			this.failureCode = failureCode;
		}

		/**
		 * Return the failure code associated with this exception - if one is set.
		 *
		 * @return the failure code if set, -1 otherwise.
		 */
		public int FailureCode
		{
			get { return failureCode; }
		}
	}
}
