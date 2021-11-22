using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace UniversalIdentity.Library.Test.Infra;

public class TestInfraTests : TestsBase
{
    public TestInfraTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact]
    public void EmptyTest()
    {
        // Theoretically this should pass even if all other functionality is broken
    }

    [Fact]
    public void EmptyTestContextTest()
    {
        using (var testContext = new TestContext(nameof(EmptyTestContextTest), this))
        {
            testContext.Uuid.Should().NotBeEmpty();
            testContext.MethodName.Should().Be(nameof(EmptyTestContextTest));
            // Theoretically this should pass if contexts work, even if all other functionality is broken
        }
    }
}