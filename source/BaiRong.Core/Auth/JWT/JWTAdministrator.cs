using System.Collections.Generic;

namespace BaiRong.Core.Auth.JWT
{
    public class JWTAdministrator
    {
        public string userName { get; set; }
        public string displayName { get; set; }

        public Dictionary<string, string> config { get; set; }

        public JWTAdministrator() { }

        public JWTAdministrator(BaiRong.Core.Model.AdministratorInfo administratorInfo)
        {
            userName = administratorInfo.UserName;
            displayName = administratorInfo.DisplayName;
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = administratorInfo.Email;
            }
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = administratorInfo.UserName;
            }
        }
    }
}
