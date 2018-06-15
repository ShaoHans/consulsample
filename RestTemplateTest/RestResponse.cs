using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace RestTemplateTest
{
    public class RestResponse
    {
        /// <summary>
        /// 响应状态码
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// 响应的报文头
        /// </summary>
        public HttpResponseHeaders Headers { get; private set; }

        public RestResponse(HttpStatusCode statusCode, HttpResponseHeaders headers)
        {
            StatusCode = statusCode;
            Headers = headers;
        }
    }

    public class RestResponse<T> : RestResponse
    {
        /// <summary>
        /// 响应体
        /// </summary>
        public T Body { get;}

        public RestResponse(HttpStatusCode statusCode, HttpResponseHeaders headers, T body = default(T)) : base(statusCode, headers)
        {
            Body = body;
        }
    }
}
