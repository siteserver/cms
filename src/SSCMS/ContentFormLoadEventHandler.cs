namespace SSCMS
{
    /// <summary>表示将用于处理内容编辑（新增）页面的载入事件的方法。</summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">内容编辑（新增）页面的载入事件数据的对象。</param>
    /// <returns>
    /// 返回内容属性的Html，内容编辑（新增）页面将以此Html显示内容字段表单。
    /// 如果返回值为null或者Empty，内容编辑（新增）页面将隐藏此内容字段表单。
    /// </returns>
    public delegate string ContentFormLoadEventHandler(object sender, ContentFormLoadEventArgs e);
}