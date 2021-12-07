using OfflineClient;
using Xunit;

namespace OflineClient.Test
{
    public class ProgramTest
    {
        [Fact]
        public void CLIclientScenariosTest()
        {
            Program.CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
            Program.OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
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
            Program.CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
        }

        [Fact]
        /// <summary>
        /// Early adopter opens an identity box
        /// </summary>
        public void OpenIdboxTest()
        {
            Program.OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
        }

        [Fact]
        /// <summary>
        /// Early adopter lists identities
        /// </summary>
        public void ListsIdentitiesTest()
        {
            Program.CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
            Program.OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
            Program.ListHandler(detail: true, summary: false); // idbox ids list --detail true
            Program.ListHandler(detail: false, summary: true); //  idbox ids list --summary true
        }

        [Fact]
        /// <summary>
        /// Early adopter create and select Identity
        /// </summary>
        public void CreateAndSelectIdentityTest()
        {
            Program.CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
            Program.OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler(true, false); // idbox ids list --detail true
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
        }

        [Fact]
        /// <summary>
        /// Early adopter select and SetAsPrimary Identity
        /// </summary>
        public void SetAsPrimaryIdentityTest()
        {
            Program.CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
            Program.OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
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
            Program.CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
            Program.OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
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
            Program.CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
            Program.OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
            string identifier = Program.CreateSeedIdentity(); // idbox ids createSeed
            Program.ListHandler(true, false); // idbox ids list --detail true
            Program.SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            Program.SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara
        }
    }
}