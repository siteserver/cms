using System.Collections.Generic;
using SSCMS.Core.Plugins;
using SSCMS.Core.Utils;

namespace SSCMS.Cli.Services
{
    public partial class ApiService
    {
        public (bool success, List<PluginAndUser> results, string failureMessage) PluginSearch(string word)
        {
            //var client = new RestClient(CloudUtils.Api.GetCliUrl(RestUrlPluginSearch)) { Timeout = -1 };
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("application/json", TranslateUtils.JsonSerialize(new SearchRequest
            //{
            //    Word = word
            //}), ParameterType.RequestBody);
            //var response = client.Execute<List<PluginAndUser>>(request);
            //if (!response.IsSuccessful)
            //{
            //    return (false, null, response.ErrorMessage);
            //}

            //return (true, response.Data, null);

            var url = CloudUtils.Api.GetCliUrl(RestUrlPluginSearch);
            return RestUtils.Post<SearchRequest, List<PluginAndUser>>(url, new SearchRequest
            {
                Word = word
            });
        }
    }
}
