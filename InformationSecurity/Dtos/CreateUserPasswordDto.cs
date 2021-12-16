using InformationSecurity.Models;

namespace InformationSecurity.Dtos
{
    public record CreateUserPasswordDto
    {
        public string? Address { get; init; }
        public string? UserName { get; init; }
        public string? Password { get; init; }
        public string? Description { get; init; }
        public Guid UserId { get; set; }
    }
}
