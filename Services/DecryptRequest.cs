using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace OKexTime.Services
{
    public class DecryptRequest
    {
        private readonly RequestDelegate _next;
        public DecryptRequest(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            
            //try { request.EnableRewind(); } catch { }
            var stream = request.Body;
            var originalContent = await new StreamReader(stream).ReadToEndAsync();
            Console.WriteLine("OriginalContent: " + originalContent);

            var requestContent = new StringContent(originalContent, Encoding.ASCII, "application/json");
            stream = await requestContent.ReadAsStreamAsync();//modified stream
            request.Body = stream;
            await _next.Invoke(context);
        }


    }
}
