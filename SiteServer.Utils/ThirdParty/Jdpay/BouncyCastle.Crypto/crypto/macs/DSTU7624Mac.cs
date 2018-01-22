using System;

using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;


namespace Org.BouncyCastle.Crypto.Macs
{
     /**
     * implementation of DSTU 7624 MAC
     */
     public class Dstu7624Mac : IMac
     {
          private int macSize;
                    
          private Dstu7624Engine engine;
          private int blockSize;

          private byte[] c, cTemp, kDelta;
          private byte[] buf;
          private int bufOff;

          public Dstu7624Mac(int blockSizeBits, int q)
          {
               engine = new Dstu7624Engine(blockSizeBits);

               blockSize = blockSizeBits / 8;

               macSize = q / 8;

               c = new byte[blockSize];
              
               cTemp = new byte[blockSize];

               kDelta = new byte[blockSize];
               buf = new byte[blockSize];
        }

          public void Init(ICipherParameters parameters)
          {
               if (parameters is KeyParameter)
               {
                    engine.Init(true, (KeyParameter)parameters);

                    engine.ProcessBlock(kDelta, 0, kDelta, 0);
               }
               else
               {
                    throw new ArgumentException("invalid parameter passed to Dstu7624Mac init - "
                    + Platform.GetTypeName(parameters));
               }             
          }

          public string AlgorithmName
          {
               get { return "Dstu7624Mac"; }
          }

          public int GetMacSize()
          {
               return macSize;
          }

          public void Update(byte input)
          {
            if (bufOff == buf.Length)
            {
                processBlock(buf, 0);
                bufOff = 0;
            }

            buf[bufOff++] = input;
        }

          public void BlockUpdate(byte[] input, int inOff, int len)
          {
            if (len < 0)
            {
                throw new ArgumentException(
                    "Can't have a negative input length!");
            }

            int blockSize = engine.GetBlockSize();
            int gapLen = blockSize - bufOff;

            if (len > gapLen)
            {
                Array.Copy(input, inOff, buf, bufOff, gapLen);

                processBlock(buf, 0);

                bufOff = 0;
                len -= gapLen;
                inOff += gapLen;

                while (len > blockSize)
                {
                    processBlock(input, inOff);

                    len -= blockSize;
                    inOff += blockSize;
                }
            }

            Array.Copy(input, inOff, buf, bufOff, len);

            bufOff += len;
        }

        private void processBlock(byte[] input, int inOff)
        {
            Xor(c, 0, input, inOff, cTemp);

            engine.ProcessBlock(cTemp, 0, c, 0);
        }

        private void Xor(byte[] c, int cOff, byte[] input, int inOff, byte[] xorResult)
          {
               for (int byteIndex = 0; byteIndex < blockSize; byteIndex++)
               {
                    xorResult[byteIndex] = (byte)(c[byteIndex + cOff] ^ input[byteIndex + inOff]);
               }
          }

          public int DoFinal(byte[] output, int outOff)
          {
            if (bufOff % buf.Length != 0)
            {
                throw new DataLengthException("Input must be a multiple of blocksize");
            }

            //Last block
            Xor(c, 0, buf, 0, cTemp);
            Xor(cTemp, 0, kDelta, 0, c);
            engine.ProcessBlock(c, 0, c, 0);

            if (macSize + outOff > output.Length)
            {
                throw new DataLengthException("Output buffer too short");
            }

            Array.Copy(c, 0, output, outOff, macSize);

            return macSize;
        }

          public void Reset()
          {
            Arrays.Fill(c, (byte)0x00);
            Arrays.Fill(cTemp, (byte)0x00);
            Arrays.Fill(kDelta, (byte)0x00);
            Arrays.Fill(buf, (byte)0x00);
            engine.Reset();
            engine.ProcessBlock(kDelta, 0, kDelta, 0);
            bufOff = 0;
        }
     }
}
