using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMS.Cli.Services
{
    public class InternalServerError
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime AddDate { get; set; }
    }
}
