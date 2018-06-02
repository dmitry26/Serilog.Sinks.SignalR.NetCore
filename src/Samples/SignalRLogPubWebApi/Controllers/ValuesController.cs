using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
		public ValuesController(ILogger<ValuesController> logger) => _logger = logger;

		private readonly ILogger _logger;

		// GET api/values
		[HttpGet]
        public IEnumerable<string> Get()
        {
			_logger.LogInformation("Received GET request: api/values");
			return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
			_logger.LogInformation($"Received GET request: api/values/{id}");
			return "value";
        }        
    }
}
