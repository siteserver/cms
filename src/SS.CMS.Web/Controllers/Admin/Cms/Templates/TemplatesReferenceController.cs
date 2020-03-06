using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.StlParser.Model;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
{
    [Route("admin/cms/templates/templatesReference")]
    public partial class TemplatesReferenceController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public TemplatesReferenceController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<List<Element>>> List([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateReference))
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

        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<Field>>> ListFields([FromBody]FieldsRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.TemplateReference))
            {
                return Unauthorized();
            }

            var elements = StlAll.Elements;
            if (!elements.TryGetValue(request.ElementName, out var elementType))
            {
                return NotFound();
            }

            var list = new List<Field>();
            var fields = elementType.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
            foreach (var field in fields)
            {
                var fieldName = field.Name.ToCamelCase();
                var attr = (StlAttributeAttribute)Attribute.GetCustomAttribute(field, typeof(StlAttributeAttribute));

                if (attr != null)
                {
                    list.Add(new Field
                    {
                        Name = fieldName,
                        Title = attr.Title
                    });
                }
            }

            return list;
        }
    }
}
