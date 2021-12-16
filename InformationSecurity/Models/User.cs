using System.ComponentModel.DataAnnotations;

namespace InformationSecurity.Models
{
    public record User
    {
        [Key]
        public Guid Id { get; init; }
        [Required]
        public string? Username { get; init; }
        public string? Password { get; init; }
        public List<UserPassword>? UserPasswords { get; init; }
        public UserKey? UserKey { get; init; }
    }
}
