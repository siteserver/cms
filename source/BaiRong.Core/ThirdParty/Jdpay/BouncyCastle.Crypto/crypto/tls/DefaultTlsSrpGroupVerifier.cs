using System;
using System.Collections;

using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    public class DefaultTlsSrpGroupVerifier
        :   TlsSrpGroupVerifier
    {
        protected static readonly IList DefaultGroups = Platform.CreateArrayList();

        static DefaultTlsSrpGroupVerifier()
        {
            DefaultGroups.Add(Srp6StandardGroups.rfc5054_1024);
            DefaultGroups.Add(Srp6StandardGroups.rfc5054_1536);
            DefaultGroups.Add(Srp6StandardGroups.rfc5054_2048);
            DefaultGroups.Add(Srp6StandardGroups.rfc5054_3072);
            DefaultGroups.Add(Srp6StandardGroups.rfc5054_4096);
            DefaultGroups.Add(Srp6StandardGroups.rfc5054_6144);
            DefaultGroups.Add(Srp6StandardGroups.rfc5054_8192);
        }

        // Vector is (SRP6GroupParameters)
        protected readonly IList mGroups;

        /**
         * Accept only the group parameters specified in RFC 5054 Appendix A.
         */
        public DefaultTlsSrpGroupVerifier()
            :   this(DefaultGroups)
        {
        }

        /**
         * Specify a custom set of acceptable group parameters.
         * 
         * @param groups a {@link Vector} of acceptable {@link SRP6GroupParameters}
         */
        public DefaultTlsSrpGroupVerifier(IList groups)
        {
            this.mGroups = groups;
        }

        public virtual bool Accept(Srp6GroupParameters group)
        {
            foreach (Srp6GroupParameters entry in mGroups)
            {
                if (AreGroupsEqual(group, entry))
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual bool AreGroupsEqual(Srp6GroupParameters a, Srp6GroupParameters b)
        {
            return a == b || (AreParametersEqual(a.N, b.N) && AreParametersEqual(a.G, b.G));
        }

        protected virtual bool AreParametersEqual(BigInteger a, BigInteger b)
        {
            return a == b || a.Equals(b);
        }
    }
}
