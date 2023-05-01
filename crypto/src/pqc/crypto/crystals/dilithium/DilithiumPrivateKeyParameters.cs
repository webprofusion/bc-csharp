using Org.BouncyCastle.Utilities;

namespace Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium
{
    public sealed class DilithiumPrivateKeyParameters
        : DilithiumKeyParameters
    {
        internal byte[] m_rho;
        internal byte[] m_k;
        internal byte[] m_tr;
        internal byte[] m_s1;
        internal byte[] m_s2;
        internal byte[] m_t0;

        private byte[] m_t1;

        public DilithiumPrivateKeyParameters(DilithiumParameters parameters,  byte[] rho, byte[] K, byte[] tr,
            byte[] s1, byte[] s2, byte[] t0, byte[] t1)
            : base(true, parameters)
        {
            m_rho = Arrays.Clone(rho);
            m_k = Arrays.Clone(K);
            m_tr = Arrays.Clone(tr);
            m_s1 = Arrays.Clone(s1);
            m_s2 = Arrays.Clone(s2);
            m_t0 = Arrays.Clone(t0);
            m_t1 = Arrays.Clone(t1);
        }

        public byte[] GetEncoded() => Arrays.ConcatenateAll(m_rho, m_k, m_tr, m_s1, m_s2, m_t0);

        public byte[] K => Arrays.Clone(m_k);

        public byte[] GetPublicKey() => DilithiumPublicKeyParameters.GetEncoded(m_rho, m_t1);

        public DilithiumPublicKeyParameters GetPublicKeyParameters() =>
            new DilithiumPublicKeyParameters(Parameters, m_rho, m_t1);

        public byte[] Rho => Arrays.Clone(m_rho);

        public byte[] S1 => Arrays.Clone(m_s1);

        public byte[] S2 => Arrays.Clone(m_s2);

        public byte[] T0 => Arrays.Clone(m_t0);

        public byte[] T1 => Arrays.Clone(m_t1);

        public byte[] Tr => Arrays.Clone(m_tr);
    }
}
