using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
//using Nethereum.Signer.Crypto;

namespace UniversalIdentity.Library.Cryptography;

public class EthKey
{
    public EthKey()
    {
        this.IsPublic = false;
        this.EthECKey = EthECKey.GenerateKey();
    }

    public EthKey(string publicKey)
    {
        this.IsPublic = true;
        var publicKeyBytes = publicKey.HexToByteArray();
        this.EthECKey = new EthECKey(publicKeyBytes, false);
    }

    public EthECKey EthECKey { get; set; }
    public bool IsPublic { get; set; }

    public static EthKey CreateFromPublicKey(string publicKey)
    {
        var key = new EthKey();
        key.IsPublic = true;
        var publicKeyBytes = publicKey.HexToByteArray();
        key.EthECKey = new EthECKey(publicKeyBytes, false);
        return key;
    }

    public string GetPublicAddress()
    {            
        string publicAddress = this.EthECKey.GetPublicAddress();
        return publicAddress;
    }

    public string GetIdentifier()
    {            
        string publicAddress = this.EthECKey.GetPublicAddress();
        return $"did:eth:{publicAddress}";
    }

    public string GetPublicKey()
    {         
        byte[] publicKeyBytes = this.EthECKey.GetPubKey();   
        string publicKey = publicKeyBytes.ToHex(true);
        return publicKey;
    }

    public byte[] GetPublicKeyBytes()
    {
        byte[] publicKey = this.EthECKey.GetPubKey();
        return publicKey;
    }

    public byte[] GetPrivateKeyBytes()
    {
        byte[] privateKeyBytes = this.EthECKey.GetPrivateKeyAsBytes();
        return privateKeyBytes;
    }

    public string GetPrivateKey()
    {
        string privateKey = this.EthECKey.GetPrivateKey();
        return privateKey;
    }

    public string Sign(byte[] dataHash)
    {
        var signature = this.EthECKey.Sign(dataHash);
        var signatureString = "0x" + signature.R.ToHex().PadLeft(64, '0') +
                signature.S.ToHex().PadLeft(64, '0');
        return signatureString;
    }

    internal static EthKey CreateFromPrivateKey(string privateKey)
    {
        var key = new EthKey();
        key.IsPublic = true;
        var publicKeyBytes = privateKey.HexToByteArray();
        key.EthECKey = new EthECKey(publicKeyBytes, true);
        return key;
    }

    public byte[] SignGetBytes(byte[] dataHash)
    {
        var signature = this.EthECKey.Sign(dataHash);
        var signatureString = "0x" + signature.R.ToHex().PadLeft(64, '0') +
                signature.S.ToHex().PadLeft(64, '0');
        var signatureBytes = signatureString.HexToByteArray();   
        return signatureBytes;
    }

    public bool VerifyFromBytes(byte[] dataHash, byte[] signatureBytes)
    {
            byte[] rByteArray = new byte[32];
        Array.Copy(signatureBytes, 0, rByteArray, 0, 32);
        var rBigInteger = new Org.BouncyCastle.Math.BigInteger(1, rByteArray);

        byte[] sByteArray = new byte[32];
        Array.Copy(signatureBytes, 32, sByteArray, 0, 32);
        var sBigInteger = new Org.BouncyCastle.Math.BigInteger(1, sByteArray);

        var signatureObject = new EthECDSASignature(rBigInteger, sBigInteger, null);
        var isVerified = this.EthECKey.Verify(dataHash, signatureObject);
        return isVerified;
    }

    public bool Verify(byte[] dataHash, string signature)
    {
        byte[] signatureBytes = signature.HexToByteArray();
        return VerifyFromBytes(dataHash, signatureBytes);
    }
}