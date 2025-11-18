using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using Web_UI_Automation.Core;
using Web_UI_Automation.Pages;
using Shouldly;

namespace Web_UI_Automation.NUnit
{
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

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            WebDriverManager.Instance.QuitDriver();
            LoggerManager.CloseLogger();
            LoggerManager.Logger.Information("NUnit OneTimeTearDown completed.");
        }

        [Test]
        [Category("Navigation")]
        public void Test_AboutEhuPageNavigation()
        {
            LoggerManager.Logger.Information("Starting Test_AboutEhuPageNavigation");

            _homePage.GoToHomePage();
            _homePage.ClickAboutLink();

            _driver.Url.ShouldContain("https://en.ehuniversity.lt/about/");
            _driver.Title.ShouldContain("About");

            var header = _driver.FindElement(By.XPath("//strong[contains(text(), 'European Humanities University (EHU)')]"));
            header.ShouldNotBeNull();

            LoggerManager.Logger.Information("Test_AboutEhuPageNavigation passed successfully.");
        }

        public static IEnumerable<TestCaseData> SearchTermsData()
        {
            yield return new TestCaseData("study programs", "/?s=study+programs", "study");
            yield return new TestCaseData("admission", "/?s=admission", "admission");
        }

        [Test]
        [Category("Search")]
        [TestCaseSource(nameof(SearchTermsData))]
        public void Test_SearchFunctionality(string searchTerm, string expectedUrlPart, string expectedResultWord)
        {
            LoggerManager.Logger.Information($"Starting Test_SearchFunctionality with term: {searchTerm}");

            _homePage.GoToHomePage();

            var searchComponent = _homePage.GetSearchComponent();
            searchComponent.PerformSearch(searchTerm);

            searchComponent.CheckResults(expectedUrlPart, expectedResultWord).ShouldBeTrue($"Search results for '{searchTerm}' were not relevant.");

            LoggerManager.Logger.Information("Test_SearchFunctionality passed successfully.");
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
        public void Test_ContactInfoDisplayed()
        {
            LoggerManager.Logger.Information("Starting Test_ContactInfoDisplayed");

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