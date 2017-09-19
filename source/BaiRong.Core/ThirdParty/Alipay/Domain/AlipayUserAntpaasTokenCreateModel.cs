using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserAntpaasTokenCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserAntpaasTokenCreateModel : AopObject
    {
        /// <summary>
        /// 账户绑定手机号
        /// </summary>
        [XmlElement("bind_mobile")]
        public string BindMobile { get; set; }

        /// <summary>
        /// 当前用户国家/地区，两位国家代码 国家代码以ISO 3166-1 标准为准
        /// </summary>
        [XmlElement("country")]
        public string Country { get; set; }

        /// <summary>
        /// 蚂蚁通行证登录密码，原始密码使用RSA加密后传输,示例：a11111:JuZeA/DR9NJU8aJPONdq9ZMbXI2zNHyoq3MwOxmjjY17ItpsbyuaPrfKsOzVBX9IFKyfr1Whrhlbl4WbYu9q2Xai6mWCNTKbYwvCDuY+pjel6dkka+/kK5ZwWjsN2W6eWAf5TNdy2pqheI08ZMvv1gD6t5zIQBbLGh/rv19NTd2gMwSTO++5Onek9saJi8iG+W32AOPPBWcaMv6yNJJCyA0QloBY5qFQdTOoW8DAg3dyfmFEDWNrdUxBZdL5+ZUS7HdK4i+k+vATH7tX0isEA8F40wSNzrrgTX8Dq+NcGzrAlGpSAqxgUDcxog2hrhDXBl4puYfLHskHBNKhwv0BIw==
        /// </summary>
        [XmlElement("login_password")]
        public string LoginPassword { get; set; }

        /// <summary>
        /// 蚂蚁通行证注册登录号，用于账户登录，邮箱、手机号等
        /// </summary>
        [XmlElement("logon_id")]
        public string LogonId { get; set; }

        /// <summary>
        /// 用户是否需要补全安全密码，true：需要补全，false：不需要补全。  默认为false，不需要补全。
        /// </summary>
        [XmlElement("need_supply")]
        public bool NeedSupply { get; set; }

        /// <summary>
        /// 蚂蚁通行证安全密码，通过RSA加密传输,示例：b111111:Dsz+toTsBnIwyG7IWuzshgwXxkHImAACx8yUb9PhP4+zyEV/xAPM/N9AdAFh0Di9xLG6syACSTn4KYMYs5GoSyaI2TJ0e2TcC8Gm5VJK0uinJVRhgWPnfsyiSl9amhObbPXtQgVO7szmYI8duChphFz0I2MKMOQVvWWF7Z9sSXZCfUGLPtL6ZS+xb3W9scczasR49IO8V49ll5NGzwFTvvc9yGPTxj3AIPbUPBG4byktfPWKoiRpTstGQORmAGPZT+gumEJxxpcATMcsnJMnHYfdrhEW8/VFleC5m5aaoCl2mdmEgh4X6NSt8MpgnUxhXwW090+dx3UQwU5pqGRvkw==
        /// </summary>
        [XmlElement("security_password")]
        public string SecurityPassword { get; set; }

        /// <summary>
        /// 注册来源场景，shangshu_register--上树对接蚂蚁通行证，该场景登录号、登录密码、用户类型为必传参数
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }

        /// <summary>
        /// 用户类型，1 -- 企业用户， 2 -- 个人用户
        /// </summary>
        [XmlElement("user_type")]
        public string UserType { get; set; }
    }
}
