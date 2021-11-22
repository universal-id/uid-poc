using System;
using Nethereum.Util;
using UniversalIdentity.Library.Cryptography;

namespace UniversalIdentity.Library.Runtime;

public class IdentityInteractionService
{
    public IdentityInteractionService(EthKey ethKey, string identifier)
    {
        this.EthKey = ethKey;
        this.Identifier = identifier;
    }

    public EthKey EthKey { get; set; }
    public string Identifier { get; set; }

    public byte[] RequestSignature(byte[] data, ControlType controlType = ControlType.Full)
    {
        if(this.EthKey.IsPublic)
        {
            throw new Exception("Cannot sign using a public Ethereum key.");
        }

        var dataHash = Sha3Keccack.Current.CalculateHash(data);
        var signature = EthKey.SignGetBytes(dataHash);

        return signature;
    }
    //public event byte[] SignatureRequested(byte[] data, ControlType controlType);
}
