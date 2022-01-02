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
using UniversalIdentity.Library.Interaction;

namespace UniversalIdentity.Library.Test.Infra.Interaction
{
    public class InteractionTestContext : TestContext
    {
        public InteractionTestContext(string testMethodName
            , TestsBase testClass) : base (testMethodName, testClass)
        {
            var tempPath = Path.GetTempPath();
            var uniqueString = this.Uuid.ToString();
            tempPath = Path.Combine(tempPath, uniqueString);
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            var interactionService = new InteractionService(tempPath);
            interactionService.Init();
            this.InteractionService = interactionService;
        }

        public InteractionService InteractionService { get; set; }
    }
}