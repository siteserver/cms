namespace SiteServer.CMS.StlParser.Model
{
	
	public enum EStlEntityType
	{
		Stl,					    //通用实体
        StlElement,                 //STL元素实体
		Content,					//内容实体
		Channel,					//栏目实体
        Photo,                      //商品图片实体
        Comment,                    //评论实体
        Request,                    //参数获取实体
        Navigation,                 //导航地址
        Sql,                        //Sql实体
        User,                       //用户实体
        Unkown
	}

    public class EStlEntityTypeUtils
	{
        public const string RegexStringAll = @"{stl\.[^{}]*}|{stl:[^{}]*}|{content\.[^{}]*}|{channel\.[^{}]*}|{comment\.[^{}]*}|{request\.[^{}]*}|{sql\.[^{}]*}|{user\.[^{}]*}|{navigation\.[^{}]*}|{photo\.[^{}]*}";

        public const string RegexStringSql = @"{sql.[^{}]*}";

        public const string RegexStringUser = @"{user.[^{}]*}";

        public static string GetValue(EStlEntityType type)
        {
            if (type == EStlEntityType.Stl)
			{
                return "Stl";
            }
            if (type == EStlEntityType.StlElement)
            {
                return "StlElement";
            }
            if (type == EStlEntityType.Content)
            {
                return "Content";
            }
            if (type == EStlEntityType.Channel)
            {
                return "Channel";
            }
            if (type == EStlEntityType.Photo)
            {
                return "Photo";
            }
            if (type == EStlEntityType.Comment)
            {
                return "Comment";
            }
            if (type == EStlEntityType.Request)
            {
                return "Request";
            }
            if (type == EStlEntityType.Navigation)
            {
                return "Navigation";
            }
            if (type == EStlEntityType.Sql)
            {
                return "Sql";
            }
            if (type == EStlEntityType.User)
            {
                return "User";
            }
            return "Unkown";
        }

		public static bool Equals(EStlEntityType type, string typeStr)
		{
			if (string.IsNullOrEmpty(typeStr)) return false;
			if (string.Equals(GetValue(type).ToLower(), typeStr.ToLower()))
			{
				return true;
			}
			return false;
		}

        public static EStlEntityType GetEntityType(string stlEntity)
        {
            var type = EStlEntityType.Unkown;
            if (!string.IsNullOrEmpty(stlEntity))
            {
                stlEntity = stlEntity.Trim().ToLower();

                if (stlEntity.StartsWith("{stl."))
                {
                    return EStlEntityType.Stl;
                }
                if (stlEntity.StartsWith("{stl:"))
                {
                    return EStlEntityType.StlElement;
                }
                if (stlEntity.StartsWith("{content."))
                {
                    return EStlEntityType.Content;
                }
                if (stlEntity.StartsWith("{channel."))
                {
                    return EStlEntityType.Channel;
                }
                if (stlEntity.StartsWith("{photo."))
                {
                    return EStlEntityType.Photo;
                }
                if (stlEntity.StartsWith("{comment."))
                {
                    return EStlEntityType.Comment;
                }
                if (stlEntity.StartsWith("{request."))
                {
                    return EStlEntityType.Request;
                }
                if (stlEntity.StartsWith("{navigation."))
                {
                    return EStlEntityType.Navigation;
                }
                if (stlEntity.StartsWith("{sql."))
                {
                    return EStlEntityType.Sql;
                }
                if (stlEntity.StartsWith("{user."))
                {
                    return EStlEntityType.User;
                }
            }
            return type;
        }
	}
}
