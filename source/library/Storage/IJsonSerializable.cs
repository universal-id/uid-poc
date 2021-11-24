using Newtonsoft.Json.Linq;

namespace UniversalIdentity.Library.Storage
{
    public interface IJsonSerializable<TDocument>
    {
        void FromJson(JObject documentJson);
        JObject ToJson();
    }
}