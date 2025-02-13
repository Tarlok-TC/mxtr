using System;
using Amazon.S3;
using Amazon.S3.Model;

namespace mxtrAutomation.Websites.Platform.Utils
{
    public interface IAmazonS3Utils
    {
        string UploadFile(string filePath, string s3Bucket, string newFileName);
    }
    public class AmazonS3Utils : IAmazonS3Utils
    {
        Amazon.S3.AmazonS3Client S3Client = null;
        public AmazonS3Utils(string accessKeyId, string secretAccessKey, string serviceUrl)
        {
            Amazon.S3.AmazonS3Config s3Config = new Amazon.S3.AmazonS3Config();
            s3Config.ServiceURL = serviceUrl;

            this.S3Client = new Amazon.S3.AmazonS3Client(accessKeyId, secretAccessKey, s3Config);
        }
        public string UploadFile(string filePath, string s3Bucket, string newFileName)
        {
            Amazon.S3.Model.PutObjectRequest s3PutRequest = new Amazon.S3.Model.PutObjectRequest();
            s3PutRequest = new Amazon.S3.Model.PutObjectRequest();
            s3PutRequest.FilePath = filePath;
            s3PutRequest.BucketName = s3Bucket;
            s3PutRequest.CannedACL = Amazon.S3.S3CannedACL.PublicRead;
            string LinkURL = string.Empty;

            if (!string.IsNullOrWhiteSpace(newFileName))
            {
                s3PutRequest.Key = newFileName;
            }

            s3PutRequest.Headers.Expires = new DateTime(2037, 1, 1);
            try
            {
                Amazon.S3.Model.PutObjectResponse s3PutResponse = this.S3Client.PutObject(s3PutRequest);
                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
                request.BucketName = s3Bucket;
                request.Key = newFileName;
                request.Expires = new DateTime(2037, 1, 1);
                request.Protocol = Protocol.HTTP;
                LinkURL = this.S3Client.GetPreSignedURL(request);
                return LinkURL;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}