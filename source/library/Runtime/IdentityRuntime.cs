using UniversalIdentity.Library;
using UniversalIdentity.Library.Cryptography;
using UniversalIdentity.Library.Storage;

namespace UniversalIdentity.Library.Runtime;

public class IdentityRuntime
{
    public IdentityInteractionService? InteractionService { get; set; }

    public IdentityRuntime(EthKey ethKey)
    {
        var identifier = ethKey.GetPublicAddress();
        this.Identifier = identifier;

        var publicKeyIdentifier = ethKey.GetPublicIdentifier();
        this.Keys = new [] { new IdentityKey() { Identifier = publicKeyIdentifier } };
    }

    public string Identifier { get; set; }

    public IdentityKey[] Keys { get; set; }

    // public bool IsSeedIdentity()
    // {
    //     if(this.Identity.Keys.Count() == 1)
    //     {
    //         //if(this.Block == null)
    //         {
    //             var keyPart = this.Identity.Identifier.Replace("did:uid:", string.Empty);
    //             if (this.Identity.Keys.Single().Identifier.Contains(keyPart))
    //             {
    //                 return true;
    //             }
    //         }
    //     }

    //     return false;
    // }

    // //[JsonIgnore]
    // public bool IsBlockHistoryVerified { get; set; }
    
    // public bool IsCommitted { get; set; }
}