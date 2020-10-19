using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Okex.Net;
using Okex.Net.RestObjects;
using OKexTime.Context;
using OKexTime.Models;
namespace OKexTime.Services
{
    public class GetAccountHistory : IHostedService, IDisposable
    {
        private readonly ILogger<GetAccountHistory> _logger;
        private Timer _timer;
        private readonly IConfiguration _config;
        private readonly OkexContext _context;
        // ReSharper disable once InconsistentNaming
        private OkexClient clinet;

        public GetAccountHistory(ILogger<GetAccountHistory> logger, IConfiguration config, OkexContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            clinet = new OkexClient();
            clinet.SetApiCredentials(_config.GetValue<string>("API_KEY"),
                _config.GetValue<string>("API_SECRET"),
                _config.GetValue<string>("API_PASS"));
            _timer = new Timer(GetData, null, TimeSpan.FromSeconds(10),
                TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void GetData(object? state)
        {
            _logger.LogInformation("Get history");
            var history = clinet.Funding_GetDepositHistoryByCurrency(_config["currency"]).Data.ToList()
                .Where(detail => detail.Status == OkexFundingDepositStatus.DepositSuccessful).ToList();
            //
            _logger.LogDebug(JsonConvert.SerializeObject(history));
            var settledRequests = _context.RequestUsdts.Where(entity => entity.Settled).ToList();
            var usettledHistory = (from entity in history let coincidence = settledRequests.FirstOrDefault(e => e.TxId == entity.TxId) where coincidence == default select entity).ToList();
            //
            var unsettledEntities = _context.RequestUsdts.Where(entity => entity.Settled == false).ToList();
            foreach (var entity in unsettledEntities)
            {
                var desiredDepositDetails = usettledHistory.FirstOrDefault(details => details.FromAddress == entity.UserId
                                                                                      && details.Amount ==
                                                                                      entity.ExpectedAmount);
                if (desiredDepositDetails != default)
                    PayForConsolidation(entity, desiredDepositDetails);
            }
        }

        private async void PayForConsolidation(UsersRequestUSDT request, OkexFundingDepositDetails details)
        {
            var (txId, error) = await TokenService.Pay(_config, request);
            if (txId != null)
            {
                _logger.LogInformation("For user request: " + JsonConvert.SerializeObject(request) + "\nTxId: " + txId);
                request.Settled = true;
                request.TxId = details.TxId;
                request.EthereumStatus = txId;
                _context.RequestUsdts.Update(request);
                await _context.SaveChangesAsync();
            }
            else
            {
                _logger.LogError("For user request: " + JsonConvert.SerializeObject(request) + "\nError:\n" + error);
                request.EthereumStatus = error;
                _context.RequestUsdts.Update(request);
                await _context.SaveChangesAsync();
            }
            
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            clinet?.Dispose();
            _timer?.Dispose();
        }
    }
}
