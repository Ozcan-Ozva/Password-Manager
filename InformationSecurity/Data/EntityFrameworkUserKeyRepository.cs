using InformationSecurity.Models;
using InformationSecurity.Repositories;
using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;

namespace InformationSecurity.Data
{
    public class EntityFrameworkUserKeyRepository : IUserKeyRepository
    {
        private readonly ApplicationDbContext _context;

        public EntityFrameworkUserKeyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateUserKeyAsync(UserKey userKey)
        {
            await _context.UserKeys.AddAsync(userKey);
        }

        public Task DeleteUserKeyAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserKey> GetUserKeyAsync(Guid id)
        {
            var user = await _context.UserKeys.FindAsync(id);
            return await Task.FromResult(user);
        }

        public async Task<IEnumerable<UserKey>> GetUserKeysAsync()
        {
            return await Task.FromResult(_context.UserKeys.ToList());
        }

        public Task UpdateUserKeyAsync(UserKey userKey)
        {
            throw new NotImplementedException();
        }

        public byte[] Encrypt(string PlainText, string key)
        {
            // Get bytes of plaintext string
            byte[] plainBytes = Encoding.UTF8.GetBytes(PlainText);

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
                key32[i] = key5[i % 6];
            }
            var aes = new AesGcm(key32);
            aes.Encrypt(nonce, plainBytes.AsSpan(), cipherBytes, tag);

            // Encode for transmission
            return Encoding.UTF8.GetBytes(Convert.ToBase64String(encryptedData));
        }
    }
}
