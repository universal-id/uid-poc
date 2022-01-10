using UniversalIdentity.Cli.Models;
using System.CommandLine;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Cli.Offline;

namespace UniversalIdentity.Cli
{
    // Creates a new identity box
    // Opens an identity box in interactive mode
    // Lists identities
    // Accesses primary identity
    // Accesses identity information
    public class Program
    {
        static async Task Main(string[] args)
        {
            OfflineCliHandlers cliHandlers = new OfflineCliHandlers(Directory.GetCurrentDirectory());
            RootCommand idbox = cliHandlers.InitialIdbox();

            await idbox.InvokeAsync(args);
        }
    }
}
