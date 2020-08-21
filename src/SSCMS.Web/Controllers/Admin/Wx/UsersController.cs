using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Wx;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class UsersController : ControllerBase
    {
        private const string Route = "wx/users";
        private const string RouteActionsAddTag = "wx/users/actions/addTag";
        private const string RouteActionsEditTag = "wx/users/actions/editTag";
        private const string RouteActionsDeleteTag = "wx/users/actions/deleteTag";
        private const string RouteActionsBlock = "wx/users/actions/block";
        private const string RouteActionsUnBlock = "wx/users/actions/unBlock";
        private const string RouteActionsRemark = "wx/users/actions/remark";

        private readonly IAuthManager _authManager;
        private readonly IWxManager _wxManager;
        private readonly IWxUserRepository _wxUserRepository;

        public UsersController(IAuthManager authManager, IWxManager wxManager, IWxUserRepository wxUserRepository)
        {
            _authManager = authManager;
            _wxManager = wxManager;
            _wxUserRepository = wxUserRepository;
        }

        public class GetRequest : SiteRequest
        {
            public bool Init { get; set; }
            public bool IsBlock { get; set; }
            public int TagId { get; set; }
            public string Keyword { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class GetResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public IEnumerable<WxUserTag> Tags { get; set; }
            public int Total { get; set; }
            public int Count { get; set; }
            public IEnumerable<WxUser> Users { get; set; }
        }

        public class AddTagRequest : SiteRequest
        {
            public string TagName { get; set; }
        }

        public class AddTagResult
        {
            public IEnumerable<WxUserTag> Tags { get; set; }
        }

        public class EditTagRequest : SiteRequest
        {
            public int TagId { get; set; }
            public string TagName { get; set; }
        }

        public class EditTagResult
        {
            public IEnumerable<WxUserTag> Tags { get; set; }
        }

        public class DeleteTagRequest : SiteRequest
        {
            public int TagId { get; set; }
        }

        public class DeleteTagResult
        {
            public IEnumerable<WxUserTag> Tags { get; set; }
        }

        public class BlockRequest : SiteRequest
        {
            public List<string> OpenIds { get; set; }
        }

        public class RemarkRequest : SiteRequest
        {
            public string OpenId { get; set; }
            public string Remark { get; set; }
        }
    }
}
