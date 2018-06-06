using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API服务提供者.Controllers
{
    [Route("api/user")]
    public class UserController: Controller
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogWarning("请求来了~~~");
            return Json(new { ServerTime = DateTime.Now, Name = "jim" });
        }
    }
}
