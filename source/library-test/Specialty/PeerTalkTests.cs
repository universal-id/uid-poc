using System;
using Xunit;
using Nethereum.Signer;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Linq;
using FluentAssertions;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Nethereum.HdWallet;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Asn1.X9;
using Ipfs;
using PeerTalk;
using System.Threading.Tasks;
using System.Threading;
using PeerTalk.Discovery;
using Makaretu.Dns;
using PeerTalk.Protocols;
using Semver;
using System.IO;
using System.Text;
using UniversalIdentity.Library.Test.Infra;
using UniversalIdentity.Library.Communication;

namespace UniversalIdentity.Library.Test.Specialized
{
    public class PeerTalkTests : TestsBase
    {
        public PeerTalkTests(ITestOutputHelper outputHelper) : base(outputHelper) {}

        [Fact]
        public void PeerKeyGenerationTest()
        {
            using (var testContext = new TestContext(nameof(PeerKeyGenerationTest), this))
            {
                var (peerKey, peer) = PeerHelper.GenerateKeyAndPeer();
            
                testContext.Info($"peer.Id: {peer.Id}");
                testContext.Info($"peer.PublicKey: {peer.PublicKey}"); 

                peer.Id.Should().NotBeNull();        
                peer.Id.ToString().Should().NotBeNullOrEmpty();        
                peer.PublicKey.Should().NotBeNullOrEmpty();
            }            
        }

        [Fact]
        public async Task ListenAndConnectTest()
        {
            using (var testContext = new TestContext(nameof(ListenAndConnectTest), this))
            {
                var (firstPeerKey, firstPeer) = PeerHelper.GenerateKeyAndPeer();
                var (secondPeerKey, secondPeer) = PeerHelper.GenerateKeyAndPeer();
            
                //MultiAddress localHostAnyPortAddress = "/ip4/127.0.0.1/tcp/0";
                MultiAddress localHostAnyPortAddress = "/ip4/0.0.0.0/tcp/0";
                var firstSwarm = new Swarm { LocalPeer = firstPeer };
                var secondSwarm = new Swarm { LocalPeer = secondPeer };
                firstSwarm.ConnectionEstablished += (s, e) =>
                {
                    testContext.Info($"First swarm > Connection established. RemotePeer.Id:{e.RemotePeer.Id}");
                    testContext.Info($"\tIsIncoming:{e.IsIncoming}");
                };

                await firstSwarm.StartAsync();
                await secondSwarm.StartAsync();

                try
                {
                    var firstSwarmListeningAddress = await firstSwarm.StartListeningAsync(localHostAnyPortAddress);
                    testContext.Info($"First swarm address: {firstSwarmListeningAddress}");
                    firstPeer.Addresses.Should().Contain(firstSwarmListeningAddress);
                    testContext.Info($"First peer addresses: {string.Join(',', firstPeer.Addresses)}");


                    var connection = await secondSwarm.ConnectAsync(firstSwarmListeningAddress);
                    testContext.Info($"Connected to remote peer with Id: {connection.RemotePeer.Id}");
                    testContext.Info($"\tRemote address: {connection.RemoteAddress}");

                    secondSwarm.KnownPeers.Should().Contain(firstPeer);
                    firstSwarm.KnownPeers.Should().Contain(secondPeer);

                    await firstSwarm.StopListeningAsync(localHostAnyPortAddress);
                    firstPeer.Addresses.Should().NotContain(firstSwarmListeningAddress);
                }
                finally
                {
                    await firstSwarm.StopAsync();
                    await secondSwarm.StopAsync();
                }
            }            
        }

        [Fact]
        public async Task MulticastDnsDiscoveryTest()
        {
            using (var testContext = new TestContext(nameof(MulticastDnsDiscoveryTest), this))
            {
                var (firstPeerKey, firstPeer) = PeerHelper.GenerateKeyAndPeer();
                var (secondPeerKey, secondPeer) = PeerHelper.GenerateKeyAndPeer();

                firstPeer.Addresses = new MultiAddress[] { $"/ip4/104.131.131.82/tcp/4001/ipfs/{firstPeer.Id}" };
                secondPeer.Addresses = new MultiAddress[] { $"/ip4/104.131.131.82/tcp/4001/ipfs/{secondPeer.Id}" };

                var discoveryCompletedEvent = new ManualResetEvent(false);
                var firstMulticastDns = new MdnsNext
                {
                    MulticastService = new MulticastService(),
                    LocalPeer = firstPeer
                };
                var secondMulticastDns = new MdnsNext
                {
                    MulticastService = new MulticastService(),
                    LocalPeer = secondPeer
                };
                firstMulticastDns.PeerDiscovered += (s, e) =>
                {
                    testContext.Info($"First MDNS> Peer discovered. Id:{e.Id} - Addresses:{string.Join(',',e.Addresses)}");
                    if (e.Id == secondPeer.Id)
                    {
                        discoveryCompletedEvent.Set();
                    }
                };

                secondMulticastDns.PeerDiscovered += (s, e) =>
                {
                    testContext.Info($"Second MDNS> Peer discovered. Id:{e.Id} - Addresses:{string.Join(',',e.Addresses)}");
                };

                await firstMulticastDns.StartAsync();
                firstMulticastDns.MulticastService.Start();
                await secondMulticastDns.StartAsync();
                secondMulticastDns.MulticastService.Start();
     
                try
                {
                    discoveryCompletedEvent.WaitOne(TimeSpan.FromSeconds(2)).Should().BeTrue();
                }
                finally
                {
                    await firstMulticastDns.StopAsync();
                    await secondMulticastDns.StopAsync();
                    firstMulticastDns.MulticastService.Stop();
                    secondMulticastDns.MulticastService.Stop();
                }
            }
        }

        // [Fact]
        // public async Task MulticastDnsListenAndConnectTest()
        // {
        //     using (var testContext = new TestContext(nameof(MulticastDnsListenAndConnectTest), this))
        //     {
        //         var (firstPeerKey, firstPeer) = PeerHelper.GenerateKeyAndPeer();
        //         var (secondPeerKey, secondPeer) = PeerHelper.GenerateKeyAndPeer();
            
        //         MultiAddress localHostAnyPortAddress = "/ip4/0.0.0.0/tcp/0";
        //         var firstSwarm = new Swarm { LocalPeer = firstPeer };
        //         var secondSwarm = new Swarm { LocalPeer = secondPeer };
        //         firstSwarm.ConnectionEstablished += (s, e) =>
        //         {
        //             testContext.Info($"First swarm > Connection established. RemotePeer.Id:{e.RemotePeer.Id}");
        //             testContext.Info($"\tIsIncoming:{e.IsIncoming}");
        //         };

        //         var discoveryCompletedEvent = new ManualResetEvent(false);
        //         var firstMulticastDns = new MdnsNext
        //         {
        //             MulticastService = new MulticastService(),
        //             LocalPeer = firstPeer
        //         };
        //         var secondMulticastDns = new MdnsNext
        //         {
        //             MulticastService = new MulticastService(),
        //             LocalPeer = secondPeer
        //         };
        //         MultiAddress transmittedFirstSwarmAddress = null;
        //         secondMulticastDns.PeerDiscovered += (s, e) =>
        //         {
        //             testContext.Info($"Second MDNS> Peer discovered. Id:{e.Id} - Addresses:{string.Join(',',e.Addresses)}");
        //             if (e.Id == firstPeer.Id)
        //             {
        //                 transmittedFirstSwarmAddress = e.Addresses.Where(a => a.ToString().Contains("127.0.0.1")).First();

        //                 testContext.Info($"Connecting to: {transmittedFirstSwarmAddress}");
        //                 var connection = secondSwarm.ConnectAsync(transmittedFirstSwarmAddress).Result;
        //                 testContext.Info($"Connected to remote peer with Id: {connection.RemotePeer.Id}");
        //                 testContext.Info($"\tRemote address: {connection.RemoteAddress}");

        //                 discoveryCompletedEvent.Set();
        //             }
        //         };

        //         await firstSwarm.StartAsync();
        //         await secondSwarm.StartAsync();
        //         await secondMulticastDns.StartAsync();
        //         secondMulticastDns.MulticastService.Start(); 

        //         try
        //         {
        //             var firstSwarmListeningAddress = await firstSwarm.StartListeningAsync(localHostAnyPortAddress);
        //             testContext.Info($"First swarm address: {firstSwarmListeningAddress}");
        //             firstPeer.Addresses.Should().Contain(firstSwarmListeningAddress);

        //             // Start a new multicast service after listening on port
        //             await firstMulticastDns.StartAsync();
        //             firstMulticastDns.MulticastService.Start();

        //             discoveryCompletedEvent.WaitOne(TimeSpan.FromSeconds(3)).Should().BeTrue();
        //             transmittedFirstSwarmAddress.Should().BeEquivalentTo(firstSwarmListeningAddress);   

        //             secondSwarm.KnownPeers.Should().Contain(firstPeer);
        //             firstSwarm.KnownPeers.Should().Contain(secondPeer);      

        //             await firstSwarm.StopListeningAsync(localHostAnyPortAddress);
        //             firstPeer.Addresses.Should().NotContain(transmittedFirstSwarmAddress);
        //         }
        //         finally
        //         {
        //             await firstMulticastDns.StopAsync();
        //             await secondMulticastDns.StopAsync();
        //             firstMulticastDns.MulticastService.Stop();
        //             secondMulticastDns.MulticastService.Stop();

        //             await firstSwarm.StopAsync();
        //             await secondSwarm.StopAsync();
        //         }
        //     }            
        // }

          [Fact]
        public async Task CustomPingProtocolTest()
        {
            using (var testContext = new TestContext(nameof(ListenAndConnectTest), this))
            {
                var (firstPeerKey, firstPeer) = PeerHelper.GenerateKeyAndPeer();
                var (secondPeerKey, secondPeer) = PeerHelper.GenerateKeyAndPeer();
            
                //MultiAddress localHostAnyPortAddress = "/ip4/127.0.0.1/tcp/0";
                MultiAddress localHostAnyPortAddress = "/ip4/0.0.0.0/tcp/0";
                var firstSwarm = new Swarm { LocalPeer = firstPeer };
                var firstSwarmPingProtocol = new TestPingProtocol() { Swarm = firstSwarm };
                firstSwarm.AddProtocol(firstSwarmPingProtocol);
                var secondSwarm = new Swarm { LocalPeer = secondPeer };
                var secondSwarmPingProtocol = new TestPingProtocol(){ Swarm = secondSwarm };
                secondSwarm.AddProtocol(secondSwarmPingProtocol);

                firstSwarm.ConnectionEstablished += (s, e) =>
                {
                    testContext.Info($"First swarm > Connection established. RemotePeer.Id:{e.RemotePeer.Id}");
                    testContext.Info($"\tIsIncoming:{e.IsIncoming}");
                };

                await firstSwarm.StartAsync();
                await secondSwarm.StartAsync();

                try
                {
                    var firstSwarmListeningAddress = await firstSwarm.StartListeningAsync(localHostAnyPortAddress);
                    testContext.Info($"First swarm address: {firstSwarmListeningAddress}");
                    firstPeer.Addresses.Should().Contain(firstSwarmListeningAddress);
                    testContext.Info($"First peer addresses: {string.Join(',', firstPeer.Addresses)}");


                    var connection = await secondSwarm.ConnectAsync(firstSwarmListeningAddress);
                    testContext.Info($"Connected to remote peer with Id: {connection.RemotePeer.Id}");
                    testContext.Info($"\tRemote address: {connection.RemoteAddress}");

                    secondSwarm.KnownPeers.Should().Contain(firstPeer);
                    firstSwarm.KnownPeers.Should().Contain(secondPeer);

                    var input = "SomeString";
                    var output = firstSwarmPingProtocol.Ping(secondPeer, input).Result;
                    output.Should().BeEquivalentTo(input);

                    await firstSwarm.StopListeningAsync(localHostAnyPortAddress);
                    firstPeer.Addresses.Should().NotContain(firstSwarmListeningAddress);
                }
                finally
                {
                    await firstSwarm.StopAsync();
                    await secondSwarm.StopAsync();
                }
            }            
        }

        public class TestPingProtocol : IPeerProtocol
        {
            public string Name { get; } = "test/ping";

            public SemVersion Version { get; } = new SemVersion(1, 0);

            public Swarm Swarm { get; set; }


            public override string ToString() { return $"/{Name}/{Version}"; }

            public async Task ProcessMessageAsync(PeerConnection connection, Stream stream, CancellationToken cancel = default)
            {
                while (true)
                {
                    var requestLength = await stream.ReadVarint32Async().ConfigureAwait(false);
                    var request = new byte[requestLength];
                    await stream.ReadExactAsync(request, 0, requestLength, cancel).ConfigureAwait(false);

                    // Echo the message
                    await stream.WriteVarintAsync(requestLength).ConfigureAwait(false);
                    await stream.WriteAsync(request, 0, requestLength, cancel).ConfigureAwait(false);
                    await stream.FlushAsync(cancel).ConfigureAwait(false);
                }
            }

            public async Task<string> Ping(Peer peer, string input, CancellationToken cancel = default)
            {
                using (var stream = await Swarm.DialAsync(peer, this.ToString(), cancel))
                {
                    var message = Encoding.UTF8.GetBytes(input);
                    await stream.WriteVarintAsync(message.Length);
                    stream.Write(message, 0, message.Length);
                    stream.Flush();

                    var responseSize = await stream.ReadVarint32Async();
                    var responseBytes = new byte[responseSize];
                    await stream.ReadExactAsync(responseBytes, 0, responseSize, cancel).ConfigureAwait(false);
                    var response = Encoding.UTF8.GetString(responseBytes);
                    return response;
                }
            }
        }
    }
}
