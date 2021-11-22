using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Test.Infra;
using UniversalIdentity.Library.Cryptography;
using FluentAssertions;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System;

namespace UniversalIdentity.Library.Test
{
    /// Specialized functionality: Cryptography
    public class CryptographyTests : TestsBase
    {
        public CryptographyTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

        [Fact] /// Core developers can perform Ethereum cryptography operations against specialized library: key, sign, verify, etc.
        public void EthereumCryptographyTest()
        {
            using (var testContext = new TestContext(nameof(EthereumCryptographyTest), this))
            {
                var someDataString = "{ 1, 2, \"some other data\" }";
                testContext.Info($"Data string: '{someDataString}'");
                byte[] someData = System.Text.Encoding.UTF8.GetBytes(someDataString);
                byte[] hash = Sha3Keccack.Current.CalculateHash(someData);
                hash.Should().NotBeNull();
                hash.Should().NotBeEmpty();
                testContext.Info($"Data hash: {hash.ToHex(true)}");

                var ethECKey = EthECKey.GenerateKey();
                string publicAddress = ethECKey.GetPublicAddress();
                publicAddress.Should().StartWith("0x");
                testContext.Info($"Public address: {publicAddress}");

                byte[] publicKey = ethECKey.GetPubKey();
                publicKey.Should().NotBeNull();
                publicKey.Should().NotBeEmpty();
                testContext.Info($"Public key hex: {publicKey.ToHex()}");

                byte[] privateKeyBytes = ethECKey.GetPrivateKeyAsBytes();
                privateKeyBytes.Should().NotBeNull();
                privateKeyBytes.Should().NotBeEmpty();
                testContext.Info($"Private key hex: {privateKeyBytes.ToHex()}");

                string privateKeyString = ethECKey.GetPrivateKey();
                privateKeyString.Should().NotBeNullOrEmpty();
                testContext.Info($"Private key string: {privateKeyString}");

                var signature = ethECKey.SignAndCalculateV(hash);
                var signature64ByteArray = signature.To64ByteArray();
                signature64ByteArray.Should().NotBeNull();
                signature64ByteArray.Should().NotBeEmpty();
                testContext.Info($"Signature 64 byte array: {signature64ByteArray.ToHex(true)}");
                var stringSignature = EthECDSASignature.CreateStringSignature(signature);
                stringSignature.Should().NotBeNullOrEmpty();
                testContext.Info($"String signature: {stringSignature}");

                var rBigInt = signature.R.ToHex().HexToBigInteger(false);
                Console.WriteLine($"Signature R: {rBigInt}");
                var rBigIntBouncyCastle = new Org.BouncyCastle.Math.BigInteger(signature.R);
                Console.WriteLine($"Signature R*: {rBigIntBouncyCastle}");
                rBigInt.ToString().Equals(rBigIntBouncyCastle.ToString());

                var sBigInt = signature.S.ToHex().HexToBigInteger(false);
                Console.WriteLine($"Signature S: {sBigInt}");
                var sBigIntBouncyCastle = new Org.BouncyCastle.Math.BigInteger(signature.S);
                Console.WriteLine($"Signature S*: {sBigIntBouncyCastle}");
                sBigInt.ToString().Equals(sBigIntBouncyCastle.ToString());

                var vBigInt = signature.V.ToHex().HexToBigInteger(false);
                Console.WriteLine($"Signature V: {vBigInt}");
                var vBigIntBouncyCastle = new Org.BouncyCastle.Math.BigInteger(signature.V);
                Console.WriteLine($"Signature V*: {vBigIntBouncyCastle}");
                vBigInt.ToString().Equals(vBigIntBouncyCastle.ToString());

                var isVerified = ethECKey.Verify(hash, signature);
                isVerified.Should().BeTrue();
                testContext.Info($"Is hash signature verified: {isVerified}");

                var ethECKeyFromPublic = new EthECKey(publicKey, false);
                string publicKeyPublicAddress = ethECKey.GetPublicAddress();
                publicKeyPublicAddress.Should().StartWith("0x");
                publicKeyPublicAddress.Should().BeEquivalentTo(publicAddress);
                testContext.Info($"Public key public address: {publicKeyPublicAddress}");

                var isVerifiedFromPublicKey = ethECKeyFromPublic.Verify(hash, signature);
                isVerifiedFromPublicKey.Should().BeTrue();
                testContext.Info($"Is hash signature verified by public key: {isVerifiedFromPublicKey}");

                var tamperedDataString = " " + someDataString;
                testContext.Info($"Tampered data string: '{tamperedDataString}'");
                byte[] tamperedData = System.Text.Encoding.UTF8.GetBytes(tamperedDataString);
                byte[] tamperedHash = Sha3Keccack.Current.CalculateHash(tamperedData);
                testContext.Info($"Tampered data hash: {tamperedHash.ToHex(true)}");
                var isTamperedHashVerified = ethECKeyFromPublic.Verify(tamperedHash, signature);
                isTamperedHashVerified.Should().BeFalse();
                testContext.Info($"Is tampered hash signature verified: {isTamperedHashVerified}");
            }
        }
        
        [Fact] /// App developer can use our EthKey abstraction for cryptography operations: key, sign, verify, etc.
        public void EthKeyTest()
        {
            using (var testContext = new TestContext(nameof(EthKeyTest), this))
            {
                var someDataString = "{ 1, 2, \"some other data\" }";
                testContext.Info($"Data string: '{someDataString}'");
                byte[] someData = System.Text.Encoding.UTF8.GetBytes(someDataString);
                byte[] hash = Sha3Keccack.Current.CalculateHash(someData);
                hash.Should().NotBeNull();
                hash.Should().NotBeEmpty();
                testContext.Info($"Data hash: {hash.ToHex(true)}");

                var key = new EthKey();
                string publicKey = key.GetPublicKey();
                publicKey.Should().StartWith("0x");
                testContext.Info($"Public key: {publicKey}");

                string publicAddress = key.GetPublicAddress();
                publicAddress.Should().StartWith("0x");
                testContext.Info($"Public address: {publicAddress}");

                byte[] publicKeyBytes = key.GetPublicKeyBytes();
                publicKeyBytes.Should().NotBeNull();
                publicKeyBytes.Should().NotBeEmpty();

                byte[] privateKeyBytes = key.GetPrivateKeyBytes();
                privateKeyBytes.Should().NotBeNull();
                privateKeyBytes.Should().NotBeEmpty();

                string privateKey = key.GetPrivateKey();
                privateKey.Should().NotBeNull();
                privateKey.Should().NotBeEmpty();
                testContext.Info($"Private key: {privateKey}");

                string signature = key.Sign(hash);
                testContext.Info($"Signature: {signature}");

                bool isVerified = key.Verify(hash, signature);
                isVerified.Should().BeTrue();
                testContext.Info($"Is hash signature verified: {isVerified}");

                var keyFromPublic = new EthKey(publicKey);
                string publicKeyFromPublic = keyFromPublic.GetPublicKey();
                publicKeyFromPublic.Should().StartWith("0x");
                publicKeyFromPublic.Should().BeEquivalentTo(publicKey);
                testContext.Info($"Public key public address: {publicKeyFromPublic}");

                var isVerifiedFromPublicKey = keyFromPublic.Verify(hash, signature);
                isVerifiedFromPublicKey.Should().BeTrue();
                testContext.Info($"Is hash signature verified by public key: {isVerifiedFromPublicKey}");

                var tamperedDataString = " " + someDataString;
                testContext.Info($"Tampered data string: '{tamperedDataString}'");
                byte[] tamperedData = System.Text.Encoding.UTF8.GetBytes(tamperedDataString);
                byte[] tamperedHash = Sha3Keccack.Current.CalculateHash(tamperedData);
                testContext.Info($"Tampered data hash: {tamperedHash.ToHex(true)}");
                var isTamperedHashVerified = keyFromPublic.Verify(tamperedHash, signature);
                isTamperedHashVerified.Should().BeFalse();
                testContext.Info($"Is tampered hash signature verified: {isTamperedHashVerified}");
            }
        }

        // [Fact]
        // public void MnemonicKeyGenerationTest()
        // {
        //     using (var testContext = new TestContext(nameof(MnemonicKeyGenerationTest), this))
        //     {
        //         var wallet = new Nethereum.HdWallet.Wallet(TestConstants.TestMnemonic, null);
        //         for (int i = 0; i < 10; i++)
        //         {
        //             var account = wallet.GetAccount(i); 
        //             Console.WriteLine($"Account index : {i} - Address : {account.Address} - Private key : {account.PrivateKey}");
        //         }
        //     }
        // }
    }
}