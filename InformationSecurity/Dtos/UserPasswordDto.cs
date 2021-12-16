using System.ComponentModel.DataAnnotations;

namespace InformationSecurity.Dtos
{
    public record UserPasswordDto
    {
        public string? Address { get; init; }
        public string? UserName { get; init; }
        public string? Password { get; init; }
        public string? Description { get; init; }
        // Username
        [Required]
        public string? MainUsername { get; init; }
    }
}
