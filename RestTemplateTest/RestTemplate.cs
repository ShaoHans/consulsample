using DnsClient;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Http;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace RestTemplateTest
{
    public class RestTemplate
    {
        private readonly IDnsQuery _dnsQuery;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consulTcpAddress">Consul服务TCP地址</param>
        /// <param name="conosulTcpPort">Consul服务TCP端口</param>
        public RestTemplate(string consulTcpAddress = "127.0.0.1", int conosulTcpPort = 8600)
        {
            _dnsQuery = new LookupClient(new IPEndPoint(IPAddress.Parse(consulTcpAddress), conosulTcpPort));
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 根据API服务名称解析出对应的IP地址和端口
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <returns>返回IP地址和端口（如：192.168.1.100:5000）</returns>
        private async Task<string> ResolveServiceAddressAsync(string serviceName)
        {
            var result = await _dnsQuery.ResolveServiceAsync("service.consul", serviceName);
            var firstHost = result.FirstOrDefault();
            if(firstHost == null)
            {
                throw new Exception($"未发现{serviceName}的服务实例");
            }
            var address = firstHost.AddressList.Any() ? firstHost.AddressList.FirstOrDefault().ToString() : firstHost.HostName;
            var port = firstHost.Port;
            return $"{address}:{port}";
        }

        /// <summary>
        /// 根据API服务Url解析出实际的含IP地址端口的Url，如：http://userservice/api/user/1 解析为 http://192.168.1.100:5000/api/user/1
        /// </summary>
        /// <param name="url">http://userservice/api/user/1</param>
        /// <returns>http://192.168.1.100:5000/api/user/1</returns>
        private async Task<string> ResolveServiceUrlAsync(string url)
        {
            Uri uri = new Uri(url);
            string serviceName = uri.Host;
            string serviceAddress = await ResolveServiceAddressAsync(serviceName);
            return $"{uri.Scheme}://{serviceAddress}{uri.PathAndQuery}";
        }

        public async Task<RestResponse> SendAsync(HttpRequestMessage requestMessage)
        {
            var responseMessage = await _httpClient.SendAsync(requestMessage);
            return new RestResponse(responseMessage.StatusCode, responseMessage.Headers);
        }

        public async Task<RestResponse<T>> SendForEntityAsync<T>(HttpRequestMessage requestMessage)
        {
            var responseMessage = await _httpClient.SendAsync(requestMessage);
            string content = await responseMessage.Content.ReadAsStringAsync();
            T body = default(T);
            if(!string.IsNullOrWhiteSpace(content))
            {
                body = JsonConvert.DeserializeObject<T>(content);
            }
            var restResponse = new RestResponse<T>(responseMessage.StatusCode, responseMessage.Headers, body);
            return restResponse;
        }

        /// <summary>
        /// 发送Get请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="requestHeaders"></param>
        /// <returns></returns>
        public async Task<RestResponse<T>> GetForEntityAsync<T>(string url, HttpRequestHeaders requestHeaders = null)
        {
            using (HttpRequestMessage requestMsg = new HttpRequestMessage())
            {
                if (requestHeaders != null)
                {
                    foreach (var header in requestHeaders)
                    {
                        requestMsg.Headers.Add(header.Key, header.Value);
                    }
                }
                requestMsg.Method = HttpMethod.Get;
                requestMsg.RequestUri = new Uri(await ResolveServiceUrlAsync(url));
                return await SendForEntityAsync<T>(requestMsg);
            }
        }
    }
}
