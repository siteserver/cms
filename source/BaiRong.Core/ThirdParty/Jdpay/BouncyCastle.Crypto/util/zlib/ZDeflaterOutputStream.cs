using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Zlib {
    /// <summary>
    /// Summary description for DeflaterOutputStream.
    /// </summary>
    [Obsolete("Use 'ZOutputStream' instead")]
    public class ZDeflaterOutputStream : Stream {
        protected ZStream z=new ZStream();
        protected int flushLevel=JZlib.Z_NO_FLUSH;
        private const int BUFSIZE = 4192;
        protected byte[] buf=new byte[BUFSIZE];
        private byte[] buf1=new byte[1];

        protected Stream outp;

        public ZDeflaterOutputStream(Stream outp) : this(outp, 6, false) {
        }
    
        public ZDeflaterOutputStream(Stream outp, int level) : this(outp, level, false) {
        }
    
        public ZDeflaterOutputStream(Stream outp, int level, bool nowrap) {
            this.outp=outp;
            z.deflateInit(level, nowrap);
        }
    
    
        public override bool CanRead {
            get {
                // TODO:  Add DeflaterOutputStream.CanRead getter implementation
                return false;
            }
        }
    
        public override bool CanSeek {
            get {
                // TODO:  Add DeflaterOutputStream.CanSeek getter implementation
                return false;
            }
        }
    
        public override bool CanWrite {
            get {
                // TODO:  Add DeflaterOutputStream.CanWrite getter implementation
                return true;
            }
        }
    
        public override long Length {
            get {
                // TODO:  Add DeflaterOutputStream.Length getter implementation
                return 0;
            }
        }
    
        public override long Position {
            get {
                // TODO:  Add DeflaterOutputStream.Position getter implementation
                return 0;
            }
            set {
                // TODO:  Add DeflaterOutputStream.Position setter implementation
            }
        }
    
        public override void Write(byte[] b, int off, int len) {
            if(len==0)
                return;
            int err;
            z.next_in=b;
            z.next_in_index=off;
            z.avail_in=len;
            do{
                z.next_out=buf;
                z.next_out_index=0;
                z.avail_out=BUFSIZE;
                err=z.deflate(flushLevel);
                if(err!=JZlib.Z_OK)
                    throw new IOException("deflating: "+z.msg);
				if (z.avail_out < BUFSIZE)
				{
					outp.Write(buf, 0, BUFSIZE-z.avail_out);
				}
            }
            while(z.avail_in>0 || z.avail_out==0);
        }
    
        public override long Seek(long offset, SeekOrigin origin) {
            // TODO:  Add DeflaterOutputStream.Seek implementation
            return 0;
        }
    
        public override void SetLength(long value) {
            // TODO:  Add DeflaterOutputStream.SetLength implementation

        }
    
        public override int Read(byte[] buffer, int offset, int count) {
            // TODO:  Add DeflaterOutputStream.Read implementation
            return 0;
        }
    
        public override void Flush() {
            outp.Flush();
        }
    
        public override void WriteByte(byte b) {
            buf1[0]=(byte)b;
            Write(buf1, 0, 1);
        }

        public void Finish() {
            int err;
            do{
                z.next_out=buf;
                z.next_out_index=0;
                z.avail_out=BUFSIZE;
                err=z.deflate(JZlib.Z_FINISH);
                if(err!=JZlib.Z_STREAM_END && err != JZlib.Z_OK)
                    throw new IOException("deflating: "+z.msg);
                if(BUFSIZE-z.avail_out>0){
                    outp.Write(buf, 0, BUFSIZE-z.avail_out);
                }
            }
            while(z.avail_in>0 || z.avail_out==0);
            Flush();
        }

        public void End() {
            if(z==null)
                return;
            z.deflateEnd();
            z.free();
            z=null;
        }
        
#if PORTABLE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                try{
                    try{Finish();}
                    catch (IOException) {}
                }
                finally{
                    End();
                    Platform.Dispose(outp);
                    outp=null;
                }
            }
            base.Dispose(disposing);
        }
#else
        public override void Close() {
            try{
                try{Finish();}
                catch (IOException) {}
            }
            finally{
                End();
                Platform.Dispose(outp);
                outp=null;
            }
            base.Close();
        }
#endif
    }
}
