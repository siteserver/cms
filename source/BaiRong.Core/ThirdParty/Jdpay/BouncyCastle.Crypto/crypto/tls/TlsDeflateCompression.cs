using System;
using System.IO;

using Org.BouncyCastle.Utilities.Zlib;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class TlsDeflateCompression : TlsCompression
    {
        public const int LEVEL_NONE = JZlib.Z_NO_COMPRESSION;
        public const int LEVEL_FASTEST = JZlib.Z_BEST_SPEED;
        public const int LEVEL_SMALLEST = JZlib.Z_BEST_COMPRESSION;
        public const int LEVEL_DEFAULT = JZlib.Z_DEFAULT_COMPRESSION;

        protected readonly ZStream zIn, zOut;

        public TlsDeflateCompression()
            : this(LEVEL_DEFAULT)
        {
        }

        public TlsDeflateCompression(int level)
        {
            this.zIn = new ZStream();
            this.zIn.inflateInit();

            this.zOut = new ZStream();
            this.zOut.deflateInit(level);
        }

        public virtual Stream Compress(Stream output)
        {
            return new DeflateOutputStream(output, zOut, true);
        }

        public virtual Stream Decompress(Stream output)
        {
            return new DeflateOutputStream(output, zIn, false);
        }

        protected class DeflateOutputStream : ZOutputStream
        {
            public DeflateOutputStream(Stream output, ZStream z, bool compress)
                : base(output, z)
            {
                this.compress = compress;

                /*
                 * See discussion at http://www.bolet.org/~pornin/deflate-flush.html .
                 */
                this.FlushMode = JZlib.Z_SYNC_FLUSH;
            }

            public override void Flush()
            {
                /*
                 * TODO The inflateSyncPoint doesn't appear to work the way I hoped at the moment.
                 * In any case, we may like to accept PARTIAL_FLUSH input, not just SYNC_FLUSH.
                 * It's not clear how to check this in the Inflater.
                 */
                //if (!this.compress && (z == null || z.istate == null || z.istate.inflateSyncPoint(z) <= 0))
                //{
                //    throw new TlsFatalAlert(AlertDescription.decompression_failure);
                //}
            }
        }
    }
}
