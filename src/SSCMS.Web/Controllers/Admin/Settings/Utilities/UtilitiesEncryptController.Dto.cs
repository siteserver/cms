namespace SSCMS.Web.Controllers.Admin.Settings.Utilities
{
    public partial class UtilitiesEncryptController
    {
        public class SubmitRequest
        {
            public bool IsEncrypt { get; set; }
            public string Value { get; set; }
        }
    }
}
