
using UniversalIdentity.Library.Cryptography;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Library;
using System;

namespace UniversalIdentity.Library.Runtime;

public class IdBoxRuntimeService
{
    public IdBoxRuntime GetIdBoxRuntime()
    {
        return this.IdBoxRuntime;
    }

    public IdBoxStorage? IdBoxStorage { get; set; }

    public IdBoxRuntime? IdBoxRuntime { get; set; }
    public IdentityRuntime? MainIdentity { get; set; }
    public IdBoxStorageService IdBoxStorageService { get; set; }

    public object SetPropertyInfo(IdentityInfo name, string v)
    {
        throw new NotImplementedException();
    }

    public void DeletePropertyInfo(IdentityInfo image)
    {
        throw new NotImplementedException();
    }

    public object GetPropertyInfo(IdentityInfo image)
    {
        throw new NotImplementedException();
    }

    public IdBoxStorageService CreateIdentityBox(string tempPath)
    {
        var idBoxService = new IdBoxStorageService(tempPath);
        idBoxService.InitializeStorage();
        this.IdBoxStorage = idBoxService.IdBoxStorage;
        return idBoxService;
    }

    public IdentityRuntime CreateMainIdentity(EthKey ethKey)
    {
        var identity = new IdentityRuntime(ethKey);
        var identityInteractionService = new IdentityInteractionService(ethKey, identity.Identifier);
        identity.InteractionService = identityInteractionService;

        this.CommitIdentity(identity);

        this.MainIdentity = identity;
        return identity;
    }

    public void CommitIdentity(IdentityRuntime identity)
    {
        throw new NotImplementedException();
    }
}