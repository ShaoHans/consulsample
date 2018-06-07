﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace API服务提供者
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            string ip = config["ip"];
            string port = config["port"];

            var host = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

            if (!string.IsNullOrWhiteSpace(ip) && !string.IsNullOrWhiteSpace(port))
            {
                host.UseUrls($"http://{ip}:{port}");
            }

            return host.Build();
        }
            
    }
}
