using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API服务提供者.Controllers
{
    [Route("api/user")]
    public class UserController: Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Json(new { ServerTime = DateTime.Now, Name = "jim" });
        }
    }
}
