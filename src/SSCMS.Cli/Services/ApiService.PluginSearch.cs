using System.Collections.Generic;

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

            return ExecutePost<SearchRequest, List<PluginAndUser>>(RestUrlPluginSearch, new SearchRequest
            {
                Word = word
            });
        }
    }
}
