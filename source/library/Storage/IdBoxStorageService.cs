using System;

namespace UniversalIdentity.Library.Storage
{
    public class IdBoxStorageService
    {
        private string Path;

        public IdBoxStorageService(string path)
        {
            this.Path = path;
            IdBoxStorage = new IdBoxStorage(path);
        }

        public IdBoxStorage IdBoxStorage { get; set; }

        public void InitializeStorage()
        {
            
        }

        public IdentityStorage CreateSeedIdentity()
        {
            throw new NotImplementedException();
        }
    }
}