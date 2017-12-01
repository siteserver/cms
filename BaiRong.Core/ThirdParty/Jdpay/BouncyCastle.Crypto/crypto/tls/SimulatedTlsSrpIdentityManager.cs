using System;

using Org.BouncyCastle.Crypto.Agreement.Srp;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Crypto.Tls
{
    /**
     * An implementation of {@link TlsSRPIdentityManager} that simulates the existence of "unknown" identities
     * to obscure the fact that there is no verifier for them. 
     */
    public class SimulatedTlsSrpIdentityManager
        :   TlsSrpIdentityManager
    {
        private static readonly byte[] PREFIX_PASSWORD = Strings.ToByteArray("password");
        private static readonly byte[] PREFIX_SALT = Strings.ToByteArray("salt");

        /**
         * Create a {@link SimulatedTlsSRPIdentityManager} that implements the algorithm from RFC 5054 2.5.1.3
         *
         * @param group the {@link SRP6GroupParameters} defining the group that SRP is operating in
         * @param seedKey the secret "seed key" referred to in RFC 5054 2.5.1.3
         * @return an instance of {@link SimulatedTlsSRPIdentityManager}
         */
        public static SimulatedTlsSrpIdentityManager GetRfc5054Default(Srp6GroupParameters group, byte[] seedKey)
        {
            Srp6VerifierGenerator verifierGenerator = new Srp6VerifierGenerator();
            verifierGenerator.Init(group, TlsUtilities.CreateHash(HashAlgorithm.sha1));

            HMac mac = new HMac(TlsUtilities.CreateHash(HashAlgorithm.sha1));
            mac.Init(new KeyParameter(seedKey));

            return new SimulatedTlsSrpIdentityManager(group, verifierGenerator, mac);
        }

        protected readonly Srp6GroupParameters mGroup;
        protected readonly Srp6VerifierGenerator mVerifierGenerator;
        protected readonly IMac mMac;

        public SimulatedTlsSrpIdentityManager(Srp6GroupParameters group, Srp6VerifierGenerator verifierGenerator, IMac mac)
        {
            this.mGroup = group;
            this.mVerifierGenerator = verifierGenerator;
            this.mMac = mac;
        }

        public virtual TlsSrpLoginParameters GetLoginParameters(byte[] identity)
        {
            mMac.BlockUpdate(PREFIX_SALT, 0, PREFIX_SALT.Length);
            mMac.BlockUpdate(identity, 0, identity.Length);

            byte[] salt = new byte[mMac.GetMacSize()];
            mMac.DoFinal(salt, 0);

            mMac.BlockUpdate(PREFIX_PASSWORD, 0, PREFIX_PASSWORD.Length);
            mMac.BlockUpdate(identity, 0, identity.Length);

            byte[] password = new byte[mMac.GetMacSize()];
            mMac.DoFinal(password, 0);

            BigInteger verifier = mVerifierGenerator.GenerateVerifier(salt, identity, password);

            return new TlsSrpLoginParameters(mGroup, verifier, salt);
        }
    }
}
