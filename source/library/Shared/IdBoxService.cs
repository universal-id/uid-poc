using UniversalIdentity.Library.Cryptography;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Library;
using System;
using UniversalIdentity.Library.Communication;

namespace UniversalIdentity.Library;

public class IdBoxService
{
    public IdBoxService(string path)
    {
        this.Storage = new IdBoxStorage(path);
        this.Communication = new CommunicationService();
    }

    public IdBoxStorage Storage { get; }

    public CommunicationService Communication { get; set; }
}