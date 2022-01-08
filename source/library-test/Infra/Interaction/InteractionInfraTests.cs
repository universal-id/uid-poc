using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Storage;
using FluentAssertions;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.IO;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using UniversalIdentity.Library.Cryptography;
using UniversalIdentity.Library.Communication;
using PeerTalk;
using UniversalIdentity.Library.Interaction;

namespace UniversalIdentity.Library.Test.Infra.Interaction;

public class InteractionInfraTests : TestsBase
{
    public InteractionInfraTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact] /// Developer can programmatically:
    /// - Create and initialize an interaction service object
    public void CreateInteractionServiceTest()
    {
        using (var testContext = new TestContext(nameof(CreateInteractionServiceTest), this))
        {
            var tempPath = Path.GetTempPath();
            var uniqueString = testContext.Uuid.ToString();
            tempPath = Path.Combine(tempPath, uniqueString);
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);

            var interactionService = new InteractionService(tempPath);
            var interactionServicePath = System.IO.Path.Combine(interactionService.Path, InteractionService.KeysFolder);
            Directory.Exists(interactionServicePath).Should().BeFalse();

            interactionService.Init();
            Directory.Exists(interactionServicePath).Should().BeTrue();
        }
    }

    [Fact] /// Developer can programmatically
    /// - Cannot programmatically create a non-existent interaction service
    public void EmptyInteractionServiceTest()
    {
        using (var testContext = new TestContext(nameof(EmptyInteractionServiceTest), this))
        {
            try { new InteractionService(null); Assert.True(false); }
            catch {}

            var interactionService = new InteractionService("Some invalid string.");

            try { interactionService.Init(); Assert.True(false); }
            catch {}
        }   
    }

    [Fact] /// Developer can programmatically:
    /// - Create and store a new key
    /// - Access and use key directly from the interaction service implementation
    public void InteractionServiceCreateAndAccessKeyTest()
    {
        using (var testContext = new InteractionTestContext(nameof(InteractionServiceCreateAndAccessKeyTest), this))
        {
            var interactionService = testContext.InteractionService;

            var key = interactionService.CreateAndStoreNewKey();

            key.Should().NotBeNull();
            var keyIdentifier = key.GetIdentifier();
            testContext.Info($"Key identifier: {keyIdentifier}");
            keyIdentifier.Should().NotBeNullOrEmpty();
            key.IsPublic.Should().BeTrue();
            key.GetPublicIdentifier().Should().NotBeNullOrEmpty();

            interactionService.IsKeyStored(keyIdentifier).Should().BeTrue();
            var privateEthKey = interactionService.GetStoredPrivateKey(keyIdentifier);
            privateEthKey.Should().NotBeNull();
            privateEthKey.IsPublic.Should().BeFalse();
            var privateKey = privateEthKey.GetPrivateKey();
            testContext.Info($"Private key: {privateKey}");
            privateKey.Should().NotBeNullOrEmpty();

            interactionService.GetStoredKeyIdentifiers().Should().Contain(keyIdentifier);
            var publicEthKey = interactionService.GetStoredKey(keyIdentifier);
            publicEthKey.Should().NotBeNull();
            publicEthKey.IsPublic.Should().BeTrue();
            var publicKey = publicEthKey.GetPublicKey();
            testContext.Info($"Public key: {publicKey}");
            publicKey.Should().NotBeNullOrEmpty();
        }   
    }
}