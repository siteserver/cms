namespace SiteServer.Plugin
{
    /// <summary>表示将用于处理Rest Api请求事件的方法。</summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">Rest Api请求事件数据的对象。</param>
    /// <returns>
    /// Rest Api请求将返回的对象，系统将把此对象系列化为JSON字符串返回。
    /// </returns>
    public delegate object RestApiEventHandler(object sender, RestApiEventArgs e);
}
