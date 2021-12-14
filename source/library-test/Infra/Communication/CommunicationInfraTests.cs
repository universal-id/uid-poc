using Xunit;
using Xunit.Abstractions;
using UniversalIdentity.Library.Storage;
using FluentAssertions;
using Nethereum.Util;
using Nethereum.Hex.HexConvertors.Extensions;
using System.IO;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using UniversalIdentity.Library.Cryptography;
using UniversalIdentity.Library.Communication;

namespace UniversalIdentity.Library.Test.Infra.Communication;

public class CommunicationInfraTests : TestsBase
{
    public CommunicationInfraTests(ITestOutputHelper outputHelper) : base(outputHelper) {} // Wires up test logging

    [Fact] /// Developer can programmatically:
    /// - Create a communication service object which in turn:
    /// - Starts a swarm with no protocols
    public void CreateCommunicationServiceTest()
    {
        using (var testContext = new TestContext(nameof(CreateCommunicationServiceTest), this))
        {
            var communicationService = new CommunicationService();
            
            var swarm = communicationService.Swarm;
            swarm.Should().NotBeNull();
            swarm.IsRunning.Should().BeTrue();

            communicationService.BeaconProtocol.Should().NotBeNull();

            communicationService.Dispose();
            swarm.IsRunning.Should().BeFalse();
        }
    }

    [Fact] /// Developer can programmatically:
    /// - Start a set of communication services using the test context
    public void StartCommunicationServicesTest()
    {
        using (var testContext = new CommunicationTestContext(nameof(StartCommunicationServicesTest), this))
        {
            var firstPerson = testContext.FirstPerson;
            firstPerson.Communication.Should().NotBeNull();
            var swarm1 = firstPerson.Communication.Swarm;
            swarm1.Should().NotBeNull();
            swarm1.IsRunning.Should().BeTrue();
            firstPerson.Communication.BeaconProtocol.Should().NotBeNull();
            
            var secondPerson = testContext.SecondPerson;
            secondPerson.Communication.Should().NotBeNull();
            var swarm2 = secondPerson.Communication.Swarm;
            swarm2.Should().NotBeNull();
            swarm2.IsRunning.Should().BeTrue();
            secondPerson.Communication.BeaconProtocol.Should().NotBeNull();
        }   
    }

    [Fact] /// Developer can programmatically:
    /// - Activeate the beacon for connections
    public void ActivateBeaconTest()
    {
        using (var testContext = new CommunicationTestContext(nameof(StartCommunicationServicesTest), this))
        {
            var firstPerson = testContext.FirstPerson;
            var swarm1 = firstPerson.Communication.Swarm;
            var beaconProtocol1 = firstPerson.Communication.BeaconProtocol;
            beaconProtocol1.ActivateBeacon(string.Empty);

            beaconProtocol1.LocalBeacon.Should().NotBeNull();
            var beaconEndpoint1 = beaconProtocol1.LocalBeacon!;
            beaconEndpoint1.State.Should().Be(BeaconState.Active);
            beaconEndpoint1.IsRemote.Should().BeFalse();
            //beaconEndpoint.IsReceiver.Should().BeTrue();
            beaconEndpoint1.Address.Should().NotBeNull();
            var address = beaconEndpoint1.Address!;

            var secondPerson = testContext.SecondPerson;
            var swarm2 = secondPerson.Communication.Swarm;
            var beaconProtocol2 = secondPerson.Communication.BeaconProtocol;
            var beaconEndpoint2 = beaconProtocol2.ConnectToBeacon(address).Result;
            beaconEndpoint2.IsRemote.Should().BeTrue();
            //beaconEndpoint2.Address.Should().NotBeNull();
        }   
    }
}