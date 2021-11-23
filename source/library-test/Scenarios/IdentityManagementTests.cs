using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Test.Infra;
using UniversalIdentity.Library;
using UniversalIdentity.Library.Runtime;
using UniversalIdentity.Library.Test.Features;
using FluentAssertions;

namespace UniversalIdentity.Library.Test.Scenarios;

/// Super-Scenarios:
/// - Identity management for app developers:
///     - App developer can build an app that sets up identity and manages information.
/// - Identity management for users:
///     - User can set up identity and manage information.
public class IdentityManagementTests : TestsBase
{
    public IdentityManagementTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    // [Fact] /// (Sub)-Scenario: Identity creation - User can create identity box and identity.
    // public async void UserCanSetupIdentityTest()
    // {
    //     using (var testContext = new IdBoxTestContext(nameof(UserCanSetupIdentityTest), this))
    //     {
    //         var (idBoxService, callContext) = testContext.CreateAndVerifyEmptyTestIdBoxThenGetParameters();
    //         var coreIdentity = idBoxService.CreateAndVerifyCoreIdentity();
    //     }
    // }

    // [Fact] /// (Sub)-Scenario: Identity creation - User can create identity box and identity.
    // public async void UserCanCrudCoreIdentityInformationTest()
    // {
    //     using (var testContext = new IdBoxTestContext(nameof(UserCanCrudCoreIdentityInformationTest), this))
    //     {
    //         var (identityClient, callContext, coreIdentity) = testContext.CreateAndVerifyTestCoreIdentityThenGetParameters();
    //         identityClient.SetPropertyInfo(IdentityInfo.Name, "SomeName").Should().BeOfType<string>().Should().BeEquivalentTo<string>("SomeName");
    //         identityClient.SetPropertyInfo(IdentityInfo.Image, testContext.GenerateProfileImage()).Should().NotBeNull();
    //         identityClient.DeletePropertyInfo(IdentityInfo.Image);
    //         identityClient.GetPropertyInfo(IdentityInfo.Image).Should().BeNull();
    //     }
    // } 
}