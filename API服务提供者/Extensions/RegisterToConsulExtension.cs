using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API服务提供者.Extensions
{
    /// <summary>
    /// 向Consul注册RestApi服务
    /// </summary>
    public static class RegisterToConsulExtension
    {
        /*
         * 
        "ServiceDiscovery": {
            "ServiceName": "UserServiceAPI",
            "Consul": {
              "HttpEndpoint": "http://127.0.0.1:8500",
              "DnsEndpoint": {
                "Address": "127.0.0.1",
                "Port": 8600
              }
            }
          } 

        */

        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            // 配置Consul服务注册地址
            services.Configure<ServiceDiscoveryOptions>(configuration.GetSection("ServiceDiscovery"));
            // 配置Consul客户端
            services.AddSingleton<IConsulClient>(service =>
            new ConsulClient
            (
                config =>
                {
                    var serviceDisOptions = service.GetRequiredService<IOptions<ServiceDiscoveryOptions>>().Value;
                    if (!string.IsNullOrWhiteSpace(serviceDisOptions.Consul.HttpEndpoint))
                    {
                        config.Address = new Uri(serviceDisOptions.Consul.HttpEndpoint);
                    }
                }
            ));

            return services;
        }


        public static IApplicationBuilder UseConsul(this IApplicationBuilder app)
        {
            IConsulClient consul = app.ApplicationServices.GetRequiredService<IConsulClient>();
            IApplicationLifetime appLife = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            IOptions<ServiceDiscoveryOptions> serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ServiceDiscoveryOptions>>();
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>()
                .Addresses
                .Select(p => new Uri(p));

            // 向Consul客户端注册RestApi服务
            foreach (var address in addresses)
            {
                var serviceId = $"{serviceOptions.Value.ServiceName}_{address.Host}:{address.Port}";

                var httpCheck = new AgentServiceCheck()
                {
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1),
                    Interval = TimeSpan.FromSeconds(30),
                    HTTP = new Uri(address, "HealthCheck").OriginalString
                };

                var registration = new AgentServiceRegistration()
                {
                    Checks = new[] { httpCheck },
                    Address = address.Host,
                    ID = serviceId,
                    Name = serviceOptions.Value.ServiceName,
                    Port = address.Port
                };

                consul.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                // 服务应用停止后发注册RestApi服务
                appLife.ApplicationStopping.Register(() =>
                {
                    consul.Agent.ServiceDeregister(serviceId).GetAwaiter().GetResult();
                });
            }

            return app;
        }
    }
}
