using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using Web_UI_Automation.Core;
using Web_UI_Automation.Pages;
using Shouldly;
using Allure.NUnit.Attributes;
using System.IO;
using Allure.NUnit;
using Allure;
using System;
using NUnit.Framework.Interfaces;

namespace Web_UI_Automation.NUnit
{
    [AllureNUnit]
    [TestFixture]
    [Category("EHU_Website")]
    public class NUnitTests
    {
        private IWebDriver _driver;
        private EhuHomePage _homePage;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            LoggerManager.Logger.Information("NUnit OneTimeSetup started.");
            _driver = WebDriverManager.Instance.GetDriver();
            _homePage = new EhuHomePage(_driver);
        }

        [SetUp]
        public void Setup()
        {
            _driver.Navigate().GoToUrl("about:blank");
        }

        [TearDown]
        public void TearDown()
        {
            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                LoggerManager.Logger.Error($"Test Failed: {TestContext.CurrentContext.Test.Name}. Capturing screenshot.");

                ITakesScreenshot screenshotDriver = (ITakesScreenshot)_driver;
                Screenshot screenshot = screenshotDriver.GetScreenshot();

                string screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory,
                                     $"{TestContext.CurrentContext.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

                screenshot.SaveAsFile(screenshotPath);

                TestContext.AddTestAttachment(screenshotPath, "Screenshot on Failure");
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            WebDriverManager.Instance.QuitDriver();
            LoggerManager.CloseLogger();
            LoggerManager.Logger.Information("NUnit OneTimeTearDown completed.");
        }

        [Test]
        [Category("Navigation")]
        public void Test_AboutEhuPageNavigation_Pass()
        {
            LoggerManager.Logger.Information("Starting Test_AboutEhuPageNavigation_Pass");
            _homePage.GoToHomePage();
            _homePage.ClickAboutLink();

            _driver.Url.ShouldContain("https://en.ehuniversity.lt/about/");
            _driver.Title.ShouldContain("About");

            var header = _driver.FindElement(By.XPath("//strong[contains(text(), 'European Humanities University (EHU)')]"));
            header.ShouldNotBeNull();

            LoggerManager.Logger.Information("Test_AboutEhuPageNavigation_Pass passed successfully.");
        }

        public static IEnumerable<TestCaseData> SearchTermsData()
        {
            yield return new TestCaseData("study programs", "/?s=study+programs", "study");
            yield return new TestCaseData("admission", "/?s=admission", "admission");
        }

        [Test]
        [Category("Search")]
        [TestCaseSource(nameof(SearchTermsData))]
        public void Test_SearchFunctionality_DataProvider(string searchTerm, string expectedUrlPart, string expectedResultWord)
        {
            LoggerManager.Logger.Information($"Starting Test_SearchFunctionality_DataProvider with term: {searchTerm}");

            _homePage.GoToHomePage();

            var searchComponent = _homePage.GetSearchComponent();
            searchComponent.PerformSearch(searchTerm);

            searchComponent.CheckResults(expectedUrlPart, expectedResultWord).ShouldBeTrue($"Search results for '{searchTerm}' were not relevant.");

            LoggerManager.Logger.Information("Test_SearchFunctionality_DataProvider passed successfully.");
        }

        [Test]
        [Category("Search")]
        public void Test_SearchFunctionality_Fail()
        {
            LoggerManager.Logger.Information("Starting Test_SearchFunctionality_Fail (Expected to Fail)");

            _homePage.GoToHomePage();

            var searchComponent = _homePage.GetSearchComponent();
            searchComponent.PerformSearch("nonexistent term");

            searchComponent.CheckResults("/?s=nonexistent+term", "nonexistent").ShouldBeTrue("This test should intentionally FAIL.");

            LoggerManager.Logger.Information("Test_SearchFunctionality_Fail completed.");
        }


        [Test]
        [Category("Navigation")]
        public void Test_LanguageChangeToLithuanian()
        {
            LoggerManager.Logger.Information("Starting Test_LanguageChangeToLithuanian");

            _homePage.GoToHomePage();
            _homePage.ChangeLanguageToLithuanian();

            _homePage.IsLithuanianContentPresent().ShouldBeTrue("Lithuanian content was not detected.");
            _driver.Url.ShouldContain("https://lt.ehuniversity.lt/");

            LoggerManager.Logger.Information("Test_LanguageChangeToLithuanian passed successfully.");
        }


        [Test]
        [Category("Contact")]
        public void Test_ContactInfoDisplayed_Skip()
        {
            Assert.Inconclusive("Skipping this test due to ongoing maintenance on Contact page.");
            LoggerManager.Logger.Warning("Test_ContactInfoDisplayed was skipped.");

            //LoggerManager.Logger.Information("Starting Test_ContactInfoDisplayed");

            var contactPage = new EhuContactPage(_driver);
            contactPage.GoToContactPage();
            var bodyText = contactPage.GetBodyText();

            bodyText.ShouldContain("franciskscarynacr@gmail.com");
            bodyText.ShouldContain("+370 68 771365");
            bodyText.ShouldContain("+375 29 5781488");
            bodyText.ShouldContain("Facebook");
            bodyText.ShouldContain("Telegram");
            bodyText.ShouldContain("VK");

            LoggerManager.Logger.Information("Test_ContactInfoDisplayed passed successfully.");
        }
    }
}