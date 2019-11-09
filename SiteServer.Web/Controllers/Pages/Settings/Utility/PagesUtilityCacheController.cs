using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Settings.Utility
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/utilityCache")]
    public class PagesUtilityCacheController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult Get()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                var parameterList = new List<KeyValuePair<string, string>>();
                foreach (var key in CacheUtils.AllKeys)
                {
                    var value = GetCacheValue(key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        parameterList.Add(new KeyValuePair<string, string>(key, value));
                    }
                }

                return Ok(new
                {
                    Value = parameterList,
                    parameterList.Count
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private static string GetCacheValue(string key)
        {
            var value = CacheUtils.Get(key);

            var valueType = value?.GetType().FullName;
            if (valueType == null)
            {
                return null;
            }

            if (valueType == "System.String")
            {
                return $"String, Length:{value.ToString().Length}";
            }

            if (valueType == "System.Int32")
            {
                return value.ToString();
            }

            if (valueType.StartsWith("System.Collections.Generic.List"))
            {
                return $"List, Count:{((ICollection)value).Count}";
            }

            return valueType;
        }

        [HttpPost, Route(Route)]
        public IHttpActionResult Post()
        {
            try
            {
                var request = new AuthenticatedRequest();
                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasSystemPermissions(ConfigManager.SettingsPermissions.Utility))
                {
                    return Unauthorized();
                }

                CacheUtils.ClearAll();
                CacheDbUtils.Clear();

                var parameterList = new List<KeyValuePair<string, string>>();
                foreach (var key in CacheUtils.AllKeys)
                {
                    var value = GetCacheValue(key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        parameterList.Add(new KeyValuePair<string, string>(key, value));
                    }
                }

                return Ok(new
                {
                    Value = parameterList,
                    parameterList.Count
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
