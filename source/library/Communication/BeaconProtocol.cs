using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.CorrelationVector;
using System.IO;
using System.Linq;
using Ipfs;
using PeerTalk;
using PeerTalk.Discovery;
using PeerTalk.Protocols;
using Semver;
using System.Threading.Tasks;
using System.Threading;
using ProtoBuf;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;

namespace UniversalIdentity.Library.Communication
{
    public class BeaconProtocol : IPeerProtocol
    {
        public BeaconProtocol(Swarm swarm)
        {
            this.Swarm = swarm;
        }

        public string Name => "beacon";

        public SemVersion Version => "0.0";

        public Swarm Swarm { get; set; }

        public async Task ProcessMessageAsync(PeerConnection connection, Stream stream, CancellationToken cancel = default)
        {
            var request = await PeerTalk.ProtoBufHelper.ReadMessageAsync<BeaconMessage>(stream, cancel).ConfigureAwait(false);
            var response = new BeaconMessage
            {
                Type = request.Type,
            };
            switch (request.Type)
            {
                case BeaconMessageType.Connect:
                    response = ProcessConnectToBeacon(connection, request, response);
                    break;
                // default:
            }
            if (response != null)
            {
                ProtoBuf.Serializer.SerializeWithLengthPrefix(stream, response, PrefixStyle.Base128);
                await stream.FlushAsync(cancel).ConfigureAwait(false);
            }
        }

        public BeaconEndpoint ActivateBeacon(string identifier)
        {
            //var identity = this.IdentityContext.Repository.GetIdentity(identifier);
            var swarmListeningAddress = this.Swarm.StartListeningAsync(CommunicationService.LocalHostAnyPortAddress).Result;
            var connectBeacon = new BeaconEndpoint();
            connectBeacon.Address = swarmListeningAddress;
            connectBeacon.State = BeaconState.Active;
            connectBeacon.Identifier = identifier;
            //connectBeacon.Name = identity.Info["name"];

            this.LocalBeacon = connectBeacon;
            return connectBeacon;
        }

        public BeaconEndpoint RespondToBeacon(string address, string identifier)
        {
            var multiAddress = new MultiAddress(address);
            var peerConnection = this.Swarm.ConnectAsync(multiAddress).Result;
            var peer = peerConnection.RemotePeer;
            using (var stream = Swarm.DialAsync(peer, this.ToString()).Result)
            {  
                var beaconMessage = new BeaconMessage() 
                {
                    Type = BeaconMessageType.Connect,
                    BeaconConnectMessage = new BeaconConnectMessage()
                };                           
                ProtoBuf.Serializer.SerializeWithLengthPrefix(stream, beaconMessage, PrefixStyle.Base128);
                stream.FlushAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                var beaconResponseMessage = ProtoBufHelper.ReadMessageAsync<BeaconMessage>(stream).ConfigureAwait(false).GetAwaiter().GetResult();
                
                if(beaconResponseMessage.Type != BeaconMessageType.Connect)
                {
                    throw new Exception();
                }

                var beaconConnectResponseMessage = beaconResponseMessage.BeaconConnectMessage;
                var remoteBeacon = new BeaconEndpoint(isRemote: true)
                {
                    PeerConnection = peerConnection,
                    //Name = beaconConnectResponseMessage.Name,
                    Identifier = beaconConnectResponseMessage.Identifier,
                    State = beaconConnectResponseMessage.State
                };
                return remoteBeacon;
            }
        }

        public BeaconEndpoint? LocalBeacon { get; set; }

        public async Task<BeaconEndpoint> ConnectToBeacon(MultiAddress address)
        {
            var peerConnection = await Swarm.ConnectAsync(address);
            var peer = peerConnection.RemotePeer;
            using (var stream = await Swarm.DialAsync(peer, this.ToString()))
            {  
                var beaconMessage = new BeaconMessage() 
                {
                    Type = BeaconMessageType.Connect,
                    BeaconConnectMessage = new BeaconConnectMessage()
                };                           
                ProtoBuf.Serializer.SerializeWithLengthPrefix(stream, beaconMessage, PrefixStyle.Base128);
                await stream.FlushAsync().ConfigureAwait(false);

                var beaconResponseMessage = await ProtoBufHelper.ReadMessageAsync<BeaconMessage>(stream).ConfigureAwait(false);
                
                var beaconConnectResponseMessage = beaconResponseMessage.BeaconConnectMessage;
                if(beaconResponseMessage.Type != BeaconMessageType.Connect
                    || beaconConnectResponseMessage == null)
                {
                    throw new Exception();
                }

                var remoteBeacon = new BeaconEndpoint(isRemote: true)
                {
                    PeerConnection = peerConnection,
                    // Name = beaconConnectResponseMessage.Name,
                    Identifier = beaconConnectResponseMessage!.Identifier,
                    State = beaconConnectResponseMessage.State
                };
                return remoteBeacon;
            }
        }

        private BeaconMessage ProcessConnectToBeacon(PeerConnection peerConnection, BeaconMessage request, BeaconMessage response)
        {
            if (this.LocalBeacon.State != BeaconState.Active)
            {
                return null;
            }

            var remotePeer = peerConnection.RemotePeer;
            BeaconEndpoint respondingReceiverBeaconEndpoint = null;
            if(respondingReceiverBeaconEndpoint == null)
            {
                respondingReceiverBeaconEndpoint = new BeaconEndpoint(isReceiver: true, isRemote: true)
                {
                    PeerConnection = peerConnection,
                };
                //this.RespondingReceivers.Add(remotePeer, respondingReceiverBeaconEndpoint);
            }
            else
            {
                respondingReceiverBeaconEndpoint.PeerConnection = peerConnection;
            }

            var beaconConnectMessage = new BeaconConnectMessage()
            {
                Identifier = this.LocalBeacon.Identifier,
                //Name = this.LocalBeacon.Name,
                State = this.LocalBeacon.State
            };
            response.BeaconConnectMessage = beaconConnectMessage;
            return response;
        }
    }

    public class BeaconEndpoint
    {
        public BeaconEndpoint(bool isRemote = false, bool isReceiver = false)
        {
            this.IsRemote = isRemote;
            this.IsReceiver = isReceiver;
        }

        public string? Identifier { get; set; }
        // public string Name { get; set; }
        public BeaconState State { get; set; }
        public bool IsRemote { get; set; }
        public bool IsReceiver { get; set; }
        public PeerConnection? PeerConnection { get; set; }
        public MultiAddress? Address { get; set; }
    }

    public enum BeaconState
    {
        Unspecified,
        Active,
        Inactive
    }
}
