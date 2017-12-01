using System;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Pkix
{
	/**
	 * An exception indicating one of a variety of problems encountered when 
	 * validating a certification path. <br />
	 * <br />
	 * A <code>CertPathValidatorException</code> provides support for wrapping
	 * exceptions. The {@link #getCause getCause} method returns the throwable, 
	 * if any, that caused this exception to be thrown. <br />
	 * <br />
	 * A <code>CertPathValidatorException</code> may also include the 
	 * certification path that was being validated when the exception was thrown 
	 * and the index of the certificate in the certification path that caused the 
	 * exception to be thrown. Use the {@link #getCertPath getCertPath} and
	 * {@link #getIndex getIndex} methods to retrieve this information.<br />
	 * <br />
	 * <b>Concurrent Access</b><br />
	 * <br />
	 * Unless otherwise specified, the methods defined in this class are not
	 * thread-safe. Multiple threads that need to access a single
	 * object concurrently should synchronize amongst themselves and
	 * provide the necessary locking. Multiple threads each manipulating
	 * separate objects need not synchronize.
	 *
	 * @see CertPathValidator
	 **/
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE)
    [Serializable]
#endif
    public class PkixCertPathValidatorException
        : GeneralSecurityException
	{
		private Exception cause;
		private PkixCertPath certPath;
		private int index = -1;

		public PkixCertPathValidatorException() : base() { }

		/// <summary>
		/// Creates a <code>PkixCertPathValidatorException</code> with the given detail
		/// message. A detail message is a <code>String</code> that describes this
		/// particular exception. 
		/// </summary>
		/// <param name="message">the detail message</param>
		public PkixCertPathValidatorException(string message) : base(message) { }

		/// <summary>
		/// Creates a <code>PkixCertPathValidatorException</code> with the specified
		/// detail message and cause.
		/// </summary>
		/// <param name="message">the detail message</param>
		/// <param name="cause">the cause (which is saved for later retrieval by the
		/// {@link #getCause getCause()} method). (A <code>null</code>
		/// value is permitted, and indicates that the cause is
		/// nonexistent or unknown.)</param>
		public PkixCertPathValidatorException(string message, Exception cause) : base(message)
		{
			this.cause = cause;
		}

		/// <summary>
		/// Creates a <code>PkixCertPathValidatorException</code> with the specified
		/// detail message, cause, certification path, and index.
		/// </summary>
		/// <param name="message">the detail message (or <code>null</code> if none)</param>
		/// <param name="cause">the cause (or <code>null</code> if none)</param>
		/// <param name="certPath">the certification path that was in the process of being
		/// validated when the error was encountered</param>
		/// <param name="index">the index of the certificate in the certification path that</param>																																																																																   * 
		public PkixCertPathValidatorException(
			string			message,
			Exception		cause,
			PkixCertPath	certPath,
			int				index)
			: base(message)
		{
			if (certPath == null && index != -1)
			{
				throw new ArgumentNullException(
					"certPath = null and index != -1");
			}
			if (index < -1
				|| (certPath != null && index >= certPath.Certificates.Count))
			{
				throw new IndexOutOfRangeException(
					" index < -1 or out of bound of certPath.getCertificates()");
			}

			this.cause = cause;
			this.certPath = certPath;
			this.index = index;
		}

		//
		// Prints a stack trace to a <code>PrintWriter</code>, including the
		// backtrace of the cause, if any.
		// 
		// @param pw
		//            the <code>PrintWriter</code> to use for output
		//
		//		public void printStackTrace(PrintWriter pw)
		//		{
		//			super.printStackTrace(pw);
		//			if (getCause() != null)
		//			{
		//				getCause().printStackTrace(pw);
		//			}
		//	}
		//}


		//	/**
		//	 * Creates a <code>CertPathValidatorException</code> that wraps the
		//	 * specified throwable. This allows any exception to be converted into a
		//	 * <code>CertPathValidatorException</code>, while retaining information
		//	 * about the wrapped exception, which may be useful for debugging. The
		//	 * detail message is set to (<code>cause==null ? null : cause.toString()
		//	 * </code>)
		//	 * (which typically contains the class and detail message of cause).
		//	 * 
		//	 * @param cause
		//	 *            the cause (which is saved for later retrieval by the
		//	 *            {@link #getCause getCause()} method). (A <code>null</code>
		//	 *            value is permitted, and indicates that the cause is
		//	 *            nonexistent or unknown.)
		//	 */
		//	public PkixCertPathValidatorException(Throwable cause)
		//	{
		//		this.cause = cause;
		//	}
		//

		/// <summary>
		/// Returns the detail message for this <code>CertPathValidatorException</code>.
		/// </summary>
		/// <returns>the detail message, or <code>null</code> if neither the message nor cause were specified</returns>
		public override string Message
		{
			get
			{
				string message = base.Message;

				if (message != null)
				{
					return message;
				}

				if (cause != null)
				{
					return cause.Message;
				}

				return null;
			}
		}

	/**
	 * Returns the certification path that was being validated when the
	 * exception was thrown.
	 * 
	 * @return the <code>CertPath</code> that was being validated when the
	 *         exception was thrown (or <code>null</code> if not specified)
	 */
		public PkixCertPath CertPath
		{
			get { return certPath; }
		}

	/**
	 * Returns the index of the certificate in the certification path that
	 * caused the exception to be thrown. Note that the list of certificates in
	 * a <code>CertPath</code> is zero based. If no index has been set, -1 is
	 * returned.
	 * 
	 * @return the index that has been set, or -1 if none has been set
	 */
	public int Index
	{
        get { return index; }
	}

//	/**
//	 * Returns the cause of this <code>CertPathValidatorException</code> or
//	 * <code>null</code> if the cause is nonexistent or unknown.
//	 * 
//	 * @return the cause of this throwable or <code>null</code> if the cause
//	 *         is nonexistent or unknown.
//	 */
//	public Throwable getCause()
//	{
//		return cause;
//	}
//
//	/**
//	 * Returns a string describing this exception, including a description of
//	 * the internal (wrapped) cause if there is one.
//	 * 
//	 * @return a string representation of this
//	 *         <code>CertPathValidatorException</code>
//	 */
//	public String toString()
//	{
//		StringBuffer sb = new StringBuffer();
//		String s = getMessage();
//		if (s != null)
//		{
//			sb.append(s);
//		}
//		if (getIndex() >= 0)
//		{
//			sb.append("index in certpath: ").append(getIndex()).append('\n');
//			sb.append(getCertPath());
//		}
//		return sb.toString();
//	}

	}
}
