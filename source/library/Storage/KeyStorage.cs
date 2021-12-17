namespace UniversalIdentity.Library.Storage
{
    public class KeyStorage
    {
        public KeyStorage(string identifier, string key)
        {
            this.Identifier = identifier;
            this.PublicKey = key;
        }

        public string Identifier { get; set; }
        public ValueLevel Level { get; set; }
        public long Created { get; set; }
        public string PublicKey { get; set; }
    }
}