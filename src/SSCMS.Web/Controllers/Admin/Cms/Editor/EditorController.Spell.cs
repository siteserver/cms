﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;
using System;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteSpell)]
        public async Task<ActionResult<SpellResult>> Spell([FromBody] SpellRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    MenuUtils.ContentPermissions.Add, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            var content = await _pathManager.EncodeContentAsync(site, channel, request.Content);

            var fullContent = $"{content.Title}{content.SubTitle}{content.Summary}{content.Author}{content.Source}{content.Body}";

            try
            {
                var results = await _spellManager.SpellingCheckAsync(fullContent);
                if (results.Success)
                {
                    return results;
                }
                else
                {
                    return this.Error(results.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return this.Error(ex.Message);
            }
        }
    }
}