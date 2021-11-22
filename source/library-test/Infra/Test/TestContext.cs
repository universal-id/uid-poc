using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Xunit.Abstractions;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.CorrelationVector;
using Xunit;
using UniversalIdentity.Library.Test;

namespace UniversalIdentity.Library.Test.Infra
{
    public class TestContext : IDisposable
    {
        public TestContext(string testMethodName, TestsBase testClass)
        {
            this.MethodName = testMethodName;
            this.Class = testClass; 
            this.Uuid = Guid.NewGuid();
            // this.Vector = CorrelationVector.Spin(SessionVector.Value, new SpinParameters
            // {
            //     Interval = SpinCounterInterval.Fine, // Needed to distinguish between tests since they can finish within a few milliseconds
            //     Periodicity = SpinCounterPeriodicity.Short,
            //     Entropy = SpinEntropy.None
            // });
            // CallContextLazy = new Lazy<CallContext>(delegate()
            // {
            //     var callContext = CreateTestRootContext(this.Uuid, this.Class, this.MethodName, this.Vector);
            //     return callContext; 
            // }, true);   

            StartingTest();    
        }

        public void StartingTest()
        {
            // this.LoggerScope = this.CallContext.GetLogger(this.Class.GetType())
            //      .BeginScope(new Dictionary<string, object> {
            //                         { "TestMethod", this.MethodName },
            //                         { "Vector", this.Vector.Value },
            //                         { "TaskUuid", this.Uuid.ToString() },
            //                         { "SessionUuid", SessionUuid.ToString() },
            //          });
            // this.Logger.LogInformation($"Starting test {this.MethodName}, Vector: {this.Vector.Value}, Guid: {this.CallContext.Uuid}");
        }

        public void Info(string message)
        {
            this.Class.GetLogger<TestsBase>().LogInformation(message);
        }

        public string MethodName { get; }
        public TestsBase Class { get; }
        public Guid Uuid { get; }
        //public Lazy<CallContext> CallContextLazy;
        //public CallContext CallContext { get { return this.CallContextLazy.Value; } }
        //public IDisposable LoggerScope { get; set; }
        //public CorrelationVector Vector { set; get; }
        //public ILogger Logger { get { return this.CallContext.GetLogger(this.Class.GetType()); } }

        //public static Guid SessionUuid = Guid.NewGuid();
        //public static CorrelationVector SessionVector = new CorrelationVector(SessionUuid);

        public virtual void Dispose()
        {
            // this.Logger.LogInformation($"Ending test {this.MethodName}, Vector: {this.Vector.Value}, Guid: {this.CallContext.Uuid}");
            // if (this.LoggerScope != null) this.LoggerScope.Dispose();
        }

        // public CallContext CreateTestRootContext(Guid testUuid, Tests testClass, string testMethodName
        //     , CorrelationVector vector)
        // {
        //     var services = new ServiceCollection();
        //     services.AddLogging(builder =>
        //     {
        //         // Optional: Apply filters to configure LogLevel Trace or above is sent to
        //         // Application Insights for all categories.

        //         // Add console logger
        //         builder.AddConsole();

        //         // Add XUnit logger for running tests
        //         builder.AddXUnit(testClass.OutputHelper);
        //     });
        //     IServiceProvider serviceProvider = services.BuildServiceProvider();

        //     var callContext = new CallContext()
        //     {
        //         Name = testMethodName,
        //         Uuid = testUuid,
        //         ServiceCollection = services,
        //         ServiceProvider = serviceProvider,
        //         Vector = vector
        //     };
        //     return callContext;            
        // }
    }
}
