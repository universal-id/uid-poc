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
using Nethereum.RPC.Eth.DTOs;
using UniversalIdentity.Library.Test.Infra;

namespace UniversalIdentity.Library.Test.Specialized.Ethereum
{
    public class EthereumTests : TestsBase
    {
        public EthereumTests(ITestOutputHelper outputHelper) : base(outputHelper) {}

        [Fact]
        [Trait("Manual", "true")]
        public async void TestAccountTest()
        {
            using (var testContext = new TestContext(nameof(TestAccountTest), this))
            {
                var account = ContractHelper.GetAccount();
                var web3 = ContractHelper.GetWeb3(account);
                var balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
                Console.WriteLine($"Balance in Wei: {balance.Value}");

                var etherAmount = Web3.Convert.FromWei(balance.Value);
                Console.WriteLine($"Balance in Ether: {etherAmount}");
            }
        }
    }
}
