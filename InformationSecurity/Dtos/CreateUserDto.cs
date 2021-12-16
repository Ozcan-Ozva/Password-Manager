using System.ComponentModel.DataAnnotations;

namespace InformationSecurity.Dtos
{
    public record CreateUserDto
    {
        [Required]
        public string? Username { get; init; }
        public string? Password { get; init; }
    }
}
