using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Consul;

namespace 服务消费者2
{
    public class RestTemplate
    {
        private readonly ServiceDiscoveryOptions _serviceDiscoveryOptions;

        public RestTemplate(IOptions<ServiceDiscoveryOptions> options)
        {
            _serviceDiscoveryOptions = options.Value;
        }

        public async Task<string> ResolveBaseUrlAsync()
        {
            using (ConsulClient client = new ConsulClient(config => config.Address = new Uri(_serviceDiscoveryOptions.Consul.HttpEndpoint)))
            {
                var services = (await client.Agent.Services()).Response;
                var userServices = services.Where(s => s.Value.Service.Equals(_serviceDiscoveryOptions.ServiceName, StringComparison.OrdinalIgnoreCase))
                    .Select(s => s.Value)
                    .ToList();

                // TODO：注入负载均衡策略
                var userService = userServices.ElementAt(Environment.TickCount % userServices.Count);

                return $"{client.Config.Address.Scheme}://{userService.Address}:{userService.Port}";
            }
        }
    }
}
