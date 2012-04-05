using System;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Security.Cryptography;
using S3Uploader.Models;
using System.Configuration;

namespace S3Uploader.Controllers
{
    public class UploadsController : Controller
    {
        public ActionResult Index()
        {
            string publicKey = ConfigurationManager.AppSettings["AWSAccessKey"];
            string secretKey = ConfigurationManager.AppSettings["AWSSecretKey"];
            string bucketName = ConfigurationManager.AppSettings["AWSBucket"];
            string uploaderPath = ConfigurationManager.AppSettings["AWSUploaderPath"];
            FileUploadViewModel viewModel = GenerateViewModel(publicKey, secretKey, bucketName, uploaderPath);

            return View(viewModel);
        }

        public ActionResult Create(UploadedAsset viewModel)
        {
            if(ModelState.IsValid)
            {
                // TODO: Save viewModel to db
            }

            // TODO: replace with success msg
            return new EmptyResult();
        }

        private FileUploadViewModel GenerateViewModel(string publicKey, string secretKey, string bucketName, string uploaderPath)
        {
            var vm = new FileUploadViewModel
            {
                FormAction = "http://" + bucketName + ".s3.amazonaws.com/",
                FormMethod = "post",
                FormEnclosureType = "multipart/form-data",
                Bucket = bucketName,
                FileId = "uploads/:uuid" + "/${filename}",
                AwsAccessKey = publicKey,
                // one of private, public-read, public-read-write, or authenticated-read
                Acl = "public-read"
            };

            var policy = 
                new 
                {
                    expiration = "2013-04-20T11:54:21.032Z", 
                    conditions = new object[]
                                        {
                                            new { bucket = bucketName },
                                            new [] { "starts-with", "$key", "" },
                                            new { acl = "public-read" },
                                            new { success_action_status = "201" },
                                            new object[] { "content-length-range", 0, 20971520 },
                                            new [] { "starts-with", "$Content-Type", "" }
                                        }
                };

            String serialzedPolicy = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(policy);

            var encoding = new ASCIIEncoding();
            vm.Base64Policy = Convert.ToBase64String(encoding.GetBytes(serialzedPolicy));
            vm.Signature = CreateSignature(secretKey, serialzedPolicy);
            vm.CreatResourceUrl = "Uploads/Create";
            vm.UploaderPath = uploaderPath + "uploader.html";


            vm.ToQuery = vm.FormAction + vm.UploaderPath + "?AWSAccessKeyId=" + vm.AwsAccessKey +
                         "&_policy=" + vm.Base64Policy + "&_signature=" + vm.Signature +
                         "&bucket=" + HttpUtility.UrlEncode(vm.FormAction) + "&key=" + HttpUtility.UrlEncode(vm.FileId);
            
            return vm;
        }

        private string CreateSignature(string secretKey, string policy)
        {
            var encoding = new ASCIIEncoding();
            var policyBytes = encoding.GetBytes(policy);
            var base64Policy = Convert.ToBase64String(policyBytes);

            var secretKeyBytes = encoding.GetBytes(secretKey);
            var hmacsha1 = new HMACSHA1(secretKeyBytes);

            var base64PolicyBytes = encoding.GetBytes(base64Policy);
            var signatureBytes = hmacsha1.ComputeHash(base64PolicyBytes);

            return Convert.ToBase64String(signatureBytes);
        }
    }
}
