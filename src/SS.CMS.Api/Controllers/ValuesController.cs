using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Services;

namespace SS.CMS.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;

        public ValuesController(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2", _settingsManager.IsNightlyUpdate.ToString(), _settingsManager.SecretKey, _settingsManager.DatabaseType.Value, _settingsManager.ContentRootPath };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value) { }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value) { }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id) { }
    }
}