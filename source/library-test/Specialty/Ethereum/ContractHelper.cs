using System;
using Xunit;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Linq;
using FluentAssertions;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Nethereum.HdWallet;
using Nethereum.Web3;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3.Accounts;

namespace UniversalIdentity.Library.Test.Specialized.Ethereum
{
    public static class ContractHelper
    {
        public static Account GetAccount()
        {
            var wallet = new Wallet(TestConstants.TestMnemonic, null);
            var account = wallet.GetAccount(2); 
            Console.WriteLine($"Address : {account.Address} - Private key : {account.PrivateKey}");
            return account;
        }

        public static Web3 GetWeb3(Account account)
        {
            var infuraUrl = TestConstants.GetInfuraUrl();
            Console.WriteLine(infuraUrl);
            var web3 = new Web3(account, infuraUrl);
            return web3;
        }

        public static HexBigInteger GetMaxGas()
        {
            return new HexBigInteger(1000000);
        }

        public static DateTime ConvertFromUnixTimestamp(HexBigInteger hexDate)
        {
            var time = (int)hexDate.Value;
            DateTime _origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return _origin.AddSeconds(time);
        }
    }
}
