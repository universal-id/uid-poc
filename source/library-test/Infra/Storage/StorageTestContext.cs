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

namespace UniversalIdentity.Library.Test.Infra.Storage
{
    public class IdBoxStorageTestContext : TestContext
    {
        public IdBoxStorageTestContext(string testMethodName
            , TestsBase testClass) : base (testMethodName, testClass)
        {
            var tempPath = Path.GetTempPath();
            var uniqueString = this.Uuid.ToString() + "-id-box";
            tempPath = Path.Combine(tempPath, uniqueString);
            if( !Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            this.IdBoxStorage = new IdBoxStorage(tempPath);
        }

        public IdBoxStorage IdBoxStorage { get; set; }

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