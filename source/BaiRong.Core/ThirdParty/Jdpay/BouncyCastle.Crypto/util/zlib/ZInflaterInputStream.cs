using System;
using System.IO;

namespace Org.BouncyCastle.Utilities.Zlib {
    /// <summary>
    /// Summary description for DeflaterOutputStream.
    /// </summary>
    [Obsolete("Use 'ZInputStream' instead")]
    public class ZInflaterInputStream : Stream {
        protected ZStream z=new ZStream();
        protected int flushLevel=JZlib.Z_NO_FLUSH;
        private const int BUFSIZE = 4192;
        protected byte[] buf=new byte[BUFSIZE];
        private byte[] buf1=new byte[1];

        protected Stream inp=null;
        private bool nomoreinput=false;

        public ZInflaterInputStream(Stream inp) : this(inp, false) {
        }
    
        public ZInflaterInputStream(Stream inp, bool nowrap) {
            this.inp=inp;
            z.inflateInit(nowrap);
            z.next_in=buf;
            z.next_in_index=0;
            z.avail_in=0;
        }
    
        public override bool CanRead {
            get {
                // TODO:  Add DeflaterOutputStream.CanRead getter implementation
                return true;
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
                return false;
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
        }
    
        public override long Seek(long offset, SeekOrigin origin) {
            // TODO:  Add DeflaterOutputStream.Seek implementation
            return 0;
        }
    
        public override void SetLength(long value) {
            // TODO:  Add DeflaterOutputStream.SetLength implementation

        }
    
        public override int Read(byte[] b, int off, int len) {
            if(len==0)
                return(0);
            int err;
            z.next_out=b;
            z.next_out_index=off;
            z.avail_out=len;
            do {
                if((z.avail_in==0)&&(!nomoreinput)) { // if buffer is empty and more input is avaiable, refill it
                    z.next_in_index=0;
                    z.avail_in=inp.Read(buf, 0, BUFSIZE);//(BUFSIZE<z.avail_out ? BUFSIZE : z.avail_out));
                    if(z.avail_in<=0) {
                        z.avail_in=0;
                        nomoreinput=true;
                    }
                }
                err=z.inflate(flushLevel);
                if(nomoreinput&&(err==JZlib.Z_BUF_ERROR))
                    return(0);
                if(err!=JZlib.Z_OK && err!=JZlib.Z_STREAM_END)
                    throw new IOException("inflating: "+z.msg);
                if((nomoreinput||err==JZlib.Z_STREAM_END)&&(z.avail_out==len))
                    return(0);
            } 
            while(z.avail_out==len&&err==JZlib.Z_OK);
            //System.err.print("("+(len-z.avail_out)+")");
            return(len-z.avail_out);
        }
    
        public override void Flush() {
            inp.Flush();
        }
    
        public override void WriteByte(byte b) {
        }

#if PORTABLE
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Platform.Dispose(inp);
            }
            base.Dispose(disposing);
        }
#else
        public override void Close()
        {
            Platform.Dispose(inp);
            base.Close();
        }
#endif

        public override int ReadByte() {
            if(Read(buf1, 0, 1)<=0)
                return -1;
            return(buf1[0]&0xFF);
        }
    }
}
