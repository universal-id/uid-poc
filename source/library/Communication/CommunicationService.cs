using UniversalIdentity.Library.Cryptography;
using UniversalIdentity.Library.Storage;
using UniversalIdentity.Library;
using System;
using PeerTalk;

namespace UniversalIdentity.Library.Communication;

public class CommunicationService : IDisposable
{
    public const string LocalHostAnyPortAddress = "/ip4/0.0.0.0/tcp/0";

    public CommunicationService()
    {
        var (peerKey, peer) = PeerHelper.GenerateKeyAndPeer();
        
        this.Swarm = new Swarm { LocalPeer = peer };
        var beaconProtocol = new BeaconProtocol(this.Swarm);// {IdentityContext = this };
        //var identityInterchangeProtocol = new IdentityInterchangeProtocol() { Swarm = this.Swarm, IdentityContext = this };
        this.Swarm.AddProtocol(beaconProtocol);
        //this.Swarm.AddProtocol(identityInterchangeProtocol);
        this.BeaconProtocol = beaconProtocol;
        //this.IdentityInterchangeProtocol = identityInterchangeProtocol;
        this.Swarm.StartAsync().Wait();
        //this.MulticastDns = WireUpNewMulticast();
    }

    public Swarm Swarm { get; set; }
    public BeaconProtocol BeaconProtocol { get; }

    public void Dispose()
    {
        this.Swarm.StopAsync().Wait();
    }
}