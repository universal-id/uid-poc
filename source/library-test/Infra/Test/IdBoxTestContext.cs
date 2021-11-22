 using System;
//using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Xunit.Abstractions;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.CorrelationVector;
using Xunit;
using UniversalIdentity.Library.Test;
using UniversalIdentity.Library.Runtime;
using System.IO;

namespace UniversalIdentity.Library.Test.Infra
{
    public class IdBoxTestContext : TestContext
    {
        public IdBoxTestContext(string testMethodName
            , TestsBase testClass) : base (testMethodName, testClass)
        {
            IdBoxService = new IdBoxRuntimeService();
        }

        public IdBoxRuntimeService IdBoxService { get; set; }
        public CallContext CallContext { get; set; }

        internal string GenerateProfileImage()
        {
            throw new NotImplementedException();
        }

        public string GetTempPath()
        {
            var uuid = this.Uuid;
            var tempPath = Path.GetTempPath();
            tempPath = Path.Combine(tempPath, uuid.ToString());
            if (!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            return tempPath;
        }
    }
}