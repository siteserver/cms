using System;

namespace Org.BouncyCastle.Crypto.Parameters
{
    [Obsolete("Use AeadParameters")]
    public class CcmParameters
        : AeadParameters 
    {
		/**
		 * Base constructor.
		 * 
		 * @param key key to be used by underlying cipher
		 * @param macSize macSize in bits
		 * @param nonce nonce to be used
		 * @param associatedText associated text, if any
		 */
		public CcmParameters(
			KeyParameter	key,
			int				macSize,
			byte[]			nonce,
			byte[]			associatedText)
			: base(key, macSize, nonce, associatedText)
		{
		}
	}
}
