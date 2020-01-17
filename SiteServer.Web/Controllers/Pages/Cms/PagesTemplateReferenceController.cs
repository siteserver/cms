using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/templateReference")]
    public partial class PagesTemplateReferenceController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<List<Element>> List()
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, auth.SiteId, Constants.SitePermissions.TemplateReference);

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
        public async Task<List<Field>> ListFields([FromBody]FieldsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSitePermissionsAsync(Request, request.SiteId, Constants.SitePermissions.TemplateReference);

            var elements = StlAll.Elements;
            if (!elements.TryGetValue(request.ElementName, out var elementType))
            {
                return Request.NotFound<List<Field>>();
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
