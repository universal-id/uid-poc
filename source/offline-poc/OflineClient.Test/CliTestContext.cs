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
using UniversalIdentity.Library.Test.Infra;

namespace UniversalIdentity.Cli.Test
{
    public class CliTestContext : TestContext
    {
        public CliTestContext(string testMethodName
            , TestsBase testClass) : base (testMethodName, testClass)
        {
            var tempPath = Path.GetTempPath();
            var uniqueString = this.Uuid.ToString() + "-id-box";
            var uniqueCliString = this.Uuid.ToString() + "-cli";
            tempPath = Path.Combine(tempPath, uniqueString);
            var tempCliPath = Path.Combine(tempPath, uniqueCliString);
            if(!Directory.Exists(tempPath)) Directory.CreateDirectory(tempPath);
            if(!Directory.Exists(tempCliPath)) Directory.CreateDirectory(tempCliPath);
            this.CliPath = tempCliPath;
            this.IdBoxPath = tempPath;
        }

        public string CliPath { get; set; }
        public string IdBoxPath { get; set; }
    }
}