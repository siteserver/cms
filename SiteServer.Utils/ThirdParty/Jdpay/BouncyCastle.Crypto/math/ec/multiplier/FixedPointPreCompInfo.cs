namespace Org.BouncyCastle.Math.EC.Multiplier
{
    /**
     * Class holding precomputation data for fixed-point multiplications.
     */
    public class FixedPointPreCompInfo
        : PreCompInfo
    {
        protected ECPoint m_offset = null;

        /**
         * Array holding the precomputed <code>ECPoint</code>s used for a fixed
         * point multiplication.
         */
		protected ECPoint[] m_preComp = null;

        /**
         * The width used for the precomputation. If a larger width precomputation
         * is already available this may be larger than was requested, so calling
         * code should refer to the actual width.
         */
        protected int m_width = -1;

        public virtual ECPoint Offset
        {
			get { return m_offset; }
			set { this.m_offset = value; }
		}

        public virtual ECPoint[] PreComp
        {
            get { return m_preComp; }
            set { this.m_preComp = value; }
        }

        public virtual int Width
        {
            get { return m_width; }
            set { this.m_width = value; }
        }
    }
}
