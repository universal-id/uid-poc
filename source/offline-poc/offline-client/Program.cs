using OfflineClient.Models;
using System.CommandLine;
using UniversalIdentity.Library.Storage;

namespace OfflineClient
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
            CliHandlers cliHandlers = new CliHandlers(Directory.GetCurrentDirectory());
            RootCommand idbox = cliHandlers.InitialIdbox();

            await idbox.InvokeAsync(args);
        }
    }
}
