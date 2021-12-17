using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UniversalIdentity.Library.Cryptography;

namespace UniversalIdentity.Library.Storage
{
    public class IdBoxStorage : IJsonSerializable<IdBoxStorage>
    {
        public string Path;
        private string? primaryIdentity;

        public IdBoxStorage(string path)
        {
            this.Path = path;
            this.Repository = new FileRepository(path);

            if (this.Repository.IsInitialized())
            {
                LoadIdBox(this);
                Identities = LoadIdentities();
            }
        }

        public FileRepository Repository { get; set; }

        public void InitializeStorage()
        {
            if (this.Repository == null) throw new Exception("Expected repository to be valid before initializing storage.");
            this.Repository.Init();
            Save();
        }

        public IdentityStorage CreateSeedIdentity()
        {
            var key = new EthKey();
            string publicKey = key.GetPublicKey();
            string identifier = key.GetIdentifier();

            var identityStorage = new IdentityStorage()
            {
                Identifier = identifier,
                Level = ValueLevel.MediumLow,
                Keys = new[] { new KeyStorage(identifier, publicKey) {
                     Level = ValueLevel.MediumLow,
                     Created = Helper.ConvertToUnixTime(DateTime.UtcNow)
                }},
                Info = new[]
                {
                    new Info{
                        Key ="Key1",Value="Value1"
                    }
                }
            };

            return identityStorage;
        }

        //public Dictionary<string, IdentityStorage> Identities = new Dictionary<string, IdentityStorage>(StringComparer.OrdinalIgnoreCase);
        public IEnumerable<IdentityStorage> Identities { get; set; }
        public string? PrimaryIdentity { get; set ; }

        public IdentityStorage SaveIdentity(IdentityStorage identityStorage)
        {
            //Identities[identityStorage.Identifier] = identityStorage;
            this.Repository.UpdateOneFile($"identities", $"f{identityStorage.Identifier}", identityStorage.ToJson().ToString());
            var fileContents = this.Repository.GetFileContents($"identities", $"f{identityStorage.Identifier}");
            var updatedIdentityStorage = new IdentityStorage();
            var updatedIdentityJson = JObject.Parse(fileContents);
            updatedIdentityStorage.FromJson(updatedIdentityJson);
            return updatedIdentityStorage;
        }

        public void LoadIdBox(IdBoxStorage idBoxStorage)
        {
            //string fileName = $"f{Path.Replace(System.IO.Path.DirectorySeparatorChar.ToString(), string.Empty).Replace(":", "")}";
            //var idBoxJsonFileContents = Repository.GetFileContents("idbox", $"f{fileName}");
            var idBoxJsonFileContents = Repository.GetFileContents("idbox", $"idbox.json");
            if(idBoxJsonFileContents != null)
            {
                var idBoxJson = JObject.Parse(idBoxJsonFileContents);
                idBoxStorage.FromJson(idBoxJson);
            }
        }

        public IEnumerable<IdentityStorage> LoadIdentities()
        {
            IEnumerable<string> files = Repository.GetFiles("identities");
            if (files is not null)
            {
                foreach (var file in files)
                {
                    var fileContents = this.Repository.GetFileContents("identities", $"{file}");
                    var result = new IdentityStorage();
                    var updatedIdentityJson = JObject.Parse(fileContents);
                    result.FromJson(updatedIdentityJson);

                    yield return result;
                }
            }
        }
        public IdentityStorage GetIdentity(string identifier)
        {
            var fileContents = this.Repository.GetFileContents($"identities", $"f{identifier}");
            var result = new IdentityStorage();
            var updatedIdentityJson = JObject.Parse(fileContents);
            result.FromJson(updatedIdentityJson);

            return result;
        }

        public void FromJson(JObject documentJson)
        {
            PrimaryIdentity = (string)documentJson["primaryIdentity"];
            Identities = JsonConvert.DeserializeObject<IEnumerable<IdentityStorage>>((string)documentJson["identities"]);
        }

        public JObject ToJson()
        {
            return new()
            {
                ["primaryIdentity"] = PrimaryIdentity,
                ["identities"] = JsonConvert.SerializeObject(Identities)
            };
        }
        public void Save()
        {
            string fileName = $"f{Path.Replace(System.IO.Path.DirectorySeparatorChar.ToString(), string.Empty).Replace(":", "")}";
            Repository.UpdateOneFile("idbox", $"idbox.json", ToJson().ToString());
        }

        public IdBoxStorage Get()
        {
            // string fileName = $"f{Path.Replace(System.IO.Path.DirectorySeparatorChar.ToString(), string.Empty).Replace(":", "")}";
            // var fileContents = Repository.GetFileContents($"idbox", $"f{fileName}");
            var fileContents = Repository.GetFileContents($"idbox", $"idbox.json");
            var updatedIdentityJson = JObject.Parse(fileContents);
            FromJson(updatedIdentityJson);

            return this;
        }
    }
}