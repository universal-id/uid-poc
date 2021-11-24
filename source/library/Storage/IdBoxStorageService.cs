using System;
using UniversalIdentity.Library.Cryptography;

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
            var identityStorage = this.IdBoxStorage.CreateSeedIdentity();
            return identityStorage;
        }
    }
}