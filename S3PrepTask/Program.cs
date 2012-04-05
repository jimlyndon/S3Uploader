using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3PrepTask
{
    class Program
    {
        static void Main(string[] args)
        {
            using (AmazonS3 client = Amazon.AWSClientFactory.CreateAmazonS3Client())
            {
                try
                {
                    NameValueCollection appConfig = ConfigurationManager.AppSettings;

                    // get file path
                    string folder = Path.GetFullPath(Environment.CurrentDirectory + "../../../../S3Uploader/");

                    // write uploader form to s3
                    const string uploaderTemplate = "uploader.html";
                    string file = Path.Combine(folder, "Templates/" + uploaderTemplate);

                    var request = new PutObjectRequest();
                    request.WithBucketName(appConfig["AWSBucket"])
                        .WithKey(appConfig["AWSUploaderPath"] + uploaderTemplate)
                        .WithInputStream(File.OpenRead(file));

                    request.CannedACL = S3CannedACL.PublicRead;
                    request.StorageClass = S3StorageClass.ReducedRedundancy;

                    S3Response response = client.PutObject(request);
                    response.Dispose();

                    // write js to s3
                    var jsFileNames = new[] { "jquery.fileupload.js", "jquery.iframe-transport.js", "s3_uploader.js" };
                    foreach (var jsFileName in jsFileNames)
                    {
                        file = Path.Combine(folder, "Scripts/s3_uploader/" + jsFileName);
                        request = new PutObjectRequest();

                        request.WithBucketName(appConfig["AWSBucket"])
                            .WithKey(appConfig["AWSUploaderPath"] + "js/" + jsFileName)
                            .WithInputStream(File.OpenRead(file));

                        request.CannedACL = S3CannedACL.PublicRead;
                        request.StorageClass = S3StorageClass.ReducedRedundancy;

                        response = client.PutObject(request);
                        response.Dispose();
                    }
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    if (amazonS3Exception.ErrorCode != null &&
                        (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                        amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                    {
                        Console.WriteLine("Please check the provided AWS Credentials.");
                        Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                    }
                    else
                    {
                        Console.WriteLine("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                    }
                }
            }      
        }
    }
}
