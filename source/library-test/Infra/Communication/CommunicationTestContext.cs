 using System;
//using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Xunit.Abstractions;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.CorrelationVector;
using Xunit;
using UniversalIdentity.Library.Test;
using UniversalIdentity.Library.Runtime;
using UniversalIdentity.Library.Storage;
using System.IO;

namespace UniversalIdentity.Library.Test.Infra.Communication
{
    public class CommunicationTestContext : TestContext
    {
        public CommunicationTestContext(string testMethodName
            , TestsBase testClass) : base (testMethodName, testClass)
        {
            var tempPath = Path.GetTempPath();
            var uniqueString1 = "111" + this.Uuid.ToString().Substring(3,this.Uuid.ToString().Length - 3 - 1) + "-id-box";;
            var uniqueString2 = "222" + this.Uuid.ToString().Substring(3,this.Uuid.ToString().Length - 3 - 1) + "-id-box"; ;
            var tempPath1 = Path.Combine(tempPath, uniqueString1);
            var tempPath2 = Path.Combine(tempPath, uniqueString2);
            if( !Directory.Exists(tempPath1)) Directory.CreateDirectory(tempPath1);
            if( !Directory.Exists(tempPath2)) Directory.CreateDirectory(tempPath2);
             this.FirstPerson = new IdBoxService(tempPath1);
             this.SecondPerson = new IdBoxService(tempPath2);
        }

        public IdBoxService FirstPerson { get; set; }
        public IdBoxService SecondPerson { get; set; }

        public override void Dispose()
        {
            this.FirstPerson.Communication.Dispose();
            this.SecondPerson.Communication.Dispose();
            // this.FirstPerson.Communication.Swarm.StopAsync().Wait();
            // this.SecondPerson.Communication.Swarm.StopAsync().Wait();
            // this.LocalStorageHost.Dispose();
            base.Dispose();
        }
    }
}