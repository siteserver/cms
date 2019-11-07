using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils;

namespace SiteServer.CMS.Model.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserInfo, User>().AfterMap<ToUserAction>();
            CreateMap<User, UserInfo>().AfterMap<ToUserInfoAction>();
        }

        private class ToUserAction : IMappingAction<UserInfo, User>
        {
            public void Process(UserInfo source, User destination, ResolutionContext context)
            {
                destination.Locked = TranslateUtils.ToBool(source.IsLockedOut);
                destination.Checked = TranslateUtils.ToBool(source.IsChecked);
                destination.DisplayName = string.IsNullOrEmpty(destination.DisplayName)
                    ? destination.UserName
                    : destination.DisplayName;
                destination.Password = string.Empty;
                destination.PasswordFormat = string.Empty;
                destination.PasswordSalt = string.Empty;
                destination.GroupName = UserGroupManager.GetUserGroupName(destination.GroupId);

                var allAttributes = DataProvider.UserDao.TableColumns.Select(x => x.AttributeName).ToList();

                foreach (var o in source.ToDictionary(allAttributes))
                {
                    destination.Set(o.Key, o.Value);
                }
            }
        }

        private class ToUserInfoAction : IMappingAction<User, UserInfo>
        {
            public void Process(User source, UserInfo destination, ResolutionContext context)
            {
                destination.IsLockedOut = source.Locked.ToString();
                destination.IsChecked = source.Checked.ToString();

                var dict = new Dictionary<string, object>();

                var styleInfoList = TableStyleManager.GetUserStyleInfoList();

                foreach (var styleInfo in styleInfoList)
                {
                    //dict.Remove(styleInfo.AttributeName);
                    dict[styleInfo.AttributeName] = source.Get(styleInfo.AttributeName);
                }

                //foreach (var attributeName in UserAttribute.AllAttributes.Value)
                //{
                //    if (StringUtils.StartsWith(attributeName, "Is"))
                //    {
                //        dict.Remove(attributeName);
                //        dict[attributeName] = source.GetBool(attributeName);
                //    }
                //    else
                //    {
                //        dict.Remove(attributeName);
                //        dict[attributeName] = source.Get(attributeName);
                //    }
                //}

                //foreach (var attributeName in UserAttribute.ExcludedAttributes.Value)
                //{
                //    dict.Remove(attributeName);
                //}

                destination.SettingsXml = TranslateUtils.JsonSerialize(dict);
            }
        }
    }
}
