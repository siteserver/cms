using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using NuGet.Packaging;
using SSCMS.Core.StlParser.Model;
using SSCMS.Dto;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesReferenceController : ControllerBase
    {
        private const string Route = "cms/templates/templatesReference";

        private readonly IAuthManager _authManager;

        public TemplatesReferenceController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<List<Element>>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.TemplatesReference))
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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.TemplatesReference))
            {
                return Unauthorized();
            }

            var elements = StlAll.Elements;
            if (!elements.TryGetValue(request.ElementName, out var elementType))
            {
                return NotFound();
            }

            var list = new List<Field>();
            var fields = new List<FieldInfo>();
            if (typeof(StlListBase).IsAssignableFrom(elementType))
            {
                fields.AddRange(typeof(StlListBase).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));
            }
            fields.AddRange(elementType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));

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

        [AllowAnonymous]
        [HttpGet, Route(Route + "/{elementName}")]
        public ActionResult<ListResult> List([FromRoute]string elementName)
        {
            var elements = StlAll.Elements;
            if (!elements.TryGetValue(elementName, out var elementType))
            {
                return NotFound();
            }

            var name = elementName.Substring(4);
            var stlAttribute = (StlElementAttribute)Attribute.GetCustomAttribute(elementType, typeof(StlElementAttribute));

            var references = new List<ListReference>
            {
                new ListReference
                {
                    Name = $"{elementName} {stlAttribute.Title}",
                    Url = $"https://www.siteserver.cn/docs/stl/{name}/"
                }
            };

            var attributes = new List<ListAttribute>();
            var fields = new List<FieldInfo>();
            if (typeof(StlListBase).IsAssignableFrom(elementType))
            {
                fields.AddRange(typeof(StlListBase).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));
            }
            fields.AddRange(elementType.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public));
            foreach (var field in fields)
            {
                var fieldName = field.Name.ToCamelCase();
                var attr = (StlAttributeAttribute)Attribute.GetCustomAttribute(field, typeof(StlAttributeAttribute));

                if (attr != null)
                {
                    attributes.Add(new ListAttribute
                    {
                        Name = fieldName,
                        Description = attr.Title
                    });
                }
            }

            return new ListResult
            {
                Version = 1.1,
                Tags = new List<ListTag>
                {
                    new ListTag
                    {
                        Name = elementName,
                        Description = stlAttribute.Title,
                        Attributes = attributes,
                        References = references
                    }
                }
            };
        }

        public class ListAttribute
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class ListTag
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public List<ListAttribute> Attributes { get; set; }
            public List<ListReference> References { get; set; }
        }

        public class ListReference
        {
            public string Name { get; set; }
            public string Url { get; set; }
        }

        public class ListResult
        {
            public double Version { get; set; }
            public List<ListTag> Tags { get; set; }
        }
    }
}
