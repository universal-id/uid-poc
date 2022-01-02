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

            //CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
            //Console.WriteLine("**********************0");
            //OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
            //Console.WriteLine("**********************1");
            //ListHandler("--detail"); // idbox ids list --detail
            //Console.WriteLine("**********************2");
            //string identifier = CreateSeedIdentity(); // idbox ids createSeed
            //Console.WriteLine("**********************3");
            //ListHandler("--summary"); // idbox ids list --detail
            //Console.WriteLine("**********************4");
            //SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            //Console.WriteLine("**********************5");
            //SetAsPrimaryHandler(); // idbox ids setPrimary
            //Console.WriteLine("**********************6");
            //GetPrimaryHandler(); // idbox ids getprimary
            //Console.WriteLine("**********************7");
            //GetSelectedIdentityHandler("--summary"); // idbox id get --summary
            //Console.WriteLine("**********************8");
            //SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara

        }

        private static RootCommand InitialIdbox()
        {
            RootCommand idbox = new("idbox");
            Command box = new("box");
            Command create = new("create");
            Command open = new("open");
            Command ids = new("ids");
            Command list = new("list");
            Command setPrimary = new("setPrimary");
            Command getPrimary = new("getPrimary");
            Command id = new("id");
            Command select = new("select");
            Command get = new("get");
            Command info = new("info");
            Command set = new("set");
            Command createSeed = new("createSeed");

            idbox.Add(box);
            box.Add(create);
            box.Add(open);
            idbox.Add(ids);
            idbox.Add(id);
            ids.Add(list);
            ids.Add(setPrimary);
            ids.Add(getPrimary);
            ids.Add(get);
            id.Add(select);
            ids.Add(createSeed);
            id.Add(info);
            info.Add(set);

            Argument argument = new("path");
            create.AddArgument(argument);
            create.Handler = CommandHandler.Create<string>(CreateHandler);

            Argument openArgument = new("interactive");
            openArgument.SetDefaultValue("non-interactive");
            open.AddArgument(argument);
            open.AddArgument(openArgument);
            open.Handler = CommandHandler.Create<string, string>(OpenHandlerAsync);

            Argument summaryordetailArgument = new("summaryordetail");
            summaryordetailArgument.SetDefaultValue("--summary");
            list.AddArgument(summaryordetailArgument);
            list.Handler = CommandHandler.Create(ListHandler);

            createSeed.Handler = CommandHandler.Create(CreateSeedIdentityHandler);

            getPrimary.Handler = CommandHandler.Create(GetPrimaryHandler);
            getPrimary.AddAlias("get-primary");

            setPrimary.Handler = CommandHandler.Create(SetAsPrimaryHandler);
            setPrimary.AddAlias("set-primary");


            Argument selectArgument = new("identifier");
            select.AddArgument(argument);
            select.Handler = CommandHandler.Create<string>(SelectHandler);

            get.AddArgument(selectArgument);
            get.Handler = CommandHandler.Create(GetSelectedIdentityHandler);

            Option<string> keyOption = new("--key", () => "");
            Option<string> valueOption = new("--value", () => "");
            set.AddArgument(summaryordetailArgument);
            set.Handler = CommandHandler.Create(SetInfoHandler);

            return idbox;
        }

        // Creates a new identity box
        public static void CreateHandler(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            IdBoxStorage idBoxStorage = new(path);

            idBoxStorage.InitializeStorage();

            Console.WriteLine($"IdBox created from location {Path.Combine(Path.GetTempPath(), path)}");
        }

        public static void CreateSeedIdentityHandler()
        {
            CreateSeedIdentity();
        }

        public static string CreateSeedIdentity()
        {
            string path = new State().Load().Path;
            IdBoxStorage idBoxStorage = new(path);

            IdentityStorage seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            IdentityStorage savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);

            Console.WriteLine($"SeedIdentity with Identifier{seedIdentityStorage.Identifier} is created!");

            return savedSeedIdentityStorage.Identifier;
        }
    }
}
