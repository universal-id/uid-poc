using FluentAssertions;
using OfflineClient;
using OfflineClient.Models;
using System;
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
            string path = @"c:\idbox";
            Program.CreateHandler(path);
            Program.OpenHandler(path); // idbox box open c:\idbox 
            Program.ListHandler(detail:true, summary: false); // idbox ids list --detail true
            Program.ListHandler(detail:false, summary:true); //  idbox ids list --summary true
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler(true, false); // idbox ids list --detail true
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            Program.SetAsPrimaryHandler(); // idbox ids setPrimary
            Program.GetPrimaryHandler(); // idbox ids getprimary
            Program.GetSelectedIdentityHandler(true, false); // idbox id get --summary
            Program.SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara
        }

        [Fact]
        /// <summary>
        /// Early adopter creates a new identity box
        /// </summary>
        public void CreateNewIdboxTest()
        {
            string path = @"c:\idbox";
            Program.CreateHandler(path); // idbox box create c:\idbox
            Directory.Exists(path).Should().BeTrue();
        }

        [Fact]
        /// <summary>
        /// Early adopter opens an identity box
        /// </summary>
        public void OpenIdboxTest()
        {
            string path = @"c:\idbox";
            Program.OpenHandler(path); // idbox box open c:\idbox 
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
            string path = @"c:\idbox";
            Program.CreateHandler(path); // idbox box create c:\idbox
            Program.OpenHandler(path); // idbox box open c:\idbox 
            Program.ListHandler(detail: true, summary: false); // idbox ids list --detail true
            Program.ListHandler(detail: false, summary: true); //  idbox ids list --summary true
        }

        [Fact]
        /// <summary>
        /// Early adopter create and select Identity
        /// </summary>
        public void CreateAndSelectIdentityTest()
        {
            string path = @"c:\idbox";
            Program.CreateHandler(path); // idbox box create c:\idbox
            Program.OpenHandler(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler(true, false); // idbox ids list --detail true
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
            string path = @"c:\idbox";
            Program.CreateHandler(path);
            Program.OpenHandler(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler(true, false); // idbox ids list --detail true
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            Program.SetAsPrimaryHandler(); // idbox ids setPrimary
        }

        [Fact]
        /// <summary>
        /// Early adopter accesses primary identity
        /// </summary>
        public void GetPrimaryIdentityTest()
        {
            string path = @"c:\idbox";
            Program.CreateHandler(path);
            Program.OpenHandler(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler(true, false); // idbox ids list --detail true
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
            string path = @"c:\idbox";
            Program.CreateHandler(path);
            Program.OpenHandler(path); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler(true, false); // idbox ids list --detail true
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            Program.SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara
        }
    }
}