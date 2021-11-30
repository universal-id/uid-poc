using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UniversalIdentity.Library.Cryptography;

namespace UniversalIdentity.Library.Storage
{
    public class IdBoxStorage : IJsonSerializable<IdBoxStorage>
    {
        public string Path;

        public IdBoxStorage(string path)
        {
            this.Path = path;
            this.Repository = new FileRepository(path);
        }

        public FileRepository Repository { get; set; }

        public void InitializeStorage()
        {
            if (this.Repository == null) throw new Exception("Expected repository to be valid before initializing storage.");
            this.Repository.Init();
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
                Keys = new[] { new KeyStorage() {
                     Identifier = identifier,
                     PublicKey = publicKey,
                     Level = ValueLevel.MediumLow,
                     Created = Helper.ConvertToUnixTime(DateTime.UtcNow)
                }}
            };

            return identityStorage;
        }

        public Dictionary<string, IdentityStorage> Identities = new Dictionary<string, IdentityStorage>(StringComparer.OrdinalIgnoreCase);

        public string? PrimaryIdentity { get; set; }

        public IdentityStorage SaveIdentity(IdentityStorage identityStorage)
        {
            Identities[identityStorage.Identifier] = identityStorage;
            this.Repository.UpdateOneFile($"identities", $"f{identityStorage.Identifier}", identityStorage.ToJson().ToString());
            var fileContents = this.Repository.GetFileContents($"identities", $"f{identityStorage.Identifier}");
            var updatedIdentityStorage = new IdentityStorage();
            var updatedIdentityJson = JObject.Parse(fileContents);
            updatedIdentityStorage.FromJson(updatedIdentityJson);
            return updatedIdentityStorage;
        }

        public void FromJson(JObject documentJson)
        {
            PrimaryIdentity = (string)documentJson["primaryIdentity"];
        }

        public JObject ToJson() => new() { ["primaryIdentity"] = PrimaryIdentity };
    }
}