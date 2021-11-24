namespace UniversalIdentity.Library.Storage
{
    public class KeyStorage
    {
        public string Identifier { get; set; }
        public ValueLevel Level { get; set; }
        public long Created { get; set; }
        public string PublicKey { get; set; }
    }
}