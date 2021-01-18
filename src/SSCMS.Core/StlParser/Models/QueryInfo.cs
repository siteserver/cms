using System.Collections.Generic;
using Datory;

namespace SSCMS.Core.StlParser.Models
{
    public class QueryInfo
    {
        public string Type { get; set; }
        public string Column { get; set; }
        public string Op { get; set; }
        public string Value { get; set; }
        public DataType DataType { get; set; }
        public List<QueryInfo> Queries { get; set; }
    }
}
