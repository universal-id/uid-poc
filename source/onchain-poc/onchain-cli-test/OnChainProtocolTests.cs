
// #define MANUAL

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
using UniversalIdentity.Library.Test.Specialized.Ethereum;
using UniversalIdentity.Library.Cryptography;

namespace UniversalIdentity.OnChain.Cli.Test
{
    public class OnChainProtocolTests : TestsBase
    {
        public OnChainProtocolTests(ITestOutputHelper outputHelper) : base(outputHelper) {}

        #if MANUAL
        [Fact]
        #endif
        [Trait("Manual", "true")]
        public async void DeploySystemTest()
        {
            // https://docs.nethereum.com/en/latest/nethereum-smartcontrats-gettingstarted/
            using (var testContext = new TestContext(nameof(DeploySystemTest), this))
            {
                var account = ContractHelper.GetAccount();
                var web3 = ContractHelper.GetWeb3(account);

                var contractByteCode = TestConstants.SystemContractByteCode;
                var contractAbi = TestConstants.SystemContractAbi;
                var senderAddress = account.Address;
                var estimateGas = await web3.Eth.DeployContract.EstimateGasAsync(contractAbi, contractByteCode, senderAddress);

                var deploymentMessage = new OnChainProtocolHelper.SystemContractDeploymentMessage();
                var deploymentHandler = web3.Eth.GetContractDeploymentHandler<OnChainProtocolHelper.SystemContractDeploymentMessage>();
                var receipt = await deploymentHandler.SendRequestAndWaitForReceiptAsync(deploymentMessage);
                Console.WriteLine("Contract deployed at address: " + receipt.ContractAddress);
            }
        }

        #if MANUAL
        [Fact]
        #endif
        [Trait("Manual", "true")]
        public async void CreateIdentityTest()
        {
            using (var testContext = new TestContext(nameof(CreateIdentityTest), this))
            {
                var account = ContractHelper.GetAccount();
                var web3 = ContractHelper.GetWeb3(account);
                var senderAddress = account.Address;
                var contract = OnChainProtocolHelper.GetSystemContract(web3);

                var createIdentityHandler = web3.Eth.GetContractTransactionHandler<OnChainProtocolHelper.CreateIdentityFunction>();
                var createIdentityFunction = new OnChainProtocolHelper.CreateIdentityFunction()
                {
                    ValueLevel = (int)OnChainProtocolHelper.ValueLevel.Medium,
                };

                var estimate = await createIdentityHandler.EstimateGasAsync(contract.Address, createIdentityFunction);
                createIdentityFunction.Gas = estimate.Value;
                Console.WriteLine($"Estimated gas: {createIdentityFunction.Gas}");

                var receipt = await createIdentityHandler.SendRequestAndWaitForReceiptAsync(contract.Address, createIdentityFunction);
                Console.WriteLine($"Transaction hash: {receipt.TransactionHash}");
            }
        }

        [Fact]
        [Trait("Manual", "false")]
        public async void EnumerateIdentitiesTest()
        {
            // https://docs.nethereum.com/en/latest/nethereum-events-gettingstarted/
            using (var testContext = new TestContext(nameof(EnumerateIdentitiesTest), this))
            {
                var account = ContractHelper.GetAccount();
                var web3 = ContractHelper.GetWeb3(account);
                var senderAddress = account.Address;
                var contract = OnChainProtocolHelper.GetSystemContract(web3);

                Console.WriteLine($"Contract address: {contract.Address}");

                var identityCreatedEventHandler = web3.Eth.GetEvent<OnChainProtocolHelper.IdentityCreatedEventDTO>(contract.Address);
                var filterAllIdentityCreatedEventsForContract = identityCreatedEventHandler.CreateFilterInput(BlockParameter.CreateEarliest(), BlockParameter.CreateLatest());
                var allIdentityCreatedEventsForContract = await identityCreatedEventHandler.GetAllChangesAsync(filterAllIdentityCreatedEventsForContract);

                foreach (var identityCreatedEvent in allIdentityCreatedEventsForContract)
                {
                    var identityContractAddress = identityCreatedEvent.Event.IdentityAddress;
                    var blockWithTransactions = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new BlockParameter(identityCreatedEvent.Log.BlockNumber));
                    Console.WriteLine($">Created identity address: {identityContractAddress} - Block number: {identityCreatedEvent.Log.BlockNumber} - timestamp: {blockWithTransactions.Timestamp} - time: {ContractHelper.ConvertFromUnixTimestamp(blockWithTransactions.Timestamp)}");
                    

                    var propertySetEventHandler = web3.Eth.GetEvent<OnChainProtocolHelper.PropertySetEventDTO>(identityContractAddress);
                    var filterAllPropertySetEventsForContract = propertySetEventHandler.CreateFilterInput(BlockParameter.CreateEarliest(), BlockParameter.CreateLatest());
                    var allPropertySetEventsForContract = await propertySetEventHandler.GetAllChangesAsync(filterAllPropertySetEventsForContract);
                    
                    foreach (var propertySetEvent in allPropertySetEventsForContract)
                    {
                        Console.WriteLine($">>Property set - name hash: {propertySetEvent.Event.NameHash.ToHex(false, false)} - value: {propertySetEvent.Event.Value}");
                    }
                }
            }
        }

        //[Fact]
        [Trait("Manual", "true")]
        public async void WeaklyTypedDeploySystemTest()
        {
            // https://docs.nethereum.com/en/latest/nethereum-gettingstarted-smartcontracts-untyped/
            using (var testContext = new TestContext(nameof(WeaklyTypedDeploySystemTest), this))
            {
                var account = ContractHelper.GetAccount();
                var web3 = ContractHelper.GetWeb3(account);
                var contractByteCode = TestConstants.SystemContractByteCode;
                var contractAbi = TestConstants.SystemContractAbi;
                var senderAddress = account.Address;
                var estimateGas = await web3.Eth.DeployContract.EstimateGasAsync(contractAbi, contractByteCode, senderAddress);

                var receipt = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(contractAbi,
                    contractByteCode, senderAddress, estimateGas, null);
                Console.WriteLine("Contract deployed at address: " + receipt.ContractAddress);
            }
        }

        #if MANUAL
        [Fact]
        #endif
        [Trait("Manual", "true")]
        public async void WeaklyTypedCreateIdentityTransactionTest()
        {
            // https://docs.nethereum.com/en/latest/contracts/calling-transactions-events/
            using (var testContext = new TestContext(nameof(WeaklyTypedCreateIdentityTransactionTest), this))
            {
                var account = ContractHelper.GetAccount();
                var web3 = ContractHelper.GetWeb3(account);
                var contract = OnChainProtocolHelper.GetSystemContract(web3);

                var createIdentityFunction = contract.GetFunction("createIdentity");
                var receipt = await createIdentityFunction.SendTransactionAndWaitForReceiptAsync(account.Address
                    , new HexBigInteger(10000000) // ContractHelper.GetMaxGas()
                    , new HexBigInteger(0)
                    , null
                    ,  2);
                Console.WriteLine($"Transaction hash: {receipt.TransactionHash}");
            }
        }

        #if MANUAL
        [Fact]
        #endif
        [Trait("Manual", "true")]
        public async void WeaklyTypedSetPropertiesTransactionTest()
        {
            var account = ContractHelper.GetAccount();
            var web3 = ContractHelper.GetWeb3(account);
            var contract = OnChainProtocolHelper.GetIdentityContract(web3);

            var operatorUriName = "http://universal.id/schemas/v0.0/identity-properties#operator-uri";
            var operatorUri = "https://operator.universalid.one";
            var nameName = "http://universal.id/schemas/v0.0/identity-properties#name";
            var name = "Universal Identity";
            var nameNameHash = CryptographyHelper.GetHashString(nameName);
            Console.WriteLine($"Name name hash: {nameNameHash}");
            var operatorUriNameHash = CryptographyHelper.GetHashString(operatorUriName);
            Console.WriteLine($"Operator URI name hash: {operatorUriNameHash}");

            var zeroHash = new HexBigInteger(0);
             var createIdentityFunction = contract.GetFunction("setProperties");
             var receipt = await createIdentityFunction.SendTransactionAndWaitForReceiptAsync(account.Address
                , ContractHelper.GetMaxGas()
                , new HexBigInteger(0)
                , null
                , operatorUriNameHash, nameNameHash, 0, 0, 0 
                , operatorUri, name, string.Empty, string.Empty, string.Empty);
             Console.WriteLine($"Transaction hash: {receipt.TransactionHash}");
        }

        [Fact]
        [Trait("Manual", "false")]
        public void PropertiesHashTest()
        {
            var operatorUriName = "http://universal.id/schemas/v0.0/identity-properties#operator-uri";
            //var operatorUri = "https://operator.universalid.one";
            var nameHash = CryptographyHelper.GetHashString(operatorUriName);
            Console.WriteLine($"Name hash: {nameHash} - name: {operatorUriName}");

            operatorUriName = "http://universal.id/schemas/v0.0/identity-properties#name";
            //operatorUri = "Universal Identity";
            nameHash = CryptographyHelper.GetHashString(operatorUriName);
            Console.WriteLine($"Name hash: {nameHash} - name: {operatorUriName}");
        }

        #if MANUAL
        [Fact]
        #endif
        [Trait("Manual", "true")]
        public async void WeaklyTypedAddAgentKeyTransactionTest()
        {
            var account = ContractHelper.GetAccount();
            var web3 = ContractHelper.GetWeb3(account);
            var contract = OnChainProtocolHelper.GetIdentityContract(web3);

            var agentKey = new EthKey();
            var keyAddress = agentKey.GetPublicAddress();
            Console.WriteLine($"New key with address: {keyAddress}");

            var createIdentityFunction = contract.GetFunction("addAgentKey");
            var receipt = await createIdentityFunction.SendTransactionAndWaitForReceiptAsync(account.Address
                , ContractHelper.GetMaxGas()
                , new HexBigInteger(0)
                , null
                , keyAddress, 0, 0, 2, 2);
            Console.WriteLine($"Transaction hash: {receipt.TransactionHash}");
        }
    }
}
