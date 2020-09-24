using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Model;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesReferenceController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<List<Element>>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesReference))
            {
                return Unauthorized();
            }

            var list = new List<Element>();
            var elements = StlAll.Elements;
            foreach (var elementName in elements.Keys)
            {
                if (!elements.TryGetValue(elementName, out var elementType)) continue;

                var name = elementName.Substring(4);
                var stlAttribute = (StlElementAttribute)Attribute.GetCustomAttribute(elementType, typeof(StlElementAttribute));

                list.Add(new Element
                {
                    Name = name,
                    ElementName = elementName,
                    Title = stlAttribute.Title
                });
            }

            return list;
        }
    }
}
