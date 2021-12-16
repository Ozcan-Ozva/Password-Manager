using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace InformationSecurity.Data
{
    class RSAAsymmetric
    {
        private static RSACryptoServiceProvider CSP = new RSACryptoServiceProvider(2048);
        private static RSACryptoServiceProvider fakeCSP = new RSACryptoServiceProvider(2048);
        private RSAParameters _privateKey;
        private RSAParameters _fakeprivateKey;
        private RSAParameters _publicKey;

        public RSAAsymmetric()
        {
            _privateKey = CSP.ExportParameters(true);
            _fakeprivateKey = fakeCSP.ExportParameters(true);
            _publicKey = CSP.ExportParameters(false);
        }

        public byte[] GetXMLPublicKey() => _publicKey.Modulus;

        public string GetPublicKey()
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, _publicKey);
            return sw.ToString();
        }

        public string Encrypt(string plainText)
        {
            CSP = new RSACryptoServiceProvider();
            CSP.ImportParameters(_publicKey);
            var data = Encoding.Unicode.GetBytes(plainText);
            var cypher = CSP.Encrypt(data, false);
            return Convert.ToBase64String(cypher);
        }

        public string Decrypt(string cypherText)
        {
            var dataByte = Convert.FromBase64String(cypherText);
            CSP.ImportParameters(_privateKey);
            var plainText = CSP.Decrypt(dataByte, false);
            return Encoding.Unicode.GetString(plainText);
        }
    }
}
