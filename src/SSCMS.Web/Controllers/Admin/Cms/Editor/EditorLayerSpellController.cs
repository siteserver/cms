using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class EditorLayerSpellController : ControllerBase
    {
        private const string Route = "cms/editor/editorLayerSpell";
        private const string RouteAddWords = "cms/editor/editorLayerSpell/actions/addWords";

        private readonly IAuthManager _authManager;
        private readonly ISpellManager _spellManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public EditorLayerSpellController(IAuthManager authManager, ISpellManager spellManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _authManager = authManager;
            _spellManager = spellManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public class AddWordsRequest : ChannelRequest
        {
            public string Word { get; set; }
        }

        public class SubmitRequest : ChannelRequest
        {
            public SpellResult Results { get; set; }
        }

        public class SubmitResult
        {
            public bool IsErrorWords { get; set; }
            public int Count { get; set; }
            public List<ErrorWord> ErrorWords { get; set; }
        }
    }
}
