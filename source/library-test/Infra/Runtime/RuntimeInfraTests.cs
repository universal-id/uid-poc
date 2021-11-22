using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Storage;
using FluentAssertions;
using UniversalIdentity.Library.Runtime;
using System.IO;

namespace UniversalIdentity.Library.Test.Infra.Runtime;

public class RuntimeInfraTests : TestsBase
{
    public RuntimeInfraTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact]
    public void EmptyIdBoxRuntimeServiceTest()
    {
        using (var testContext = new TestContext(nameof(EmptyIdBoxRuntimeServiceTest), this))
        {
            var tempPath = Path.GetTempPath();
            var uniqueString = testContext.Uuid.ToString() + "id-box";
            tempPath = Path.Combine(tempPath, uniqueString);
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            var idBoxRuntimeService = new IdBoxRuntimeService();
            idBoxRuntimeService.IdBoxStorageService.Should().BeNull("Storage service has not yet been attached.");
        }   
    }
}