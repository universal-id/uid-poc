using UniversalIdentity.Cli.Extensions;
using UniversalIdentity.Cli.Models;
using System.CommandLine;
using System.CommandLine.Invocation;
using UniversalIdentity.Library.Storage;

namespace UniversalIdentity.Cli.Offline
{
    public class OfflineCliHandlers
    {
        public OfflineCliHandlers(string executionPath)
        {
            ExecutionPath = executionPath;
        }
        public string ExecutionPath { get; set; }
        public State? State { get; set; } // Encapsulates access to disk
                                          //public IdBoxService IdBoxService { get; set; } // Enables communication service hosting for interactive mode.

        public void InitialIdBox(RootCommand rootCommand)
        {
            Command boxCommand = new("box");
            Command createCommand = new("create");
            Command openCommand = new("open");
            Command idsCommand = new("ids");
            Command listCommand = new("list");
            Command setPrimaryCommand = new("setPrimary");
            Command getPrimaryCommand = new("getPrimary");
            Command idCommand = new("id");
            Command selectCommand = new("select");
            Command getCommand = new("get");
            Command infoCommand = new("info");
            Command setCommand = new("set");
            Command createSeedCommand = new("createSeed");
            Command beaconCommand = new("beacon");
            Command activateCommand = new("activate");

            rootCommand.Add(boxCommand);
            boxCommand.Add(createCommand);
            boxCommand.Add(openCommand);
            rootCommand.Add(idsCommand);
            rootCommand.Add(idCommand);
            idsCommand.Add(listCommand);
            idsCommand.Add(setPrimaryCommand);
            idsCommand.Add(getPrimaryCommand);
            idsCommand.Add(getCommand);
            idCommand.Add(selectCommand);
            idsCommand.Add(createSeedCommand);
            rootCommand.Add(idCommand);
            idCommand.Add(selectCommand);
            idCommand.Add(infoCommand);
            rootCommand.Add(infoCommand);
            infoCommand.Add(setCommand);
            rootCommand.Add(beaconCommand);
            infoCommand.Add(activateCommand);

            //var cliHandlers = new CliHandlers(Directory.GetCurrentDirectory());

            Argument pathArgument = new("path");
            createCommand.AddArgument(pathArgument);
            createCommand.Handler = CommandHandler.Create<string>(CreateHandler);

            Argument openArgument = new("interactive");
            openArgument.SetDefaultValue("non-interactive");
            openArgument.AddSuggestions("interactive", "non-interactive");
            openCommand.AddArgument(pathArgument);
            openCommand.AddArgument(openArgument);
            openCommand.Handler = CommandHandler.Create<string, string>(OpenHandlerAsync);

            Argument summaryOrDetailArgument = new("summaryordetail");
            summaryOrDetailArgument.SetDefaultValue("--summary");
            summaryOrDetailArgument.AddSuggestions("summary", "detail");
            listCommand.AddArgument(summaryOrDetailArgument);
            listCommand.Handler = CommandHandler.Create(ListHandler);

            createSeedCommand.Handler = CommandHandler.Create(CreateSeedIdentityHandler);

            getPrimaryCommand.Handler = CommandHandler.Create(GetPrimaryHandler);
            getPrimaryCommand.AddAlias("get-primary");

            setPrimaryCommand.Handler = CommandHandler.Create(SetAsPrimaryHandler);
            setPrimaryCommand.AddAlias("set-primary");


            Argument selectArgument = new("identifier");
            selectCommand.AddArgument(pathArgument);
            selectCommand.Handler = CommandHandler.Create<string>(SelectHandler);

            getCommand.AddArgument(selectArgument);
            getCommand.Handler = CommandHandler.Create(GetSelectedIdentityHandler);

            Option<string> keyOption = new("--key", () => "");
            Option<string> valueOption = new("--value", () => "");
            setCommand.AddArgument(summaryOrDetailArgument);
            setCommand.Handler = CommandHandler.Create(SetInfoHandler);
            activateCommand.Handler = CommandHandler.Create(ActivateBeaconHandler);
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

                    RootCommand rootCommand = new("uid");
                    InitialIdBox(rootCommand);

                    if (args == "exit")
                        break;

                    await rootCommand.InvokeAsync(args);
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
            State state = new State(Path.Combine(ExecutionPath, "State.json")).Load();
            state.StartCommunications();
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
            State state = new State(Path.Combine(ExecutionPath, "State.json")).Load();
            state.StartCommunications();
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