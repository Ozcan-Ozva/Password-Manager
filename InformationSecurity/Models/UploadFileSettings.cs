namespace InformationSecurity.Models
{
    public class UploadFileSettings
    {
        public int MaxBytes { get; set; }
        public string[]? AcceptedFileTypes { get; set; }

        public bool isSupported(string fileName)
        {
            return AcceptedFileTypes.Any(s => s == Path.GetExtension(fileName).ToLower());
        }
    }
}
