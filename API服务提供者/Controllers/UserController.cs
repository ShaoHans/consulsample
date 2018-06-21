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

        private static List<User> _users = new List<User>
            {
                new User{ Id=1,Name="tom",Gender="男",Birthday=DateTime.Now.AddYears(-14)},
                new User{ Id=2,Name="lily",Gender="女",Birthday=DateTime.Now.AddYears(-16).AddMonths(-2)},
                new User{ Id=3,Name="lucy",Gender="女",Birthday=DateTime.Now.AddYears(-18).AddDays(-28)},
                new User{ Id=4,Name="jim",Gender="男",Birthday=DateTime.Now.AddYears(-15)},
                new User{ Id=5,Name="hans",Gender="男",Birthday=DateTime.Now.AddYears(-19)},
                new User{ Id=6,Name="mei",Gender="女",Birthday=DateTime.Now.AddYears(-20)},
            };

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogWarning("请求来了~~~");
            return Json(_users);
        }

        [HttpGet]
        [Route("{id}")]
        public User GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        [HttpPost]
        [Route("add")]
        public bool AddUser([FromBody]User user)
        {
            if(user == null || string.IsNullOrWhiteSpace(user.Name))
            {
                return false;
            }

            _users.Add(user);
            return true;
        }

        [HttpPut]
        [Route("edit")]
        public bool EditUser([FromBody]User newUser)
        {
            var oldUser = _users.FirstOrDefault(u => u.Id == newUser.Id);
            if(oldUser == null)
            {
                return false;
            }

            _users.Remove(oldUser);
            _users.Add(newUser);
            return true;
        }
    }
}
