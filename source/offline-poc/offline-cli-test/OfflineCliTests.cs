using FluentAssertions;
using UniversalIdentity.Cli;
using UniversalIdentity.Cli.Models;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Cli.Test;
using Xunit.Abstractions;
using UniversalIdentity.Library.Test.Infra;
using System;
using UniversalIdentity.Library.Communication;
using UniversalIdentity.Cli.Offline;

namespace UniversalIdentity.Offline.Cli.Test
{
    public class OfflineCliTests : TestsBase
    {
        public OfflineCliTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

        [Fact]
        public async Task CliClientScenariosTest()
        {
            using (var testContext = new CliTestContext(nameof(RespondToNonExistentBeaconTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
                cliHandlers.CreateHandler(path);
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox 
                cliHandlers.ListHandler("--detail"); // idbox ids list --detail
                cliHandlers.ListHandler("--summary"); // idbox ids list --summary
                string identifier = cliHandlers.CreateSeedIdentity(); // idbox ids createSeed
                cliHandlers.ListHandler("--detail"); // idbox ids list --detail
                cliHandlers.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
                cliHandlers.SetAsPrimaryHandler(); // idbox ids setPrimary
                cliHandlers.GetPrimaryHandler(); // idbox ids getprimary
                cliHandlers.GetSelectedIdentityHandler("--summary"); // idbox id get --summary
                cliHandlers.SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter creates a new identity box
        /// </summary>
        public void CreateNewIdBoxTest()
        {
            using (var testContext = new CliTestContext(nameof(CreateNewIdBoxTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
                cliHandlers.CreateHandler(path); // idbox box create c:\idbox
                Directory.Exists(path).Should().BeTrue();
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter opens an identity box
        /// </summary>
        public async void OpenIdboxTest()
        {
            using (var testContext = new CliTestContext(nameof(OpenIdboxTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(testContext.CliPath);
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox 
                string fileName = System.IO.Path.Combine(testContext.CliPath, @"State.Json");
                File.Exists(fileName).Should().BeTrue();

                string jsonString = File.ReadAllText(fileName);
                jsonString.Should().NotBeNull();

                State? result = JsonSerializer.Deserialize<State>(jsonString);
                result.Should().NotBeNull();
                result.Path.Should().Be(path);
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter lists identities
        /// </summary>
        public async Task ListsIdentitiesTest()
        {
            using (var testContext = new CliTestContext(nameof(RespondToNonExistentBeaconTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
                cliHandlers.CreateHandler(path); // idbox box create c:\idbox
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox 
                cliHandlers.ListHandler("--detail"); // idbox ids list --detail
                cliHandlers.ListHandler("--summary"); //  idbox ids list --summary true
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter create and select Identity
        /// </summary>
        public async Task CreateAndSelectIdentityTest()
        {
            using (var testContext = new CliTestContext(nameof(RespondToNonExistentBeaconTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(testContext.CliPath);
                cliHandlers.CreateHandler(path); // idbox box create c:\idbox
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox 
                string identifier = cliHandlers.CreateSeedIdentity(); // idbox ids createSeed
                cliHandlers.ListHandler("--detail"); // idbox ids list --detail
                cliHandlers.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6

                string fileName = System.IO.Path.Combine(testContext.CliPath, @"State.Json");
                string jsonString = File.ReadAllText(fileName);
                jsonString.Should().NotBeNull();

                State result = JsonSerializer.Deserialize<State>(jsonString);

                result.Should().NotBeNull();
                result.Path.Should().Be(path);
                result.SelectedIdentity.Should().Be(identifier);
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter select and SetAsPrimary Identity
        /// </summary>
        public async Task SetAsPrimaryIdentityTest()
        {
            using (var testContext = new CliTestContext(nameof(RespondToNonExistentBeaconTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
                cliHandlers.CreateHandler(path); // idbox box create c:\idbox
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox 
                string identifier = cliHandlers.CreateSeedIdentity(); // idbox ids createSeed
                cliHandlers.ListHandler("--detail"); // idbox ids list --detail
                cliHandlers.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
                cliHandlers.SetAsPrimaryHandler(); // idbox ids setPrimary
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter accesses primary identity
        /// </summary>
        public async Task GetPrimaryIdentityTest()
        {
            using (var testContext = new CliTestContext(nameof(RespondToNonExistentBeaconTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
                cliHandlers.CreateHandler(path); // idbox box create c:\idbox
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox 
                string identifier = cliHandlers.CreateSeedIdentity(); // idbox ids createSeed
                cliHandlers.ListHandler("--detail"); // idbox ids list --detail
                cliHandlers.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
                cliHandlers.SetAsPrimaryHandler(); // idbox ids setPrimary
                cliHandlers.GetPrimaryHandler(); // idbox ids getprimary
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter accesses primary identity
        /// </summary>
        public async Task SetInfoTest()
        {
            using (var testContext = new CliTestContext(nameof(RespondToNonExistentBeaconTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
                cliHandlers.CreateHandler(path); // idbox box create c:\idbox
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox 
                string identifier = cliHandlers.CreateSeedIdentity(); // idbox ids createSeed
                cliHandlers.ListHandler("--detail"); // idbox ids list --detail
                cliHandlers.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
                cliHandlers.SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter activates a beacon
        /// </summary>
        public async Task ActivateBeaconTest()
        {
            using (var testContext = new CliTestContext(nameof(ActivateBeaconTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
                cliHandlers.CreateHandler(path);
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox 
                string identifier = cliHandlers.CreateSeedIdentity(); // idbox ids createSeed
                cliHandlers.SetAsPrimaryHandler(); // idbox ids setPrimary


                var identityStorage = new IdBoxStorage(path);
                 identifier = identityStorage.PrimaryIdentity;
                identifier.Should().NotBeNull();

                cliHandlers.ActivateBeaconHandler(path, identifier!); // idbox beacon activate
                
                var idBoxService = State.IdBoxService;
                idBoxService.Should().NotBeNull();
                //var idBoxStorage = idBoxService.Storage;
                var communicationService = idBoxService.Communication;
                communicationService.Should().NotBeNull();
                using (communicationService)
                {
                    BeaconProtocol? beaconProtocol = communicationService.BeaconProtocol;
                    beaconProtocol.Should().NotBeNull();
                    beaconProtocol.LocalBeacon.Should().NotBeNull();
                }
            }
        }

        [Fact]
        /// <summary>
        /// Early adopter responds to non-existent beacon
        /// </summary>
        public async Task RespondToNonExistentBeaconTest()
        {
            using (var testContext = new CliTestContext(nameof(RespondToNonExistentBeaconTest), this))
            {
                string path = testContext.IdBoxPath;
                var cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
                cliHandlers.CreateHandler(path);
                await cliHandlers.OpenHandlerAsync(path); // idbox box open c:\idbox
                cliHandlers.CreateSeedIdentityHandler(); // idbox ids create-seed
                cliHandlers.SetAsPrimaryHandler(); // idbox ids set-primary

                var identityStorage = new IdBoxStorage(path);
                var identifier = identityStorage.PrimaryIdentity;
                identifier.Should().NotBeNull();
        
                var nonExistentAddress = "/ip4/127.0.0.1/tcp/54576/ipfs/QmVKV6NTFiBnFfi73nN74m9XCNg5xUcy2VtmLipnHQXe8X";
                try
                {
                    cliHandlers.BeaconRespondHandler(nonExistentAddress, identifier!); // idbox beacon respond 'https://127.0.0.1/abcd'
                    Assert.True(false, "Expected beacon response to fail.");
                }
                catch (AggregateException) {}

                // var idBoxService = State.IdBoxService;
                // idBoxService.Should().NotBeNull();
                // //var idBoxStorage = idBoxService.Storage;
                // var communicationService = idBoxService.Communication;
                // communicationService.Should().NotBeNull();
                // using (communicationService)
                // {
                //     var beaconProtocol = communicationService.BeaconProtocol;
                //     beaconProtocol.Should().NotBeNull();
                //     beaconProtocol.LocalBeacon.Should().NotBeNull();
                // }
            }
        }
    }
}