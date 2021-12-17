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
using static UniversalIdentity.Library.Communication.BeaconProtocol;

namespace UniversalIdentity.Library.Communication
{
    [ProtoContract]
    public class BeaconMessage
    {
        [ProtoMember(1)]
        public BeaconMessageType Type { get; set; }
        [ProtoMember(2)]
        public BeaconConnectMessage? BeaconConnectMessage { get; set; }
    }

    [ProtoContract]
    public class BeaconConnectMessage
    {
        [ProtoMember(1)]
        public BeaconState State { get; set; }
        [ProtoMember(2)]
        public string? Identifier { get; set; }
        // [ProtoMember(3)]
        // public string? Name { get; set; }
    }

    public enum BeaconMessageType
    {
        Unspecified = 0,
        Connect = 1,
    }
}