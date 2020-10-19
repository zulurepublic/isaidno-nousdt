using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nethereum.Util;
using Newtonsoft.Json;
using OKexTime.Context;
using OKexTime.Models;

namespace OKexTime.Controllers
{
    [ApiController]
    public class Subscribe : ControllerBase
    {
        private readonly ILogger<Subscribe> _logger;
        private readonly OkexContext _context;
        private readonly IConfiguration _config;
        public Subscribe(ILogger<Subscribe> logger, OkexContext context, IConfiguration config)
        {
            _logger = logger;
            _context = context;
            _config = config;
        }

        [HttpPost]
        [Route("api/v1/[controller]")]
        public IActionResult Post(UsersRequestUSDT request)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(request));
            try
            {
                request.EthereumAddress = request.EthereumAddress.ConvertToEthereumChecksumAddress();
            }
            catch (Exception)
            {
                return BadRequest("Address not valid");
            }
            if (request.ExpectedAmount < _config.GetValue<decimal>("minAmount"))
                return BadRequest("Amount to low");
            var index = request.Phone.IndexOf('(');
            if (index < 0)
                return BadRequest("Phone not valid");
            var withoutCode = request.Phone.Substring(index);
            var userId = withoutCode.Replace("(", "")
                .Replace(")", "")
                .Replace(" ", "")
                .Replace("-", "");
            request.UserId = userId;
            _context.RequestUsdts.Add(request);
            _context.SaveChanges();
            return Created("", request.Id);
        }
    }
}
