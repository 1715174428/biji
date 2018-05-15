using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace CoreWebAPITest1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("Login")]
        public string Login(string userName,string Password)
        {
            return userName + "," + Password;
        }

        [HttpGet("Login2/{userName}/{password}")]//http://..../Login2/admin/123
        public async Task<string> Login2(string userName, string password)
        {
            return userName + "," + password;
        }

        [HttpGet(nameof(Test1))]
        public IActionResult Test1()
        {
            return this.Content("hello");
        }

        [HttpGet(nameof(Test2))]

        public HttpResponseMessage Test2()
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK) { Content = new StringContent("hello")};
        }
    }
}
