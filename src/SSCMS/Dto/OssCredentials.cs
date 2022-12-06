using Datory;

namespace SSCMS.Dto
{
    public class OssCredentials
    {
        public string Endpoint { get; set; }
        public string AccessKeyId { get; set; }
        public string AccessKeySecret { get; set; }
        public string Expiration { get; set; }
        public string SecurityToken { get; set; }
        public string BucketName { get; set; }
    }
}