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

            open.AddArgument(argument);
            open.Handler = CommandHandler.Create<string>(OpenHandler);

            Option<bool> summaryOption = new("--summary", () => false);
            Option<bool> detailOption = new("--detail", () => true);
            list.AddOption(summaryOption);
            list.AddOption(detailOption);
            list.Handler = CommandHandler.Create(ListHandler);

            createSeed.Handler = CommandHandler.Create(CreateSeedIdentityHandler);

            getPrimary.Handler = CommandHandler.Create(GetPrimaryHandler);

            setPrimary.Handler = CommandHandler.Create(SetAsPrimaryHandler);

            Argument selectArgument = new("identifier");
            select.AddArgument(argument);
            select.Handler = CommandHandler.Create<string>(SelectHandler);

            get.AddOption(summaryOption);
            get.AddOption(detailOption);
            get.Handler = CommandHandler.Create(GetSelectedIdentityHandler);

            Option<string> keyOption = new("--key", () => "");
            Option<string> valueOption = new("--value", () => "");
            set.AddOption(summaryOption);
            set.AddOption(detailOption);
            set.Handler = CommandHandler.Create(SetInfoHandler);


            //await idbox.InvokeAsync(args);

            CreateHandler(@"c:\idbox"); // idbox box create c:\idbox
            Console.WriteLine("**********************0");
            OpenHandler(@"c:\idbox"); // idbox box open c:\idbox 
            Console.WriteLine("**********************1");
            ListHandler(true, false); // idbox ids list [--summary true] or idbox ids list [--summary false]
            Console.WriteLine("**********************2");
            string identifier = CreateSeedIdentity(); // idbox ids createSeed
            Console.WriteLine("**********************3");
            ListHandler(true, false); // idbox ids list [--summary true] or idbox ids list [--summary false]
            Console.WriteLine("**********************4");
            SelectHandler(identifier); // idbox id select 0xa1b2c3…d4e5f6
            Console.WriteLine("**********************5");
            SetAsPrimaryHandler(); // idbox ids setPrimary
            Console.WriteLine("**********************6");
            GetPrimaryHandler(); // idbox ids getprimary
            Console.WriteLine("**********************7");
            GetSelectedIdentityHandler(true, false); // idbox id get --summary
            Console.WriteLine("**********************8");
            SetInfoHandler("Name", "Yara"); // idbox id info set --key Name --value Yara

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

        public static void SetAsPrimaryHandler()
        {
            State state = new State().Load();
            IdBoxStorage idBoxStorage = new(state.Path);

            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == state.SelectedIdentity);

            if (identity is not null)
            {
                idBoxStorage.PrimaryIdentity=identity.Identifier;
               idBoxStorage.Save();

                Console.WriteLine($"SeedIdentity with Identifier{identity.Identifier} is set as a Primarydentity!");
            }

        }

        // Opens an identity box
        public static void OpenHandler(string path)
        {
            Console.WriteLine(path);
            IdBoxStorage idBoxStorage = new(path);
            Console.WriteLine($"IdBox opened from location {path}");

            idBoxStorage.DisplayIdenetities();

            State state = new() { SelectedIdentity = "", Path = path };
            state.Save();
        }

        // Lists identities
        public static void ListHandler(bool detail, bool summary)
        {
            Console.WriteLine("List!");
            string path = new State().Load().Path;
            IdBoxStorage idBoxStorage = new(path);

            idBoxStorage.DisplayIdenetities(detail, summary);
        }

        // Accesses primary identity
        public static void GetPrimaryHandler()
        {
            string path = new State().Load().Path;
            IdBoxStorage idBoxStorage = new(path);
            idBoxStorage.Get();

            Console.WriteLine($"Primary Identity: {idBoxStorage.PrimaryIdentity}");
        }

        public static void SelectHandler(string identifier)
        {
            State state = new State().Load();
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

        public static void GetSelectedIdentityHandler(bool detail, bool summary)
        {
            State state = new State().Load();
            IdBoxStorage idBoxStorage = new(state.Path);
            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == state.SelectedIdentity);

            if (identity == null)
            {
                Console.WriteLine("Not found!");
            }
            else
            {
                if (summary)
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
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine("Enter a key and value option (for example: idbox id info set --key k1--value v1)!");
            }

            State state = new State().Load();

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
