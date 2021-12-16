using System.ComponentModel.DataAnnotations;

namespace InformationSecurity.Models
{
    public record UploadFile
    {
        [Key]
        public Guid Id { get; init; }
        [Required]
        [StringLength(255)]
        public string? Name { get; init; }
        public Guid UserPasswordId { get; init; }
    }
}