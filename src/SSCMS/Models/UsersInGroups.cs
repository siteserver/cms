using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_UsersInGroups")]
    public class UsersInGroups : Entity
    {        
        [DataColumn]
        public int UserId { get; set; }

        [DataColumn]
        public int GroupId { get; set; }
    }
}