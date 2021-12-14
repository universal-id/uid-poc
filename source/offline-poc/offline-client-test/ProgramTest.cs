using FluentAssertions;
using OfflineClient;
using OfflineClient.Models;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using UniversalIdentity.Library.Storage;

namespace OfflineClient.Test
{
    public class ProgramTest
    {
        [Fact]
        public async Task CliClientScenariosTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path);
            await Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.ListHandler("--summary"); //  idbox ids list --summary
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3�d4e5f6
            Program.SetAsPrimaryHandler(); // idbox ids setPrimary
            Program.GetPrimaryHandler(); // idbox ids getprimary
            Program.GetSelectedIdentityHandler("--summary"); // idbox id get --summary
            Program.SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara
        }

        [Fact]
        /// <summary>
        /// Early adopter creates a new identity box
        /// </summary>
        public void CreateNewIdboxTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path); // idbox box create c:\idbox
            Directory.Exists(path).Should().BeTrue();
        }

        [Fact]
        /// <summary>
        /// Early adopter opens an identity box
        /// </summary>
        public async void OpenIdboxTest()
        {
            string path = Path.GetTempPath();
            await Program.OpenHandlerAsync(path); // idbox box open c:\idbox
            string fileName = @".\State.Json";
            File.Exists(fileName).Should().BeTrue();

            string jsonString = File.ReadAllText(fileName);
            jsonString.Should().NotBeNull();

            State result = JsonSerializer.Deserialize<State>(jsonString) ?? new State();
            result.Path.Should().Be(path);
        }

        [Fact]
        /// <summary>
        /// Early adopter lists identities
        /// </summary>
        public async Task ListsIdentitiesTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path); // idbox box create c:\idbox
            await Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.ListHandler("--summary"); //  idbox ids list --summary true
        }

        [Fact]
        /// <summary>
        /// Early adopter create and select Identity
        /// </summary>
        public async Task CreateAndSelectIdentityTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path); // idbox box create c:\idbox
            await Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3�d4e5f6

            string fileName = @".\State.Json";
            string jsonString = File.ReadAllText(fileName);
            jsonString.Should().NotBeNull();

            State result = JsonSerializer.Deserialize<State>(jsonString) ?? new State();
            result.Path.Should().Be(path);
            result.SelectedIdentity.Should().Be(identifier);
        }

        [Fact]
        /// <summary>
        /// Early adopter select and SetAsPrimary Identity
        /// </summary>
        public async Task SetAsPrimaryIdentityTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path);
            await Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3�d4e5f6
            Program.SetAsPrimaryHandler(); // idbox ids setPrimary
        }

        [Fact]
        /// <summary>
        /// Early adopter accesses primary identity
        /// </summary>
        public async Task GetPrimaryIdentityTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path);
            await Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3�d4e5f6
            Program.SetAsPrimaryHandler(); // idbox ids setPrimary
            Program.GetPrimaryHandler(); // idbox ids getprimary
        }

        [Fact]
        /// <summary>
        /// Early adopter accesses primary identity
        /// </summary>
        public async Task SetInfoTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path);
            await Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3�d4e5f6
            Program.SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara
        }

        [Fact]
        /// <summary>
        /// Early adopter activates a beacon
        /// </summary>
        public async Task ActivateBeaconTest()
        {
            string path = Path.GetTempPath();
            await Program.OpenHandlerAsync(path); // idbox box open c:\idbox
            Program.CreateSeedIdentityHandler(); // idbox ids create-seed
            Program.SetAsPrimaryHandler(); // idbox ids set-primary

            var identityStorage = new IdBoxStorage(path);
            var identifier = identityStorage.PrimaryIdentity;
            identifier.Should().NotBeNull();
        
            Program.ActivateBeaconHandler(path, identifier!); // idbox beacon activate

            var idBoxService = State.IdBoxService;
            idBoxService.Should().NotBeNull();
            //var idBoxStorage = idBoxService.Storage;
            var communicationService = idBoxService.Communication;
            communicationService.Should().NotBeNull();
            using (communicationService)
            {
                var beaconProtocol = communicationService.BeaconProtocol;
                beaconProtocol.Should().NotBeNull();
                beaconProtocol.LocalBeacon.Should().NotBeNull();
            }
        }
    }
}