using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Storage;
using FluentAssertions;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.IO;
using System;

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

    // [Fact] /// Developer can programmatically
    // /// - Create a seed core identity object (using user and agent service objects with private keys)
    // public void IdBoxStorageCreateAndSaveSeedIdentityTest()
    // {
    //     using (var testContext = new IdBoxStorageTestContext(nameof(IdBoxStorageCreateSeedIdentityTest), this))
    //     {
    //         var idBoxStorage = testContext.IdBoxStorage;
    //         idBoxStorage.InitializeStorage();

    //         var seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
    //         seedIdentityStorage.Should().NotBeNull();

    //         var savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);
    //         savedSeedIdentityStorage.Should().NotBeNull();
    //     }
    // }
}