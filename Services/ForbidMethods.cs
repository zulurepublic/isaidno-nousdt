using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OKexTime.Services
{
    public class ForbidMethods
    {
        private readonly RequestDelegate _next;

        public ForbidMethods(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            var method = context.Request.Method;
            if (method == "POST" || method == "GET") // 
            {
                if (method == "POST")
                {
                    var remoteIp = context.Connection.RemoteIpAddress;

                    if (remoteIp.IsIPv4MappedToIPv6)
                    {
                        remoteIp = remoteIp.MapToIPv4();
                    }

                    var whiteIpList = new List<IPAddress>
                    {
                        IPAddress.Parse("1.1.1.1"), IPAddress.Parse("2.2.2.2")
                    };
                    var goodIp = false;
                    foreach (var ip in whiteIpList.Where(ip => remoteIp.Equals(ip)))
                    {
                        goodIp = true;
                    }
                    if (!goodIp)
                    {
                        Console.WriteLine($"Forbidden Request from IP: {remoteIp}");
                        context.Response.StatusCode = 403;
                        return;
                    }
                    await _next.Invoke(context);
                }
                else
                    await _next.Invoke(context);
            }
            else
            {
                context.Response.StatusCode = 403;
                return;
            }
        }
    }
}
