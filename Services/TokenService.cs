using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Nethereum.HdWallet;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using OKexTime.Contract;
using OKexTime.Contract.ContractDefinition;
using OKexTime.Models;

namespace OKexTime.Services
{
    public static class TokenService
    {
        private static string ZeroAddress = "0x0000000000000000000000000000000000000000";
        public static async Task<(string txId, string error)> Pay(IConfiguration config, UsersRequestUSDT request)
        {
            var contractDecimals = Convert.ToDecimal(Math.Pow(10, config.GetValue<int>("decimals")));
            var rpcClient = new RpcClient(new Uri(config["rpc_client"]));
            var wallet = new Wallet(config["mnemo"], "");
            var account = wallet.GetAccount(0);
            var web3 = new Web3(account, rpcClient);
            var noBtcService = new NOBTCService(web3, config["erc20_address"]);
            var fullMintAmount = new BigInteger(request.ExpectedAmount * contractDecimals);
            var commission = new BigInteger(request.ExpectedAmount * 0.01m * contractDecimals);
            var userAmount = fullMintAmount - commission;
            try
            {
                var mintFunction = new MintFunction()
                {
                    Account = request.EthereumAddress,
                    Amount = userAmount,
                    Commission = commission
                };
                var response = await noBtcService.MintRequestAsync(mintFunction);
                return (response, null);
            }
            catch (Exception e)
            {
                return (null, e.Message);
            }
        }
    }
}
