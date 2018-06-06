using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace 服务消费者2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly RestTemplate _restTemplate;
        public ValuesController(RestTemplate restTemplate)
        {
            _restTemplate = restTemplate;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            using (var client = new HttpClient())
            {
                var serviceResult = await client.GetStringAsync($"{await _restTemplate.ResolveBaseUrlAsync()}/api/user");
                return Content(serviceResult);
            }
        }

        
    }
}
