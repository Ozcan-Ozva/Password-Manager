namespace InformationSecurity.Models
{
    public record UserKey
    {
        public Guid Id { get; init; }
        public byte[]? PrivateKey { get; init; }
        public Guid PublicKey { get; init; }
        public Guid UserId { get; init; }
        public User? User { get; init; }
    }
}
