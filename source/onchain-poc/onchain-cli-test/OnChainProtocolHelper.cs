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

namespace UniversalIdentity.OnChain.Cli.Test
{
    public static class OnChainProtocolHelper
    {
        public static Contract GetSystemContract(Web3 web3)
        {
            var abi = TestConstants.SystemContractAbi;
            var contractAddress = TestConstants.TestSystemContractAddress;
            var contract = web3.Eth.GetContract(abi, contractAddress);
            return contract;
        }

        public static Contract GetIdentityContract(Web3 web3)
        {
            var abi = TestConstants.IdentityContractAbi;
            var contractAddress = TestConstants.TestIdentityContractAddress;
            var contract = web3.Eth.GetContract(abi, contractAddress);
            return contract;
        }

        [Function("createIdentity", "address")]
        public class CreateIdentityFunction : FunctionMessage
        {
            //[Parameter("enum Identity.ValueLevel", "_valueLevel", 1)]
            [Parameter("uint8", "_valueLevel", 1)]
            public int ValueLevel { get; set; }
        }

        [Event("IdentityCreated")]
        public class IdentityCreatedEventDTO : IEventDTO
        {
            [Parameter("address", "identityAddress", 1, false)]
            public string IdentityAddress { get; set; }
        }

        [Event("PropertySet")]
        public class PropertySetEventDTO : IEventDTO
        {
            [Parameter("uint256", "nameHash", 1, false)]
            public BigInteger NameHash { get; set; }
            [Parameter("string", "value", 2, false)]
            public string Value { get; set; }
        }

        public class SystemContractDeploymentMessage : ContractDeploymentMessage
        {
            public SystemContractDeploymentMessage() : base(TestConstants.SystemContractByteCode){}

            //[Parameter("uint256", "totalSupply")]
            //public BigInteger TotalSupply { get; set; }
        }

        public enum ValueLevel
        {
            Unspecified = 0,
            None = 1,
            VeryLow = 2,
            Low = 3,
            MediumLow = 4,
            Medium = 5,
            MediumHigh = 6,
            High = 7,
            VeryHigh = 8,
        }
    }
}
