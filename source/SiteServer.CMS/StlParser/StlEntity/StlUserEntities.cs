using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.CMS.StlParser.StlEntity
{
    [Stl(Usage = "用户实体", Description = "通过 {user.} 实体在模板中显示用户值")]
    public class StlUserEntities
	{
        private StlUserEntities()
		{
		}

        public const string EntityName = "user";

        public static string GroupName = "GroupName";
        public static string UserId = "UserId";
        public static string UserName = "UserName";
        public static string GroupId = "GroupId";
        public static string CreateDate = "CreateDate";
        public static string LastActivityDate = "LastActivityDate";
        public static string CountOfLogin = "CountOfLogin";
        public static string CountOfWriting = "CountOfWriting";
        public static string DisplayName = "DisplayName";
        public static string Email = "Email";
        public static string Mobile = "Mobile";
        public static string AvatarUrl = "AvatarUrl";
        public static string Organization = "Organization";
        public static string Department = "Department";
        public static string Position = "Position";
        public static string Gender = "Gender";
        public static string Birthday = "Birthday";
        public static string Education = "Education";
        public static string Graduation = "Graduation";
        public static string Address = "Address";
        public static string WeiXin = "WeiXin";
        public static string Qq = "Qq";
        public static string WeiBo = "WeiBo";
        public static string Interests = "Interests";
        public static string Signature = "Signature";

        public static SortedList<string, string> AttributeList => new SortedList<string, string>
        {
	        {GroupName, "用户组名称"},
            {UserId, "用户ID"},
            {UserName, "用户名"},
            {GroupId, "用户组ID"},
            {CreateDate, "注册日期"},
            {LastActivityDate, "最后活动日期"},
            {CountOfLogin, "登录次数"},
            {CountOfWriting, "投稿次数"},
            {DisplayName, "姓名"},
            {Email, "邮箱"},
            {Mobile, "手机"},
            {AvatarUrl, "头像地址"},
            {Organization, "单位"},
            {Department, "部门"},
            {Position, "职位"},
            {Gender, "性别"},
            {Birthday, "出生日期"},
            {Education, "教育程度"},
            {Graduation, "毕业院校"},
            {Address, "通讯地址"},
            {WeiXin, "微信"},
            {Qq, "QQ"},
            {WeiBo, "微博"},
            {Interests, "兴趣爱好"},
            {Signature, "签名"}
        };

	    internal static string Parse(string stlEntity, PageInfo pageInfo)
	    {
	        var parsedContent = string.Empty;
	        if (pageInfo?.UserInfo == null) return string.Empty;

	        try
	        {
	            var entityName = StlParserUtility.GetNameFromEntity(stlEntity);
	            var attributeName = entityName.Substring(6, entityName.Length - 7);

                if (StringUtils.EqualsIgnoreCase(GroupName, attributeName))
                {
                    parsedContent = UserGroupManager.GetGroupName(pageInfo.UserInfo.GroupId);
                }

                else if (StringUtils.EqualsIgnoreCase(UserId, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.UserId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(UserName, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.UserName;
                }
                else if (StringUtils.EqualsIgnoreCase(GroupId, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.GroupId.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(CreateDate, attributeName))
                {
                    parsedContent = DateUtils.Format(pageInfo.UserInfo.CreateDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(LastActivityDate, attributeName))
                {
                    parsedContent = DateUtils.Format(pageInfo.UserInfo.LastActivityDate, string.Empty);
                }
                else if (StringUtils.EqualsIgnoreCase(CountOfLogin, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.CountOfLogin.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(CountOfWriting, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.CountOfWriting.ToString();
                }
                else if (StringUtils.EqualsIgnoreCase(DisplayName, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.DisplayName;
                }
                else if (StringUtils.EqualsIgnoreCase(Email, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Email;
                }
                else if (StringUtils.EqualsIgnoreCase(Mobile, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Mobile;
                }
                else if (StringUtils.EqualsIgnoreCase(AvatarUrl, attributeName))
                {
                    parsedContent = PageUtility.GetUserAvatarUrl(pageInfo.ApiUrl, pageInfo.UserInfo);
                }
                else if (StringUtils.EqualsIgnoreCase(Organization, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Organization;
                }
                else if (StringUtils.EqualsIgnoreCase(Department, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Department;
                }
                else if (StringUtils.EqualsIgnoreCase(Position, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Position;
                }
                else if (StringUtils.EqualsIgnoreCase(Gender, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Gender;
                }
                else if (StringUtils.EqualsIgnoreCase(Birthday, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Birthday;
                }
                else if (StringUtils.EqualsIgnoreCase(Education, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Education;
                }
                else if (StringUtils.EqualsIgnoreCase(Graduation, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Graduation;
                }
                else if (StringUtils.EqualsIgnoreCase(Address, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Address;
                }
                else if (StringUtils.EqualsIgnoreCase(WeiXin, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.WeiXin;
                }
                else if (StringUtils.EqualsIgnoreCase(Qq, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Qq;
                }
                else if (StringUtils.EqualsIgnoreCase(WeiBo, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.WeiBo;
                }
                else if (StringUtils.EqualsIgnoreCase(Interests, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Interests;
                }
                else if (StringUtils.EqualsIgnoreCase(Signature, attributeName))
                {
                    parsedContent = pageInfo.UserInfo.Signature;
                }
                else
                {
                    parsedContent = pageInfo.UserInfo.Additional.GetExtendedAttribute(attributeName);
                }
            }
	        catch
	        {
	            // ignored
	        }

	        return parsedContent;
	    }
	}
}
