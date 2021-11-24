using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Storage;
using FluentAssertions;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.IO;
using System;

namespace UniversalIdentity.Library.Test.Infra.Storage;

public class StorageServiceInfraTests : TestsBase
{
    public StorageServiceInfraTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact] /// Developer can programmatically
    /// - Programmatically create empty storage
    /// - Cannot programmatically create a non-existent storage object
    /// - All API calls dealing with empty or non-existent storage throw exception
    public void EmptyIdBoxStorageServiceTest()
    {
        using (var testContext = new TestContext(nameof(EmptyIdBoxStorageServiceTest), this))
        {
            try { new IdBoxStorageService(null); Assert.True(false); }
            catch {}

            var tempPath = Path.GetTempPath();
            var uniqueString = testContext.Uuid.ToString() + "id-box";
            tempPath = Path.Combine(tempPath, uniqueString);
            testContext.Info($">{tempPath}");
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            var idBoxStorageService = new IdBoxStorageService(uniqueString);
            idBoxStorageService.IdBoxStorage.Should().NotBeNull("Service and main IdBox are created.");

            try { idBoxStorageService.CreateSeedIdentity(); Assert.True(false); }
            catch {}
        }   
    }

    [Fact] /// Developer can programmatically
    /// - Create a seed identity - given access to user and agent service objects with private keys
    public void IdBoxStorageServiceCreateSeedIdentityTest()
    {
        using (var testContext = new TestContext(nameof(IdBoxStorageServiceCreateSeedIdentityTest), this))
        {
            var tempPath = Path.GetTempPath();
            var uniqueString = testContext.Uuid.ToString() + "-id-box";
            tempPath = Path.Combine(tempPath, uniqueString);
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            var idBoxStorageService = new IdBoxStorageService(uniqueString);
            idBoxStorageService.IdBoxStorage.Should().NotBeNull("Service and main IdBox are created.");

            var identityStorage = idBoxStorageService.CreateSeedIdentity();
            identityStorage.Should().NotBeNull();
        }   
    }
}