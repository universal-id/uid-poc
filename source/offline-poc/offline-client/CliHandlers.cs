using OfflineClient.Extensions;
using OfflineClient.Models;
using System.CommandLine;
using System.CommandLine.Invocation;
using UniversalIdentity.Library.Storage;

namespace OfflineClient
{
    public class CliHandlers
    {
        public CliHandlers(string executionPath)
        {
            ExecutionPath = executionPath;
        }
        public string ExecutionPath { get; set; }
        public State? State { get; set; } // Encapsulates access to disk
                                          //public IdBoxService IdBoxService { get; set; } // Enables communication service hosting for interactive mode.

        public RootCommand InitialIdbox()
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
            Command beacon = new("beacon");
            Command activate = new("activate");

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
            idbox.Add(id);
            id.Add(select);
            id.Add(info);
            idbox.Add(info);
            info.Add(set);
            idbox.Add(beacon);
            info.Add(activate);

            //var cliHandlers = new CliHandlers(Directory.GetCurrentDirectory());

            Argument pathArgument = new("path");
            create.AddArgument(pathArgument);
            create.Handler = CommandHandler.Create<string>(CreateHandler);

            Argument openArgument = new("interactive");
            openArgument.SetDefaultValue("non-interactive");
            openArgument.AddSuggestions("interactive", "non-interactive");
            open.AddArgument(pathArgument);
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
            select.AddArgument(pathArgument);
            select.Handler = CommandHandler.Create<string>(SelectHandler);

            get.AddArgument(selectArgument);
            get.Handler = CommandHandler.Create(GetSelectedIdentityHandler);

            Option<string> keyOption = new("--key", () => "");
            Option<string> valueOption = new("--value", () => "");
            set.AddArgument(summaryordetailArgument);
            set.Handler = CommandHandler.Create(SetInfoHandler);
            activate.Handler = CommandHandler.Create(ActivateBeaconHandler);

            return idbox;
        }


        public void CreateHandler(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            IdBoxStorage idBoxStorage = new(path);

            idBoxStorage.InitializeStorage();

            Console.WriteLine($"IdBox created from location {Path.Combine(Path.GetTempPath(), path)}");
        }

        public void CreateSeedIdentityHandler()
        {
             CreateSeedIdentity();
        }

        public string CreateSeedIdentity()
        {
            var state = new State(Path.Combine(ExecutionPath, "State.json")).Load();
            string path = state.Path;
            IdBoxStorage idBoxStorage = new(path);

            IdentityStorage seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
            IdentityStorage savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);

            Console.WriteLine($"SeedIdentity with Identifier{seedIdentityStorage.Identifier} is created!");

            state.SelectedIdentity = seedIdentityStorage.Identifier;
            state.Save();
            return savedSeedIdentityStorage.Identifier;
        }

        public async Task OpenHandlerAsync(string path, string interactive = "non-interactive")
        {
            IdBoxStorage idBoxStorage = new(path);
            Console.WriteLine($"IdBox opened from location {path}");

            idBoxStorage.DisplayIdentities();

            State state = new(path: path, selectedIdentity: "", stateFilePath:Path.Combine( ExecutionPath, "State.json"));
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

                    RootCommand idbox = InitialIdbox();

                    if (args == "exit")
                        break;

                    await idbox.InvokeAsync(args);
                }
            }
        }

        public  void SetInfoHandler(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine("Enter a key and value option (for example: idbox id info set --key k1--value v1)!");
            }

            State state = new State(Path.Combine(ExecutionPath, "State.json"));

            IdBoxStorage idBoxStorage = new(state.Path);

            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == state.SelectedIdentity);

            if (identity == null)
            {
                Console.WriteLine("Identifier not found!");
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

        public void GetSelectedIdentityHandler(string summaryordetail)
        {
            State state = new State(Path.Combine(ExecutionPath, "State.json"));
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

        public  void SelectHandler(string identifier)
        {
            State state = new State(Path.Combine(ExecutionPath, "State.json"));
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

        public  void SetAsPrimaryHandler()
        {
            State state = new State(Path.Combine(ExecutionPath, "State.json"));
            IdBoxStorage idBoxStorage = new(state.Path);

            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == state.SelectedIdentity);

            if (identity is not null)
            {
                idBoxStorage.PrimaryIdentity = identity.Identifier;
                idBoxStorage.Save();

                Console.WriteLine($"SeedIdentity with Identifier{identity.Identifier} is set as a PrimaryIdentity!");
            }
        }

        public  void GetPrimaryHandler()
        {
            string path = new State(Path.Combine(ExecutionPath, "State.json")).Path;
            IdBoxStorage idBoxStorage = new(path);
            idBoxStorage.Get();

            Console.WriteLine($"Primary Identity: {idBoxStorage.PrimaryIdentity}");
        }

        public  void ListHandler(string summaryordetail)
        {
            string path = new State(Path.Combine(ExecutionPath, "State.json")).Path;
            IdBoxStorage idBoxStorage = new(path);

            idBoxStorage.DisplayIdentities(summaryordetail);
        }

        //idbox beacon activate
        public void ActivateBeaconHandler(string path, string identifier)
        {
            State state = new State().Load();
            State.StartCommunications();
            var idBoxService = State.IdBoxService;
            var idBoxStorage = idBoxService.Storage;
            var beaconProtocol = idBoxService.Communication.BeaconProtocol;

            identifier = identifier ?? state.SelectedIdentity;

            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == identifier);

            if (identity == null)
            {
                Console.WriteLine("Identifier not found!");
            }
            else
            {
                Console.WriteLine($"Starting connect beacon ...");

                var beaconEndpoint = beaconProtocol.ActivateBeacon(identifier);

                Console.WriteLine($"Beacon started. Beacon URI: '{beaconEndpoint?.Address?.ToString()}'.");
                Console.Write($"Waiting for responses. ");
            }
        }

        //idbox beacon respond 'https://127.0.0.1/abcd'
        public void BeaconRespondHandler(string address, string identifier)
        {
            State state = new State().Load();
            State.StartCommunications();
            var idBoxService = State.IdBoxService;
            var idBoxStorage = idBoxService.Storage;
            var beaconProtocol = idBoxService.Communication.BeaconProtocol;

            identifier = identifier ?? state.SelectedIdentity;

            IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == identifier);

            if (identity == null)
            {
                Console.WriteLine("Identifier not found!");
            }
            else
            {
                Console.WriteLine($"Starting connect beacon ...");

                var beaconEndpoint = beaconProtocol.RespondToBeacon(address, identifier);

                Console.WriteLine($"Beacon started. Beacon URI: '{beaconEndpoint?.Address?.ToString()}'.");
                Console.Write($"Waiting for responses. ");
            }
        }

    }
}