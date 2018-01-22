using System;
using System.Threading;

using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Utilities;

namespace Org.BouncyCastle.Crypto.Generators
{
	public class SCrypt
	{
		// TODO Validate arguments
		public static byte[] Generate(byte[] P, byte[] S, int N, int r, int p, int dkLen)
		{
			return MFcrypt(P, S, N, r, p, dkLen);
		}

		private static byte[] MFcrypt(byte[] P, byte[] S, int N, int r, int p, int dkLen)
		{
			int MFLenBytes = r * 128;
			byte[] bytes = SingleIterationPBKDF2(P, S, p * MFLenBytes);

			uint[] B = null;

			try
			{
				int BLen = bytes.Length >> 2;
				B = new uint[BLen];

				Pack.LE_To_UInt32(bytes, 0, B);

				int MFLenWords = MFLenBytes >> 2;
				for (int BOff = 0; BOff < BLen; BOff += MFLenWords)
				{
					// TODO These can be done in parallel threads
					SMix(B, BOff, N, r);
				}

				Pack.UInt32_To_LE(B, bytes, 0);

				return SingleIterationPBKDF2(P, bytes, dkLen);
			}
			finally
			{
				ClearAll(bytes, B);
			}
		}

		private static byte[] SingleIterationPBKDF2(byte[] P, byte[] S, int dkLen)
		{
			PbeParametersGenerator pGen = new Pkcs5S2ParametersGenerator(new Sha256Digest());
			pGen.Init(P, S, 1);
			KeyParameter key = (KeyParameter)pGen.GenerateDerivedMacParameters(dkLen * 8);
			return key.GetKey();
		}

		private static void SMix(uint[] B, int BOff, int N, int r)
		{
			int BCount = r * 32;

			uint[] blockX1 = new uint[16];
			uint[] blockX2 = new uint[16];
			uint[] blockY = new uint[BCount];

			uint[] X = new uint[BCount];
			uint[][] V = new uint[N][];

			try
			{
				Array.Copy(B, BOff, X, 0, BCount);

				for (int i = 0; i < N; ++i)
				{
					V[i] = (uint[])X.Clone();
					BlockMix(X, blockX1, blockX2, blockY, r);
				}

				uint mask = (uint)N - 1;
				for (int i = 0; i < N; ++i)
				{
					uint j = X[BCount - 16] & mask;
					Xor(X, V[j], 0, X);
					BlockMix(X, blockX1, blockX2, blockY, r);
				}

				Array.Copy(X, 0, B, BOff, BCount);
			}
			finally
			{
				ClearAll(V);
				ClearAll(X, blockX1, blockX2, blockY);
			}
		}

		private static void BlockMix(uint[] B, uint[] X1, uint[] X2, uint[] Y, int r)
		{
			Array.Copy(B, B.Length - 16, X1, 0, 16);

			int BOff = 0, YOff = 0, halfLen = B.Length >> 1;

			for (int i = 2 * r; i > 0; --i)
			{
				Xor(X1, B, BOff, X2);

				Salsa20Engine.SalsaCore(8, X2, X1);
				Array.Copy(X1, 0, Y, YOff, 16);

				YOff = halfLen + BOff - YOff;
				BOff += 16;
			}

			Array.Copy(Y, 0, B, 0, Y.Length);
		}

		private static void Xor(uint[] a, uint[] b, int bOff, uint[] output)
		{
			for (int i = output.Length - 1; i >= 0; --i)
			{
				output[i] = a[i] ^ b[bOff + i];
			}
		}

		private static void Clear(Array array)
		{
			if (array != null)
			{
				Array.Clear(array, 0, array.Length);
			}
		}

		private static void ClearAll(params Array[] arrays)
		{
			foreach (Array array in arrays)
			{
				Clear(array);
			}
		}
	}
}
