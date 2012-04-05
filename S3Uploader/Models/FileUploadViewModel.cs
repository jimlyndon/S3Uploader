namespace S3Uploader.Models
{
    public class FileUploadViewModel
    {
        public string CreatResourceUrl { get; set; }
        public string UploaderPath { get; set; }
        public string FormAction { get; set; }
        public string FormMethod { get; set; }
        public string FormEnclosureType { get; set; }
        public string Bucket { get; set; }
        public string FileId { get; set; }
        public string AwsAccessKey { get; set; }
        public string Acl { get; set; }
        public string Base64Policy { get; set; }
        public string Signature { get; set; }
        public string ToQuery { get; set; }
    }
}