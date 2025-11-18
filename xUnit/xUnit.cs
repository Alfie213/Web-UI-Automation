using Xunit;
using OpenQA.Selenium;
using System.Collections.Generic;
using Web_UI_Automation.Pages;
using Web_UI_Automation.Core;
using Shouldly;

namespace Web_UI_Automation.XUnit
{
    [Trait("Category", "EHU_Website")]
    public class XUnitTests : IClassFixture<WebDriverFixture>
    {
        private readonly IWebDriver _driver;
        private readonly EhuHomePage _homePage;

        public XUnitTests(WebDriverFixture fixture)
        {
            _driver = fixture.Driver;
            _homePage = new EhuHomePage(_driver);
        }

        [Fact]
        [Trait("Category", "Navigation")]
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

        public static IEnumerable<object[]> SearchTermsData =>
            new List<object[]>
            {
                new object[] { "study programs", "/?s=study+programs", "study" },
                new object[] { "admission", "/?s=admission", "admission" }
            };

        [Theory]
        [MemberData(nameof(SearchTermsData))]
        [Trait("Category", "Search")]
        public void Test_SearchFunctionality(string searchTerm, string expectedUrlPart, string expectedResultWord)
        {
            LoggerManager.Logger.Information($"Starting Test_SearchFunctionality with term: {searchTerm}");

            _homePage.GoToHomePage();

            var searchComponent = _homePage.GetSearchComponent();
            searchComponent.PerformSearch(searchTerm);

            searchComponent.CheckResults(expectedUrlPart, expectedResultWord).ShouldBeTrue($"Search results for '{searchTerm}' were not relevant.");

            LoggerManager.Logger.Information("Test_SearchFunctionality passed successfully.");
        }


        [Fact]
        [Trait("Category", "Navigation")]
        public void Test_LanguageChangeToLithuanian()
        {
            LoggerManager.Logger.Information("Starting Test_LanguageChangeToLithuanian");

            _homePage.GoToHomePage();
            _homePage.ChangeLanguageToLithuanian();

            _homePage.IsLithuanianContentPresent().ShouldBeTrue("Lithuanian content was not detected.");
            (_driver.Url.Contains("https://lt.ehuniversity.lt/")).ShouldBeTrue("URL did not change to Lithuanian version.");

            LoggerManager.Logger.Information("Test_LanguageChangeToLithuanian passed successfully.");
        }

        [Fact]
        [Trait("Category", "Contact")]
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