using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Produces("application/json")]
	[Route("")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class HomeController : Controller
	{
		// GET: api/Home
		[HttpGet("")]
		[Route("api")]
		[Route("api/home")]
		public IActionResult Get() => Content("Hello from SignalRLogPubWebApi!");
	}
}
