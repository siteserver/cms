using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("api/pages/cms/templateReference")]
    public class TemplateReferenceController : ApiController
    {
        private const string Route = "{siteId}";
        private const string RouteName = "{siteId}/{name}";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetList(int siteId)
        {
            try
            {
                var request = new AuthRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Template))
                {
                    return Unauthorized();
                }

                return Ok(new
                {
                    Value = StlAll.Elements.Keys
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteName)]
        public IHttpActionResult Get(int siteId, string name)
        {
            try
            {
                var request = new AuthRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissions.HasSitePermissions(siteId, ConfigManager.WebSitePermissions.Template))
                {
                    return Unauthorized();
                }

                if (!StlAll.Elements.TryGetValue(name, out var elementType))
                {
                    return NotFound();
                }

                var stlAttribute = (StlClassAttribute)Attribute.GetCustomAttribute(elementType, typeof(StlClassAttribute));
                var attributes = new List<object>();

                var fields = elementType.GetFields(BindingFlags.Static | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    if (field.GetValue(null) is Attr attr)
                    {
                        attributes.Add(new
                        {
                            attr.Name,
                            Type = AttrUtils.GetAttrTypeText(attr.Type),
                            Enums = attr.GetEnums(elementType, siteId),
                            attr.Description
                        });
                    }
                }

                return Ok(new
                {
                    Value = stlAttribute,
                    Attributes = attributes
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}