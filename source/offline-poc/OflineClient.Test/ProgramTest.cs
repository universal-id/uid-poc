using FluentAssertions;
using OfflineClient;
using OfflineClient.Models;
using System.IO;
using System.Text.Json;
using Xunit;

namespace OflineClient.Test
{
    public class ProgramTest
    {
        [Fact]
        public void CLIclientScenariosTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path);
            Program.OpenHandlerAsync(path).GetAwaiter().GetResult(); // idbox box open c:\idbox 
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.ListHandler("--summary"); //  idbox ids list --summary
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
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
        public void OpenIdboxTest()
        {
            string path = Path.GetTempPath();
            Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
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
        public void ListsIdentitiesTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path); // idbox box create c:\idbox
            Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.ListHandler("--summary"); //  idbox ids list --summary true
        }

        [Fact]
        /// <summary>
        /// Early adopter create and select Identity
        /// </summary>
        public void CreateAndSelectIdentityTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path); // idbox box create c:\idbox
            Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6

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
        public void SetAsPrimaryIdentityTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path);
            Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            Program.SetAsPrimaryHandler(); // idbox ids setPrimary
        }

        [Fact]
        /// <summary>
        /// Early adopter accesses primary identity
        /// </summary>
        public void GetPrimaryIdentityTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path);
            Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            Program.SetAsPrimaryHandler(); // idbox ids setPrimary
            Program.GetPrimaryHandler(); // idbox ids getprimary
        }

        [Fact]
        /// <summary>
        /// Early adopter accesses primary identity
        /// </summary>
        public void SetInfoTest()
        {
            string path = Path.GetTempPath();
            Program.CreateHandler(path);
            Program.OpenHandlerAsync(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler("--detail"); // idbox ids list --detail
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            Program.SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara
        }
    }
}