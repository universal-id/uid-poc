using Newtonsoft.Json.Linq;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using UniversalIdentity.Library.Storage;

namespace MyNamespace
{
    // Creates a new identity box
    // Opens an identity box in interactive mode
    // Lists identities
    // Accesses primary identity
    // Accesses identity information
    class Program
    {
        static async Task Main(string[] args)
        {
            RootCommand? idbox = new("idbox");
            Command box = new("box");
            Command create = new("create");
            Command open = new("open");
            Command ids = new("ids");
            Command list = new("list");
            idbox.Add(box);
            box.Add(create);
            box.Add(open);
            idbox.Add(ids);
            ids.Add(list);

            Argument argument = new("fileName");
            create.AddArgument(argument);
            create.Handler = CommandHandler.Create<string>(CreateHandler);

            open.AddArgument(argument);
            open.Handler = CommandHandler.Create<string>(OpenHandler);

            Option<bool> summaryOption = new("--summary", () => true);
            Option<bool> detailOption = new("--detail", () => false);
            list.AddOption(summaryOption);
            list.AddOption(detailOption);
            list.Handler = CommandHandler.Create(ListHandler);


            await idbox.InvokeAsync(args);

            //CreateHandler(@"c:\idbox");
            //await OpenHandler(@"c:\idbox\0xaF3eB19fcA6E327A6972796Febb43c1D46eBDb6b");

            // Creates a new identity box
            void CreateHandler(string fileName)
            {

                if (!Directory.Exists(fileName)) Directory.CreateDirectory(fileName);
                IdBoxStorage idBoxStorage = new(fileName);


                idBoxStorage.InitializeStorage();

                IdentityStorage? seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
                IdentityStorage? savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);

                Console.WriteLine($"IdBox created from location {Path.Combine(Path.GetTempPath(), fileName)}");
            }

            async Task OpenHandler(string fileName)
            {
                IEnumerable<string>? segments = FileRepositoryHelper.GetSegments(fileName);
                IdBoxStorage idBoxStorage = new(fileName);
                string? fileContents = idBoxStorage.Repository.GetFileContents($"identities", $"f{segments.Last()}");
                IdentityStorage? updatedIdentityStorage = new IdentityStorage();
                JObject? updatedIdentityJson = JObject.Parse(fileContents);
                updatedIdentityStorage.FromJson(updatedIdentityJson);

                string text = idBoxStorage.ToJson().ToString();
                await File.WriteAllTextAsync(Path.Combine(Path.GetTempPath(), "open"), text);

                Console.WriteLine($"IdBox opened from location {Path.Combine(Path.GetTempPath(), fileName)}");
            }

            async Task ListHandler(bool detail, bool summary)
            {
                string text = await File.ReadAllTextAsync(Path.Combine(Path.GetTempPath(), "open"));

                if (detail)
                    Console.WriteLine("Detail!");
                else if (summary)
                    Console.WriteLine("Summary!");

                Console.WriteLine(text);
            }
        }
    }
}
