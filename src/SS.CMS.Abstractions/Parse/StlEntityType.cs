namespace SS.CMS.Abstractions.Parse
{
    public enum StlEntityType
	{
		Stl,					    //通用实体
        StlElement,                 //STL元素实体
		Content,					//内容实体
		Channel,					//栏目实体
        Request,                    //参数获取实体
        Navigation,                 //导航地址
        Sql,                        //Sql实体
        User,                       //用户实体
        Undefined
	}
}
