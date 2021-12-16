namespace InformationSecurity.Dtos
{
    public record UploadFileDto
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? FileUrl { get; set; }
    }
}
