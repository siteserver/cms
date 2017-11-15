using System;

using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Engines
{
	/**
	* A Noekeon engine, using direct-key mode.
	*/
	public class NoekeonEngine
		: IBlockCipher
	{
		private const int GenericSize = 16; // Block and key size, as well as the amount of rounds.

		private static readonly uint[] nullVector = 
		{
			0x00, 0x00, 0x00, 0x00 // Used in decryption
		};

		private static readonly uint[] roundConstants = 
		{
			0x80, 0x1b, 0x36, 0x6c,
			0xd8, 0xab, 0x4d, 0x9a,
			0x2f, 0x5e, 0xbc, 0x63,
			0xc6, 0x97, 0x35, 0x6a,
			0xd4
		};

		private uint[]	state = new uint[4], // a
						subKeys = new uint[4], // k
						decryptKeys = new uint[4];

		private bool _initialised, _forEncryption;

		/**
		* Create an instance of the Noekeon encryption algorithm
		* and set some defaults
		*/
		public NoekeonEngine()
		{
			_initialised = false;
		}

		public virtual string AlgorithmName
		{
			get { return "Noekeon"; }
		}

		public virtual bool IsPartialBlockOkay
		{
			get { return false; }
		}

        public virtual int GetBlockSize()
		{
			return GenericSize;
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
				throw new ArgumentException("Invalid parameters passed to Noekeon init - "
                    + Platform.GetTypeName(parameters), "parameters");

			_forEncryption = forEncryption;
			_initialised = true;

			KeyParameter p = (KeyParameter) parameters;

			setKey(p.GetKey());
		}

		public virtual int ProcessBlock(
			byte[]	input,
			int		inOff,
			byte[]	output,
			int		outOff)
		{
			if (!_initialised)
				throw new InvalidOperationException(AlgorithmName + " not initialised");

            Check.DataLength(input, inOff, GenericSize, "input buffer too short");
            Check.OutputLength(output, outOff, GenericSize, "output buffer too short");

            return _forEncryption
				?	encryptBlock(input, inOff, output, outOff)
				:	decryptBlock(input, inOff, output, outOff);
		}

		public virtual void Reset()
		{
			// TODO This should do something in case the encryption is aborted
		}

		/**
		* Re-key the cipher.
		*
		* @param  key  the key to be used
		*/
		private void setKey(byte[] key)
		{
			subKeys[0] = Pack.BE_To_UInt32(key, 0);
			subKeys[1] = Pack.BE_To_UInt32(key, 4);
			subKeys[2] = Pack.BE_To_UInt32(key, 8);
			subKeys[3] = Pack.BE_To_UInt32(key, 12);
		}

		private int encryptBlock(
			byte[]	input,
			int		inOff,
			byte[]	output,
			int		outOff)
		{
			state[0] = Pack.BE_To_UInt32(input, inOff);
			state[1] = Pack.BE_To_UInt32(input, inOff+4);
			state[2] = Pack.BE_To_UInt32(input, inOff+8);
			state[3] = Pack.BE_To_UInt32(input, inOff+12);

			int i;
			for (i = 0; i < GenericSize; i++)
			{
				state[0] ^= roundConstants[i];
				theta(state, subKeys);
				pi1(state);
				gamma(state);
				pi2(state);            
			}

			state[0] ^= roundConstants[i];
			theta(state, subKeys);

			Pack.UInt32_To_BE(state[0], output, outOff);
			Pack.UInt32_To_BE(state[1], output, outOff+4);
			Pack.UInt32_To_BE(state[2], output, outOff+8);
			Pack.UInt32_To_BE(state[3], output, outOff+12);

			return GenericSize;
		}

		private int decryptBlock(
			byte[]	input,
			int		inOff,
			byte[]	output,
			int		outOff)
		{
			state[0] = Pack.BE_To_UInt32(input, inOff);
			state[1] = Pack.BE_To_UInt32(input, inOff+4);
			state[2] = Pack.BE_To_UInt32(input, inOff+8);
			state[3] = Pack.BE_To_UInt32(input, inOff+12);

			Array.Copy(subKeys, 0, decryptKeys, 0, subKeys.Length);
			theta(decryptKeys, nullVector);

			int i;
			for (i = GenericSize; i > 0; i--)
			{
				theta(state, decryptKeys);
				state[0] ^= roundConstants[i];
				pi1(state);
				gamma(state);
				pi2(state);
			}

			theta(state, decryptKeys);
			state[0] ^= roundConstants[i];

			Pack.UInt32_To_BE(state[0], output, outOff);
			Pack.UInt32_To_BE(state[1], output, outOff+4);
			Pack.UInt32_To_BE(state[2], output, outOff+8);
			Pack.UInt32_To_BE(state[3], output, outOff+12);

			return GenericSize;
		}

		private void gamma(uint[] a)
		{
			a[1] ^= ~a[3] & ~a[2];
			a[0] ^= a[2] & a[1];

			uint tmp = a[3];
			a[3]  = a[0];
			a[0]  = tmp;
			a[2] ^= a[0]^a[1]^a[3];

			a[1] ^= ~a[3] & ~a[2];
			a[0] ^= a[2] & a[1];
		}

		private void theta(uint[] a, uint[] k)
		{
			uint tmp;
			tmp   = a[0]^a[2]; 
			tmp  ^= rotl(tmp,8)^rotl(tmp,24); 
			a[1] ^= tmp; 
			a[3] ^= tmp; 

			for (int i = 0; i < 4; i++)
			{
				a[i] ^= k[i];
			}

			tmp   = a[1]^a[3]; 
			tmp  ^= rotl(tmp,8)^rotl(tmp,24); 
			a[0] ^= tmp; 
			a[2] ^= tmp;
		}

		private void pi1(uint[] a)
		{
			a[1] = rotl(a[1], 1);
			a[2] = rotl(a[2], 5);
			a[3] = rotl(a[3], 2);
		}

		private void pi2(uint[] a)
		{
			a[1] = rotl(a[1], 31);
			a[2] = rotl(a[2], 27);
			a[3] = rotl(a[3], 30);
		}

		// Helpers

		private uint rotl(uint x, int y)
		{
			return (x << y) | (x >> (32-y));
		}
	}
}
