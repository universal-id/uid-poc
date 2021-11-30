using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace UniversalIdentity.Library.Storage
{
    public class IdentityStorage : IJsonSerializable<IdentityStorage>
    {
        public string Identifier { get; set; }
        public ValueLevel Level { get;  set; }
        public KeyStorage[] Keys { get;  set; }

        public void FromJson(JObject documentJson)
        {
            this.Identifier = (string)documentJson["identifier"];
            var keys = (JArray)documentJson["keys"];
            var identityKeysList = new List<KeyStorage>();
            Level = (ValueLevel)(int)documentJson["level"];
            foreach (var key in keys)
            {
                var identityKey = new KeyStorage();
                identityKey.Identifier = (string)key["identifier"];
                identityKey.PublicKey = (string)key["publicKey"];
                identityKey.Level = (ValueLevel)(int)key["level"];
                identityKeysList.Add(identityKey);
            }
            this.Keys = identityKeysList.ToArray();
        }

        public JObject ToJson()
        {
            var identityJson = new JObject();
            identityJson["identifier"] = this.Identifier;
            var keys = new JArray();
            identityJson["keys"] = keys;
            identityJson["level"] = (int)Level;
            foreach(var identityKey in this.Keys)
            {
                var key = new JObject();
                key["identifier"] = identityKey.Identifier;
                key["publicKey"] = identityKey.PublicKey;
                key["level"] = (int)identityKey.Level;
                keys.Add(key);
            }

            return identityJson;
        }
    }
}