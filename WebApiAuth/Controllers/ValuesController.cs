using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApiAuth.Model;

namespace WebApiAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Auth(authCode: new[] { "123", "234" }),]
    public class ValuesController : ControllerBase
    {
        private Configs Config;
        public ValuesController(IOptions<Configs> setting)
        {
            Config = setting.Value;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1" + Config.ApiRequestExpireTime, "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
