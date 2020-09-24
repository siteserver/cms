using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.StlParser.Model;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesReferenceController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<List<Field>>> ListFields([FromBody] FieldsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, Types.SitePermissions.TemplatesReference))
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
                var fieldName = StringUtils.ToCamelCase(field.Name);
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
