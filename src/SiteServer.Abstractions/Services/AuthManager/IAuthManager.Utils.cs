namespace SiteServer.Abstractions
{
    public partial interface IAuthManager
    {
        string SessionId { get; }
    }
}