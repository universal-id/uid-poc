using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Test.Infra;
using UniversalIdentity.Library;
using UniversalIdentity.Library.Runtime;
using FluentAssertions;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Library.Cryptography;
using System;

namespace UniversalIdentity.Library.Test.Features;

/// Super-Feature:
/// - Identity management API - See /engineering/source/dotnet/library-test/Features/IdManagementApiTests.cs
public class IdManagementApiNegativeTests : TestsBase
{
    public IdManagementApiNegativeTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact] /// (Sub)-Feature: Identity creation.
    public async void GetEmptyIdBoxRuntimeNegativeTest()
    {
        using (var testContext = new IdBoxTestContext(nameof(GetEmptyIdBoxRuntimeNegativeTest), this))
        {
            var (identityService, callContext) = testContext.CreateAndVerifyEmptyTestIdBoxThenGetParameters();
            try
            {
                var idBoxRuntime = identityService.GetIdBoxRuntime();
                Assert.False(true, "Expected an exception when accessing an empty runtime object.");
            }
            catch (Exception exception) 
            {
                testContext.Info($"Expected exception occurred, message:'{exception.Message}'");
            }
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