using Microsoft.VisualBasic;
using OfflineClient.Extensions;
using OfflineClient.Models;
using System.CommandLine;
using System.CommandLine.Invocation;
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
            RootCommand idbox = InitialIdbox();

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
            openArgument.AddSuggestions("interactive", "non-interactive");
            open.AddArgument(argument);
            open.AddArgument(openArgument);
            open.Handler = CommandHandler.Create<string, string>(OpenHandlerAsync);

            Argument summaryordetailArgument = new("summaryordetail");
            summaryordetailArgument.SetDefaultValue("--summary");
            summaryordetailArgument.AddSuggestions("summary", "detail");
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
            Create(path);
        }

        public static void Create(string path)
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
            CreateSeedIdentity("State.json");
        }

        public static string CreateSeedIdentity(string stateFilePath)
        {
            string path = new State(stateFilePath).Load().Path;
            IdBoxStorage idBoxStorage = new(path);

            IdentityStorage seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            IdentityStorage savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);

            Console.WriteLine($"SeedIdentity with Identifier{seedIdentityStorage.Identifier} is created!");

            return savedSeedIdentityStorage.Identifier;
        }

        public static void SetAsPrimaryHandler()
        {
            SetAsPrimary("State.json");

        }

        public static void SetAsPrimary(string stateFilePath)
        {
            State state = new State(stateFilePath).Load();
            IdBoxStorage idBoxStorage = new(state.Path);

            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == state.SelectedIdentity);

            if (identity is not null)
            {
                idBoxStorage.PrimaryIdentity = identity.Identifier;
                idBoxStorage.Save();

                Console.WriteLine($"SeedIdentity with Identifier{identity.Identifier} is set as a Primarydentity!");
            }
        }

        // Opens an identity box
        public static async Task OpenHandlerAsync(string path, string interactive = "non-interactive")
        {
            await OpenAsync( path, "State.json", interactive, InitialIdbox());
        }
        public async static Task OpenAsync( string path, string stateFilePath, string interactive, RootCommand rootCommand)
        {
            IdBoxStorage idBoxStorage = new(path);
            Console.WriteLine($"IdBox opened from location {path}");

            idBoxStorage.DisplayIdenetities();

            State state = new(path: path, selectedIdentity: "", stateFilePath: stateFilePath);
            state.Save();

            if (interactive == "--interactive")
            {
                while (true)
                {
                    string? args = "";
                    while (string.IsNullOrWhiteSpace(args))
                    {
                        Console.Write(">>");
                        args = Console.ReadLine();
                    }

                    RootCommand idbox = rootCommand;

                    if (args == "exit")
                        break;

                    await idbox.InvokeAsync(args);
                }

            }
        }

        // Lists identities
        public static void ListHandler(string summaryordetail)
        {
            List(summaryordetail, "State.json");
        }

        public static void List(string summaryordetail, string stateFilePath)
        {
            string path = new State(stateFilePath).Load().Path;
            IdBoxStorage idBoxStorage = new(path);

            idBoxStorage.DisplayIdenetities(summaryordetail);
        }

        // Accesses primary identity
        public static void GetPrimaryHandler()
        {
            GetPrimary("State.json");
        }

        public static void GetPrimary(string stateFilePath)
        {
            string path = new State(stateFilePath).Load().Path;
            IdBoxStorage idBoxStorage = new(path);
            idBoxStorage.Get();

            Console.WriteLine($"Primary Identity: {idBoxStorage.PrimaryIdentity}");
        }

        public static void SelectHandler(string identifier)
        {
            Select(identifier, "State.json");
        }

        public static void Select(string identifier,string stateFilePath)
        {
            State state = new State(stateFilePath).Load();
            IdBoxStorage idBoxStorage = new(state.Path);

            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == identifier);

            if (identity == null)
            {
                Console.WriteLine("Not found!");
            }
            else
            {

                state.SelectedIdentity = identifier;
                state.Save();

                Console.WriteLine($"Identity selected with identifier: {identity.Identifier}");
            }
        }

        public static void GetSelectedIdentityHandler(string summaryordetail)
        {
            GetSelectedIdentity(summaryordetail, "State.json");
        }

        public static void GetSelectedIdentity(string summaryordetail,string stateFilePath)
        {
            State state = new State(stateFilePath).Load();
            IdBoxStorage idBoxStorage = new(state.Path);
            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == state.SelectedIdentity);

            if (identity == null)
            {
                Console.WriteLine("Not found!");
            }
            else
            {
                if (summaryordetail == "--summary")
                {
                    Console.WriteLine($"Identifier: {identity.Identifier}");
                }
                else
                {
                    Console.WriteLine($"Identifier: {identity.Identifier}");
                    Console.WriteLine($"Level: {identity.Level}");

                    Console.WriteLine("Keys:");
                    foreach (KeyStorage key in identity.Keys)
                    {
                        Console.WriteLine($" Identifier: {key.Identifier}");
                        Console.WriteLine($" Level: {key.Level}");
                        Console.WriteLine($" Created: {key.Created}");
                        Console.WriteLine($" PublicKey: {key.PublicKey}");
                    }
                }
            }
        }

        //idbox id info set --key k1--value v1
        //Updated identity information 'k1' to 'v1'
        public static void SetInfoHandler(string key, string value)
        {
            SetInfo(key, value, "State.json");
        }

        public static void SetInfo(string key, string value,string stateFilePath)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine("Enter a key and value option (for example: idbox id info set --key k1--value v1)!");
            }

            State state = new State(stateFilePath).Load();

            IdBoxStorage idBoxStorage = new(state.Path);

            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == state.SelectedIdentity);

            if (identity == null)
            {
                Console.WriteLine("Not found!");
            }
            else
            {
                Info? existedInfo = identity.Info.FirstOrDefault(x => x.Key == key);
                if (existedInfo == null)
                {
                    List<Info> info = identity.Info.ToList();
                    info.Add(new Info { Key = key, Value = value });
                    identity.Info = info.ToArray();
                }
                else
                {
                    existedInfo.Value = value;
                }

                IdentityStorage savedSeedIdentityStorage = idBoxStorage.SaveIdentity(identity);

                Console.WriteLine($"Updated identity information '{key}' to '{value}'");
            }
        }
    }
}
