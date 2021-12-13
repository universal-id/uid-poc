using Nethereum.Signer;
using Nethereum.Hex.HexConvertors.Extensions;
using System;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;

namespace UniversalIdentity.Library.Communication
{
    public class PeerKey
        {
            public PeerKey()
            {
                // From here: https://github.com/Nethereum/Nethereum/blob/a8e086e850edbaea26c3e1e46207b7749a1a8834/src/Nethereum.Signer/EthECKey.cs#L100
                var generator = new ECKeyPairGenerator("EC");
                var secureRandom = new Org.BouncyCastle.Security.SecureRandom();
                var keyGenerationParameters = new KeyGenerationParameters(secureRandom, 256);
                generator.Init(keyGenerationParameters);
                var keyPair = generator.GenerateKeyPair();
                this.PrivateKeyParameters = (ECPrivateKeyParameters)keyPair.Private;
                this.PublicKeyParameters = (ECPublicKeyParameters)keyPair.Public;
                
                // var privateBytes = ecPrivateKeyParameters.D.ToByteArray();
                // var peerTalkKey = PeerTalk.Cryptography.Key.CreatePrivateKey(ecPrivateKeyParameters);
                // var q = ecPrivateKeyParameters.Parameters.G.Multiply(ecPrivateKeyParameters.D);
                // var ecPublicKeyParameters = new ECPublicKeyParameters(q, ecPrivateKeyParameters.Parameters);
                
                // // Curve
                // var curveName = "P-256";
                // X9ECParameters ecP = ECNamedCurveTable.GetByName(curveName);
                // var curve = ecP.Curve;
                // var isCompressed = false;
                // var publicKeyBytes = curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(isCompressed);

                // From here: https://github.com/Nethereum/Nethereum/blob/7897b8b247a3a97fa3eef5684ab31fb8cf7f4879/src/Nethereum.Signer/Crypto/ECKey.cs#L64
                // var q = publicKeyParameters.Q;
                // //Pub key (q) is composed into X and Y, the compressed form only include X, which can derive Y along with 02 or 03 prepent depending on whether Y in even or odd.
                // q = q.Normalize();
                //var publicKeyBytes = Secp256k1.Curve.CreatePoint(q.XCoord.ToBigInteger(), q.YCoord.ToBigInteger()).GetEncoded(isCompressed);
            }

            public ECPrivateKeyParameters PrivateKeyParameters { get; set; }
            public ECPublicKeyParameters PublicKeyParameters { get; set; }

            public byte[] GetPublicKeyBytes()
            {
                var publicKeybytes = this.PublicKeyParameters.Q.GetEncoded(compressed: false);
                //var publicKeyBase64NoPad = publicKeybytes.ToBase64NoPad();
                return publicKeybytes;
            }

        }
}