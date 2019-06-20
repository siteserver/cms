using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IUserManager _userManager;

        public ValuesController(ISettingsManager settingsManager, IUserManager userManager)
        {
            _settingsManager = settingsManager;
            _userManager = userManager;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            await _userManager.SignInAsync(new UserInfo
            {
                Id = 1,
                UserName = "admin"
            }, true);
            var user = User;
            var val = User.FindFirst(ClaimTypes.Name)?.Value;

            return new string[] { "value1", "value2", user.Identity.IsAuthenticated.ToString(), val };
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var user = User;
            return user.Identity.IsAuthenticated.ToString();
        }

        [Authorize]
        [HttpGet("{str}")]
        public ActionResult<string> Get(string str)
        {
            var user = User;
            return user.Identity.IsAuthenticated.ToString();
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