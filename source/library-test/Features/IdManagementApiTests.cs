using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Test.Infra;
using UniversalIdentity.Library;
using UniversalIdentity.Library.Runtime;
using FluentAssertions;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Library.Cryptography;
using System.IO;
using System;

namespace UniversalIdentity.Library.Test.Features;

/// Super-Feature:
/// - Identity management API
///     The id. management API is exposed on the IdBoxRuntimeService object, it encapsulates
///     the functionality exposed by an Identity Box at runtime
///     - It includes all of its valid states: Empty, Seed, Changed, Saved
///     - It also contains state information about changes not yet committed to storage, such 
///     as:
///         - Changes to the core identity, including control changes
///         - as well as changes to extended identity, including changes to identity information.
///     - Change operations applied to an IdBox, can have multiple sources: 1. User on the device, 
///     2. Synced from another operated instance of the IdBox (Identity sync protocol), 3. Operator of a 
///     connection identity's  original identity (Identity interchange protocol), 4. A parallel operator
///     syncing changes accross operators (Identity replication protocol) 

public class IdManagementApiTests : TestsBase
{
    public IdManagementApiTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact] /// (Sub)-Feature: Identity creation.
    public async void IdBoxCreationTest()
    {
        using (var testContext = new IdBoxTestContext(nameof(IdBoxCreationTest), this))
        {
            var (identityService, callContext) = testContext.CreateAndVerifyEmptyTestIdBoxThenGetParameters();
            var coreIdentity = identityService.CreateAndVerifyCoreIdentity();
        }
    }

    [Fact] /// (Sub)-Feature: Identity creation - User can create identity box and identity.
    public async void CoreIdentityInfoCrudTest()
    {
        using (var testContext = new IdBoxTestContext(nameof(CoreIdentityInfoCrudTest), this))
        {
            var (identityClient, callContext, coreIdentity) = testContext.CreateAndVerifyTestCoreIdentityThenGetParameters();
            identityClient.SetPropertyInfo(IdentityInfo.Name, "SomeName").Should().BeOfType<string>().Should().BeEquivalentTo<string>("SomeName");
            identityClient.SetPropertyInfo(IdentityInfo.Image, testContext.GenerateProfileImage()).Should().NotBeNull();
            identityClient.DeletePropertyInfo(IdentityInfo.Image);
            identityClient.GetPropertyInfo(IdentityInfo.Image).Should().BeNull();
        }
    } 
}

public static class IdentityClientTestContextExtensions
{
    public static (IdBoxRuntimeService, CallContext) CreateAndVerifyTestIdBoxThenGetParameters(this IdBoxTestContext testContext)
    {
        var (idBoxRuntimeService, callContext) = testContext.CreateAndVerifyEmptyTestIdBoxThenGetParameters();
        var mainIdentity = idBoxRuntimeService.CreateAndVerifyCoreIdentity();
        return (idBoxRuntimeService, callContext);
    }
    
    public static (IdBoxRuntimeService, CallContext) CreateAndVerifyEmptyTestIdBoxThenGetParameters(this IdBoxTestContext testContext)
    {
        var idBoxService = testContext.IdBoxService;
        var callContext = testContext.CallContext;

        IdBoxStorageService identityBox = idBoxService.IdBoxStorageService;
        identityBox.Should().BeNull();
        var tempPath = testContext.GetTempPath();
        tempPath = Path.Combine(tempPath, "id-box");
        if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
        identityBox = idBoxService.CreateIdentityBox(tempPath);
        identityBox.Should().NotBeNull("Identity-box should be created at this point.");

        return (idBoxService, callContext);
    }

    public static IdentityRuntime CreateAndVerifyCoreIdentity(this IdBoxRuntimeService idBox)
    {
        var identityBox = idBox.IdBoxStorage;
        identityBox.Should().NotBeNull("Identity-box should be created before identity creation.");
        IdentityRuntime identity = idBox.MainIdentity;
        identity.Should().BeNull("Core identity is not created as this point.");
        var ethKey = new EthKey();
        identity = idBox.CreateMainIdentity(ethKey);
        identity.Should().NotBeNull("Core identity should be created as this point.");
        return identity;
    }

    public static (IdBoxRuntime, CallContext, IdentityRuntime) CreateAndVerifyTestCoreIdentityThenGetParameters(this IdBoxTestContext testContext)
    {
        throw new NotImplementedException();
    }
}