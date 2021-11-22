using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using MartinCostello.Logging.XUnit;

namespace UniversalIdentity.Library.Test.Infra
{
    // Abstract tests class that exposes test logging functionality for extending classes
    public abstract class TestsBase
    {
        public TestsBase(ITestOutputHelper outputHelper) { OutputHelper = outputHelper; }
        public ITestOutputHelper OutputHelper { get; }
        public ILogger<T> GetLogger<T>()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XUnitLoggerProvider(this.OutputHelper, new XUnitLoggerOptions()));
            var logger = loggerFactory.CreateLogger<T>();
            return logger;
        }
    }
}