using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.StlParser.Attributes;
using SSCMS.Core.StlParser.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesReferenceController
    {
        [AllowAnonymous]
        [HttpGet, Route(Route + "/{elementName}")]
        public ActionResult<ListResult> ElementName([FromRoute] string elementName)
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
                var fieldName = StringUtils.ToCamelCase(field.Name);
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
    }
}
