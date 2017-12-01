using System;
using System.IO;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class ServerSrpParams
    {
        protected BigInteger m_N, m_g, m_B;
        protected byte[] m_s;

        public ServerSrpParams(BigInteger N, BigInteger g, byte[] s, BigInteger B)
        {
            this.m_N = N;
            this.m_g = g;
            this.m_s = Arrays.Clone(s);
            this.m_B = B;
        }

        public virtual BigInteger B
        {
            get { return m_B; }
        }

        public virtual BigInteger G
        {
            get { return m_g; }
        }

        public virtual BigInteger N
        {
            get { return m_N; }
        }

        public virtual byte[] S
        {
            get { return m_s; }
        }

        /**
         * Encode this {@link ServerSRPParams} to an {@link OutputStream}.
         * 
         * @param output
         *            the {@link OutputStream} to encode to.
         * @throws IOException
         */
        public virtual void Encode(Stream output)
        {
            TlsSrpUtilities.WriteSrpParameter(m_N, output);
            TlsSrpUtilities.WriteSrpParameter(m_g, output);
            TlsUtilities.WriteOpaque8(m_s, output);
            TlsSrpUtilities.WriteSrpParameter(m_B, output);
        }

        /**
         * Parse a {@link ServerSRPParams} from an {@link InputStream}.
         * 
         * @param input
         *            the {@link InputStream} to parse from.
         * @return a {@link ServerSRPParams} object.
         * @throws IOException
         */
        public static ServerSrpParams Parse(Stream input)
        {
            BigInteger N = TlsSrpUtilities.ReadSrpParameter(input);
            BigInteger g = TlsSrpUtilities.ReadSrpParameter(input);
            byte[] s = TlsUtilities.ReadOpaque8(input);
            BigInteger B = TlsSrpUtilities.ReadSrpParameter(input);

            return new ServerSrpParams(N, g, s, B);
        }
    }
}
