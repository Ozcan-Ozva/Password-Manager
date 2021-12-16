using System.ComponentModel.DataAnnotations;

namespace InformationSecurity.Dtos
{
    public record UserDto
    {
        [Required]
        public string? Username { get; init; }
        public string? Password { get; init; }
        public string? PublicKey { get; init; }
        public string? PrivateKey { get; init; }
    }
}
