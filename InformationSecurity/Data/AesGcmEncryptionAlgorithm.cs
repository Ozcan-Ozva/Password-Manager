using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace InformationSecurity.Data
{
    public class AesGcmEncryptionAlgorithm
    {
        public static string EncryptData(IEnumerable<string> PlainTexts, string key)
        {
            string result = "";
            foreach (string s in PlainTexts)
            {
                result += (Encrypt(s, key) + ":");
            }
            return result;
        }
        public static string Encrypt(string plainText, string key)
        {
            // Get bytes of plaintext string
            byte[] plainBytes = Encoding.ASCII.GetBytes(plainText);

            // Get parameter sizes
            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
            int tagSize = AesGcm.TagByteSizes.MaxSize;
            int cipherSize = plainBytes.Length;

            // We write everything into one big array for easier encoding
            int encryptedDataLength = 4 + nonceSize + 4 + tagSize + cipherSize;
            Span<byte> encryptedData = encryptedDataLength < 1024
                                     ? stackalloc byte[encryptedDataLength]
                                     : new byte[encryptedDataLength].AsSpan();

            // Copy parameters
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(0, 4), nonceSize);
            BinaryPrimitives.WriteInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4), tagSize);
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Generate secure nonce
            RandomNumberGenerator.Fill(nonce);

            // Encrypt
            byte[] key5 = Encoding.ASCII.GetBytes(key);
            var key32 = new byte[32];
            //RandomNumberGenerator.Fill(key32);
            for (int i = 0; i < 32; i++)
            {
                key32[i] = key5[i % key5.Length];
            }
            var aes = new AesGcm(key32);
            aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

            // Encode for transmission
            return Convert.ToBase64String(encryptedData);
        }

        public static IEnumerable<string> DecryptData(string cipherData, string key)
        {
            List<string> data = new List<string>();
            var credArray = cipherData.Split(':');
            foreach (var cred in credArray)
            {
                data.Add(Decrypt(cred, key));
            }
            return data;
        }

        public static string Decrypt(string cipher, string key)
        {
            // Decode
            Span<byte> encryptedData = Convert.FromBase64String(cipher).AsSpan();

            // Extract parameter sizes
            int nonceSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(0, 4));
            int tagSize = BinaryPrimitives.ReadInt32LittleEndian(encryptedData.Slice(4 + nonceSize, 4));
            int cipherSize = encryptedData.Length - 4 - nonceSize - 4 - tagSize;

            // Extract parameters
            var nonce = encryptedData.Slice(4, nonceSize);
            var tag = encryptedData.Slice(4 + nonceSize + 4, tagSize);
            var cipherBytes = encryptedData.Slice(4 + nonceSize + 4 + tagSize, cipherSize);

            // Decrypt
            Span<byte> plainBytes = cipherSize < 1024
                                  ? stackalloc byte[cipherSize]
                                  : new byte[cipherSize];
            byte[] key5 = Encoding.ASCII.GetBytes(key);
            var key32 = new byte[32];
            //RandomNumberGenerator.Fill(key32);
            for (int i = 0; i < 32; i++)
            {
                key32[i] = key5[i % key5.Length];
            }
            var aes = new AesGcm(key32);
            aes.Decrypt(nonce, cipherBytes, tag, plainBytes);

            // Convert plain bytes back into string
            return Encoding.ASCII.GetString(plainBytes);
        }
    }
}
