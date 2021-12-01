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

namespace UniversalIdentity.Library.Test.Infra.Storage;

public class StorageInfraTests : TestsBase
{
    public StorageInfraTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact] /// Developer can programmatically:
    /// - Programmatically create empty storage object
    /// - Not create a non-existent storage object
    /// - All API calls dealing with empty or non-existent storage throw exception
    public void EmptyIdBoxStorageTest()
    {
        using (var testContext = new TestContext(nameof(EmptyIdBoxStorageTest), this))
        {
            try { new IdBoxStorageService(null); Assert.True(false); }
            catch {}

            var tempPath = Path.GetTempPath();
            var uniqueString = testContext.Uuid.ToString() + "id-box";
            tempPath = Path.Combine(tempPath, uniqueString);
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            var idBoxStorage = new IdBoxStorage(uniqueString);
            idBoxStorage.Repository.Should().NotBeNull("Repository is created under the storage object.");

            try { idBoxStorage.InitializeStorage(); Assert.True(false); }
            catch {}
        }   
    }

    [Fact] /// Developer can programmatically
    /// - Create a seed core identity object (using user and agent service objects with private keys)
    public void IdBoxStorageCreateSeedIdentityTest()
    {
        using (var testContext = new IdBoxStorageTestContext(nameof(IdBoxStorageCreateSeedIdentityTest), this))
        {
            var idBoxStorage = testContext.IdBoxStorage;
            idBoxStorage.InitializeStorage();

            var seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            seedIdentityStorage.Should().NotBeNull();
        }
    }

    [Fact] /// Developer can programmatically
           /// - Create a seed core identity object (using user and agent service objects with private keys)
    public void IdBoxStorageCreateAndSaveSeedIdentityTest()
    {
        using (var testContext = new IdBoxStorageTestContext(nameof(IdBoxStorageCreateSeedIdentityTest), this))
        {
            var idBoxStorage = testContext.IdBoxStorage;
            idBoxStorage.InitializeStorage();

            var seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            seedIdentityStorage.Should().NotBeNull();

            var savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);
            savedSeedIdentityStorage.Should().NotBeNull();
        }
    }


    [Fact] /// Developer can programmatically
           /// - Creates a seed identity as the first primary identity.
    public void IdBoxStorageCreateAndMakeSeedIdentityAsFirstPrimaryIdentityTest()
    {
        using (var testContext = new IdBoxStorageTestContext(nameof(IdBoxStorageCreateSeedIdentityTest), this))
        {
            var idBoxStorage = testContext.IdBoxStorage;
            idBoxStorage.InitializeStorage();

            var seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            seedIdentityStorage.Should().NotBeNull();

            idBoxStorage.PrimaryIdentity = seedIdentityStorage.Identifier;
            idBoxStorage.PrimaryIdentity.Should().Equals(seedIdentityStorage.Identifier);
        }
    }

    [Fact] /// Developer can programmatically
           /// - Updates an existing identity with added key information
    public void UpdatesExistingIdentityWithAddedKeyInformationTest()
    {
        using (var testContext = new IdBoxStorageTestContext(nameof(IdBoxStorageCreateSeedIdentityTest), this))
        {
            IdBoxStorage? idBoxStorage = testContext.IdBoxStorage;
            idBoxStorage.InitializeStorage();

            IdentityStorage? seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            seedIdentityStorage.Should().NotBeNull();

            seedIdentityStorage.Level = ValueLevel.VeryHigh;
            var identityKeysList = new List<KeyStorage>() {
                new KeyStorage(){Identifier = "Identifier1",Level= ValueLevel.High, Created=1,PublicKey=""},
                new KeyStorage(){Identifier = "Identifier2",Level= ValueLevel.Low, Created=1,PublicKey=""},
            };
            seedIdentityStorage.Keys = identityKeysList.ToArray();

            var savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);
            savedSeedIdentityStorage.Should().NotBeNull();
            savedSeedIdentityStorage.Level.Should().Be(ValueLevel.VeryHigh);
            savedSeedIdentityStorage.Keys.Should().HaveCount(2);
        }
    }

    [Fact] /// Developer can programmatically
           /// - Updates an existing identity with identity infon
    public void UpdatesAnExistingIdentityWithIdentityInfo()
    {
        using (var testContext = new IdBoxStorageTestContext(nameof(IdBoxStorageCreateSeedIdentityTest), this))
        {
            IdBoxStorage? idBoxStorage = testContext.IdBoxStorage;
            idBoxStorage.InitializeStorage();

            IdentityStorage? seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            seedIdentityStorage.Should().NotBeNull();

            var info = new List<Info>() {
                new Info(){Key="K1",Value="V1"},
                new Info(){Key="K2",Value="V2"},
            };
            seedIdentityStorage.Info = info.ToArray();

            var savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);
            savedSeedIdentityStorage.Should().NotBeNull();
            savedSeedIdentityStorage.Info.Should().HaveCount(2);
        }
    }

    [Fact] /// Developer can programmatically
           /// - Reads identity information given an identifier
    public void IdBoxStorageCreateAndSaveSeedIdentity2Test()
    {
        using (var testContext = new IdBoxStorageTestContext(nameof(IdBoxStorageCreateSeedIdentityTest), this))
        {
            IdBoxStorage? idBoxStorage = testContext.IdBoxStorage;
            idBoxStorage.InitializeStorage();

            IdentityStorage? seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            seedIdentityStorage.Should().NotBeNull();

            IdentityStorage? savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);
            savedSeedIdentityStorage.Should().NotBeNull();

            IdentityStorage? identityStorage = idBoxStorage.GetIdentity(savedSeedIdentityStorage.Identifier);
            identityStorage.Should().NotBeNull();
            idBoxStorage.ToJson().Should().Equals(savedSeedIdentityStorage.ToJson());
        }
    }
}