using OfflineClient.Extensions;
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
    class Program
    {
        static async Task Main(string[] args)
        {
            RootCommand idbox = new("idbox");
            Command box = new("box");
            Command create = new("create");
            Command open = new("open");
            Command ids = new("ids");
            Command list = new("list");
            Command getPrimary = new("getPrimary");
            Command id = new("id");
            Command select = new("select");
            Command get = new("get");
            Command info = new("info");
            Command set = new("set");

            idbox.Add(box);
            box.Add(create);
            box.Add(open);
            idbox.Add(ids);
            ids.Add(list);
            ids.Add(getPrimary);
            id.Add(select);
            id.Add(get);
            id.Add(info);
            info.Add(set);

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

            getPrimary.Handler = CommandHandler.Create(GetPrimary);

            Argument selectArgument = new("identifier");
            select.AddArgument(argument);
            select.Handler = CommandHandler.Create<string>(SelectHandler);

            get.AddOption(summaryOption);
            get.AddOption(detailOption);
            get.Handler = CommandHandler.Create(GetHandler);

            Option<string> keyOption = new("--key", () => "");
            Option<string> valueOption = new("--value", () => "");
            set.AddOption(summaryOption);
            set.AddOption(detailOption);
            set.Handler = CommandHandler.Create(SetInfoHandler);


            //await idbox.InvokeAsync(args);

            //CreateHandler(@"c:\idbox");
            //OpenHandler(@"c:\idbox");
            //ListHandler(true, false);
            //GetPrimary();
            //SelectHandler("did:eth:0x154886Be866F59C4D9065569877c3a04B2940FC5");
            //GetHandler(true,false);
            //SetInfoHandler("Name", "Yara");

            // Creates a new identity box
            void CreateHandler(string fileName)
            {

                if (!Directory.Exists(fileName)) Directory.CreateDirectory(fileName);
                IdBoxStorage idBoxStorage = new(fileName);


                idBoxStorage.InitializeStorage();

                CreateSeedIdentityHandler();

                Console.WriteLine($"IdBox created from location {Path.Combine(Path.GetTempPath(), fileName)}");
            }

            void CreateSeedIdentityHandler()
            {
                string path = @"c:\idbox"; // TODO: Get from State json
                IdBoxStorage idBoxStorage = new(path);

                IdentityStorage seedIdentityStorage = idBoxStorage.CreateSeedIdentity();
                IdentityStorage savedSeedIdentityStorage = idBoxStorage.SaveIdentity(seedIdentityStorage);
            }

            void SetAsPrimarydentityHandler()
            {
                string path = @"c:\idbox"; // TODO: Get from State json
                IdBoxStorage idBoxStorage = new(path);

                IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == "did:eth:0x154886Be866F59C4D9065569877c3a04B2940FC5");

                if (identity != null)
                    identity.IsPrimary = true;

                IdentityStorage savedSeedIdentityStorage = idBoxStorage.SaveIdentity(identity);
            }

            // Opens an identity box
            void OpenHandler(string path)
            {
                IdBoxStorage idBoxStorage = new(path);
                List<IdentityStorage>? identities = idBoxStorage.Identities.ToList();
                Console.WriteLine($"IdBox opened from location {Path.Combine(Path.GetTempPath(), path)}");

                idBoxStorage.DisplayIdenetities();

                // TODO: Save it to State json
            }

            // Lists identities
            void ListHandler(bool detail, bool summary)
            {
                string path = @"c:\idbox"; // TODO: Gets from State json
                IdBoxStorage idBoxStorage = new(path);

                idBoxStorage.DisplayIdenetities(detail, summary);
            }

            // Accesses primary identity
            void GetPrimary()
            {
                string path = @"c:\idbox"; // TODO: Gets from State json
                IdBoxStorage idBoxStorage = new(path);

                Console.WriteLine($"Primary Identity: {idBoxStorage.PrimaryIdentity}");
            }

            void SelectHandler(string identifier)
            {
                string path = @"c:\idbox"; // TODO: Gets from State json
                IdBoxStorage idBoxStorage = new(path);

                IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == identifier);

                if (identity == null)
                {
                    Console.WriteLine("Not found!");
                }
                else
                    Console.WriteLine($"Identity selected with identifier: {identity.Identifier}");

                // TODO: Save it to State json
            }

            void GetHandler(bool detail, bool summary)
            {
                // TODO: Gets from State json
                string path = @"c:\idbox";
                IdBoxStorage idBoxStorage = new(path);
                IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == "did:eth:0x154886Be866F59C4D9065569877c3a04B2940FC5");

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
            void SetInfoHandler(string key, string value)
            {
                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
                    Console.WriteLine("Enter a key and value option (for example: idbox id info set --key k1--value v1)!");

                string path = @"c:\idbox"; // TODO: Gets from State json
                IdBoxStorage idBoxStorage = new(path);

                IdentityStorage? identity = idBoxStorage.Identities.FirstOrDefault(x => x.Identifier == "did:eth:0x154886Be866F59C4D9065569877c3a04B2940FC5");

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
                        existedInfo.Value = value;

                    IdentityStorage savedSeedIdentityStorage = idBoxStorage.SaveIdentity(identity);

                    Console.WriteLine($"Updated identity information '{key}' to '{value}'");
                }
            }
        }
    }
}
