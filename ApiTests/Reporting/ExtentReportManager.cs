using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ApiTests.Reporting
{
    [SetUpFixture]
    public class ExtentReportManager
    {
        private static ExtentReports _extent;
        private static ExtentTest _currentTest;

        [OneTimeSetUp]
        public void SetupReporting()
        {
            string reportPath = Path.Combine(NUnit.Framework.TestContext.CurrentContext.TestDirectory, "TestReports");
            Directory.CreateDirectory(reportPath);

            var sparkReporter = new ExtentSparkReporter(Path.Combine(reportPath, "ApiTestReport.html"));

            _extent = new ExtentReports();
            _extent.AttachReporter(sparkReporter);

            var config = new ConfigurationBuilder()
                .SetBasePath(NUnit.Framework.TestContext.CurrentContext.TestDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            _extent.AddSystemInfo("OS", Environment.OSVersion.VersionString);
            _extent.AddSystemInfo(".NET Version", Environment.Version.ToString());
            _extent.AddSystemInfo("Base URL", config["ApiConfig:BaseUrl"]);
        }

        [SetUp]
        public void SetUpTest()
        {
            _currentTest = _extent.CreateTest(NUnit.Framework.TestContext.CurrentContext.Test.Name);
            _currentTest.Log(Status.Info, $"Test started: {NUnit.Framework.TestContext.CurrentContext.Test.Name}");
        }

        [TearDown]
        public void TearDownTest()
        {
            var status = NUnit.Framework.TestContext.CurrentContext.Result.Outcome.Status;

            if (status == TestStatus.Failed)
            {
                _currentTest.Fail($"Test Failed: {NUnit.Framework.TestContext.CurrentContext.Result.Message}");
                _currentTest.Log(Status.Error, "Stack Trace: " + NUnit.Framework.TestContext.CurrentContext.Result.StackTrace);
            }
            else if (status == TestStatus.Skipped || status == TestStatus.Inconclusive)
            {
                _currentTest.Skip($"Test Skipped: {NUnit.Framework.TestContext.CurrentContext.Result.Message}");
            }
            else
            {
                _currentTest.Pass("Test Passed.");
            }
        }

        [OneTimeTearDown]
        public void TearDownReporting()
        {
            _extent.Flush();
        }

        public static ExtentTest CurrentTest => _currentTest;
    }
}