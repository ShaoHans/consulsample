using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace 服务消费者.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDnsQuery _dns;
        private readonly ServiceDiscoveryOptions _options;

        public ValuesController(IDnsQuery dns, IOptions<ServiceDiscoveryOptions> options)
        {
            _dns = dns;
            _options = options.Value;

        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _dns.ResolveServiceAsync("service.consul", _options.ServiceName);
            var firstHost = result.First();
            var address = firstHost.AddressList.Any() ? firstHost.AddressList.FirstOrDefault().ToString() : firstHost.HostName;
            var port = firstHost.Port;

            using (var client = new HttpClient())
            {
                var serviceResult = await client.GetStringAsync($"http://{address}:{port}/api/user");
                return Content(serviceResult);
            }
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
    }
}
