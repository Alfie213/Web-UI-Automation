using Xunit;
using OpenQA.Selenium;
using System.Collections.Generic;
using Web_UI_Automation.Pages;
using Web_UI_Automation.Core;

namespace Web_UI_Automation.XUnit
{
    // d. Category (Trait)
    [Trait("Category", "EHU_Website")]
    public class XUnitTests : IClassFixture<WebDriverFixture> // b. IClassFixture
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
            _homePage.GoToHomePage();
            _homePage.ClickAboutLink();

            Assert.Contains("https://en.ehuniversity.lt/about/", _driver.Url);
            Assert.Contains("About", _driver.Title);

            var header = _driver.FindElement(By.XPath("//strong[contains(text(), 'European Humanities University (EHU)')]"));
            Assert.NotNull(header);
        }

        public static IEnumerable<object[]> SearchTermsData =>
            new List<object[]>
            {
                new object[] { "study programs", "/?s=study+programs", "study" },
                new object[] { "admission", "/?s=admission", "admission" }
            };

        [Theory] // c. Parametrized Test
        [MemberData(nameof(SearchTermsData))] // c. Data Provider
        [Trait("Category", "Search")]
        public void Test_SearchFunctionality(string searchTerm, string expectedUrlPart, string expectedResultWord)
        {
            _homePage.GoToHomePage();

            var searchComponent = _homePage.GetSearchComponent();
            searchComponent.PerformSearch(searchTerm);

            Assert.True(searchComponent.CheckResults(expectedUrlPart, expectedResultWord));
        }


        [Fact]
        [Trait("Category", "Navigation")]
        public void Test_LanguageChangeToLithuanian()
        {
            _homePage.GoToHomePage();
            _homePage.ChangeLanguageToLithuanian();

            Assert.True(_homePage.IsLithuanianContentPresent());
            Assert.Contains("https://lt.ehuniversity.lt/", _driver.Url);
        }

        [Fact]
        [Trait("Category", "Contact")]
        public void Test_ContactInfoDisplayed()
        {
            var contactPage = new EhuContactPage(_driver);
            contactPage.GoToContactPage();
            var bodyText = contactPage.GetBodyText();

            Assert.Contains("franciskscarynacr@gmail.com", bodyText);
            Assert.Contains("+370 68 771365", bodyText);
            Assert.Contains("+375 29 5781488", bodyText);
            Assert.Contains("Facebook", bodyText);
            Assert.Contains("Telegram", bodyText);
            Assert.Contains("VK", bodyText);
        }
    }
}