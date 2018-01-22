using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* An TEA engine.
	*/
	public class TeaEngine
		: IBlockCipher
	{
		private const int
			rounds		= 32,
			block_size	= 8;
//			key_size	= 16,

		private const uint 
			delta		= 0x9E3779B9,
			d_sum		= 0xC6EF3720; // sum on decrypt

		/*
		* the expanded key array of 4 subkeys
		*/
		private uint _a, _b, _c, _d;
		private bool _initialised;
		private bool _forEncryption;

		/**
		* Create an instance of the TEA encryption algorithm
		* and set some defaults
		*/
		public TeaEngine()
		{
			_initialised = false;
		}

        public virtual string AlgorithmName
		{
			get { return "TEA"; }
		}

        public virtual bool IsPartialBlockOkay
		{
			get { return false; }
		}

        public virtual int GetBlockSize()
		{
			return block_size;
		}

		/**
		* initialise
		*
		* @param forEncryption whether or not we are for encryption.
		* @param params the parameters required to set up the cipher.
		* @exception ArgumentException if the params argument is
		* inappropriate.
		*/
        public virtual void Init(
			bool				forEncryption,
			ICipherParameters	parameters)
		{
			if (!(parameters is KeyParameter))
			{
				throw new ArgumentException("invalid parameter passed to TEA init - "
					+ Platform.GetTypeName(parameters));
			}

			_forEncryption = forEncryption;
			_initialised = true;

			KeyParameter p = (KeyParameter) parameters;

			setKey(p.GetKey());
		}

        public virtual int ProcessBlock(
			byte[]  inBytes,
			int     inOff,
			byte[]  outBytes,
			int     outOff)
		{
			if (!_initialised)
				throw new InvalidOperationException(AlgorithmName + " not initialised");

            Check.DataLength(inBytes, inOff, block_size, "input buffer too short");
            Check.OutputLength(outBytes, outOff, block_size, "output buffer too short");

            return _forEncryption
				?	encryptBlock(inBytes, inOff, outBytes, outOff)
				:	decryptBlock(inBytes, inOff, outBytes, outOff);
		}

        public virtual void Reset()
		{
		}

		/**
		* Re-key the cipher.
		*
		* @param  key  the key to be used
		*/
		private void setKey(
			byte[] key)
		{
			_a = Pack.BE_To_UInt32(key, 0);
			_b = Pack.BE_To_UInt32(key, 4);
			_c = Pack.BE_To_UInt32(key, 8);
			_d = Pack.BE_To_UInt32(key, 12);
		}

		private int encryptBlock(
			byte[]	inBytes,
			int		inOff,
			byte[]	outBytes,
			int		outOff)
		{
			// Pack bytes into integers
			uint v0 = Pack.BE_To_UInt32(inBytes, inOff);
			uint v1 = Pack.BE_To_UInt32(inBytes, inOff + 4);
	        
			uint sum = 0;
	        
			for (int i = 0; i != rounds; i++)
			{
				sum += delta;
				v0  += ((v1 << 4) + _a) ^ (v1 + sum) ^ ((v1 >> 5) + _b);
				v1  += ((v0 << 4) + _c) ^ (v0 + sum) ^ ((v0 >> 5) + _d);
			}

			Pack.UInt32_To_BE(v0, outBytes, outOff);
			Pack.UInt32_To_BE(v1, outBytes, outOff + 4);

			return block_size;
		}

		private int decryptBlock(
			byte[]	inBytes,
			int		inOff,
			byte[]	outBytes,
			int		outOff)
		{
			// Pack bytes into integers
			uint v0 = Pack.BE_To_UInt32(inBytes, inOff);
			uint v1 = Pack.BE_To_UInt32(inBytes, inOff + 4);

			uint sum = d_sum;

			for (int i = 0; i != rounds; i++)
			{
				v1  -= ((v0 << 4) + _c) ^ (v0 + sum) ^ ((v0 >> 5) + _d);
				v0  -= ((v1 << 4) + _a) ^ (v1 + sum) ^ ((v1 >> 5) + _b);
				sum -= delta;
			}

			Pack.UInt32_To_BE(v0, outBytes, outOff);
			Pack.UInt32_To_BE(v1, outBytes, outOff + 4);

			return block_size;
		}
	}
}
