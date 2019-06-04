using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace SiteServer.API
{
    public class NamespaceHttpControllerSelector : IHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;
        private readonly Lazy<Dictionary<string, HttpControllerDescriptor>> _controllers;

        public NamespaceHttpControllerSelector(HttpConfiguration config)
        {
            _configuration = config;
            _controllers = new Lazy<Dictionary<string, HttpControllerDescriptor>>(InitializeControllerDictionary);
        }

        private Dictionary<string, HttpControllerDescriptor> InitializeControllerDictionary()
        {
            var dictionary = new Dictionary<string, HttpControllerDescriptor>(StringComparer.OrdinalIgnoreCase);
            var assembliesResolver = _configuration.Services.GetAssembliesResolver();
            var controllersResolver = _configuration.Services.GetHttpControllerTypeResolver();
            var controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);
            foreach (var t in controllerTypes)
            {
                dictionary[$"{t.Namespace}.{t.Name}"] = new HttpControllerDescriptor(_configuration, t.Name, t);
            }
            return dictionary;
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var attributedRoutesData = request.GetRouteData().GetSubRoutes();
            var subRouteData = attributedRoutesData.FirstOrDefault();

            if (subRouteData != null)
            {
                return ((HttpActionDescriptor[]) subRouteData.Route.DataTokens["actions"])[0].ControllerDescriptor;

                //var controllerType = ((HttpActionDescriptor[])subRouteData.Route.DataTokens["actions"])[0].ControllerDescriptor.ControllerType;

                //if (_controllers.Value.TryGetValue($"{controllerType.Namespace}.{controllerType.Name}",
                //    out var controllerDescriptor))
                //{
                //    return controllerDescriptor;
                //}
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _controllers.Value;
        }
    }
}