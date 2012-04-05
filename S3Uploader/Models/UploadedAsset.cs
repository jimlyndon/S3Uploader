namespace S3Uploader.Models
{
    public class UploadedAsset
    {
        public string uuid { get; set; }
        public string file_name { get; set; }
        public string s3_key { get; set; }
        public string file_size { get; set; }
        public string file_type { get; set; }
    }
}