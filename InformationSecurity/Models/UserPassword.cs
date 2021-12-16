using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InformationSecurity.Models
{
    public record UserPassword
    {
        public Guid Id { get; init; }
        public string? Address { get; init; }
        public string? UserName { get; init; }
        public string? Password { get; init; }
        public string? Description { get; init; }
        public Collection<UploadFile>? UploadFiles { get; init; }
        // User Id 
        public Guid UserId { get; init; }
        public User? User { get; init; }
    }
}
