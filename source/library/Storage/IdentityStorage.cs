using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace UniversalIdentity.Library.Storage
{
    public class IdentityStorage : IJsonSerializable<IdentityStorage>
    {
        public string Identifier { get; set; }
        public ValueLevel Level { get;  set; }
        public KeyStorage[] Keys { get;  set; }
        public Info[] Info { get;  set; }

        public void FromJson(JObject documentJson)
        {
            this.Identifier = (string)documentJson["identifier"];
            var keys = (JArray)documentJson["keys"];
            var infos = (JArray)documentJson["info"];
            var identityKeysList = new List<KeyStorage>();
            Level = (ValueLevel)(int)documentJson["level"];
            foreach (var key in keys)
            {
                var identityKey = new KeyStorage((string)key["identifier"], (string)key["publicKey"]);
                identityKey.Level = (ValueLevel)(int)key["level"];
                identityKeysList.Add(identityKey);
            }
            this.Keys = identityKeysList.ToArray();
            var infoList = new List<Info>();

            foreach (var info in infos)
            {
                var temp = new Info();
                temp.Key = (string)info["key"];
                temp.Value = (string)info["value"];
                infoList.Add(temp);
            }
            this.Info = infoList.ToArray();
        }

        public JObject ToJson()
        {
            var identityJson = new JObject();
            identityJson["identifier"] = this.Identifier;
            var keys = new JArray();
            identityJson["keys"] = keys;
            identityJson["level"] = (int)Level;
            foreach (var identityKey in this.Keys)
            {
                var key = new JObject();
                key["identifier"] = identityKey.Identifier;
                key["publicKey"] = identityKey.PublicKey;
                key["level"] = (int)identityKey.Level;
                keys.Add(key);
            }

            var infos = new JArray();
            identityJson["info"] = infos;
            foreach (var info in this.Info)
            {
                var key = new JObject();
                key["key"] = info.Key;
                key["value"] = info.Value;
                infos.Add(key);
            }

            return identityJson;
        }
    }
}