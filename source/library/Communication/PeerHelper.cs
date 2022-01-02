using System;
using Ipfs;
using Microsoft.Extensions.Logging;
using UniversalIdentity.Library.Cryptography;

namespace UniversalIdentity.Library.Communication
{
    public static class PeerHelper
    {
        public static (PeerKey, Peer) GenerateKeyAndPeer()
        {
            var peerKey = new PeerKey();
            var publicKeyBytes = peerKey.GetPublicKeyBytes();
            var peerId = MultiHash.ComputeHash(publicKeyBytes);
            var publicKeyBase64 = System.Convert.ToBase64String(publicKeyBytes);
            //var publicKeyBase64NoPad = publicKeyBytes.ToBase64NoPad();
            var peer = new Peer()
            {
                Id = peerId,
                PublicKey = publicKeyBase64
                //PublicKey = publicKeyBase64NoPad
            };
            return (peerKey, peer);
        }
        
    }
}