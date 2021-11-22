using System;
using System.Collections.Generic;

namespace UniversalIdentity.Library.Storage
{
    public class IdBoxStorage
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

        public Dictionary<string, IdentityStorage> Identities = new Dictionary<string, IdentityStorage>(StringComparer.OrdinalIgnoreCase);

        public string? Main { get; set; }

        public void Save()
        {

        }

        public void Load()
        {

        }


    }
}