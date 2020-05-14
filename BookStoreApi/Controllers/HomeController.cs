using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BookStoreApi.Controllers
{
    /// <summary>
    /// This is a scaffolding controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        /// <summary>
        /// This is a get verb controller
        /// </summary>
        /// <returns></returns>
        // GET: api/Home
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        /// <summary>
        /// This is a get endpoint by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Home/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Home
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Home/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
