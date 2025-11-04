using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Interactions;

namespace Web_UI_Automation.XUnit
{
    // b. ?????????? IClassFixture ??? Setup/TearDown
    // d. Category (Trait) ?? ?????? ??????
    [Trait("Category", "EHU_Website")]
    public class xUnit : IClassFixture<WebDriverFixture>
    {
        private readonly IWebDriver _driver;

        public xUnit(WebDriverFixture fixture)
        {
            _driver = fixture.Driver;
        }

        // d. Category (Trait)
        [Fact]
        [Trait("Category", "Navigation")]
        public void Test_AboutEhuPageNavigation()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");
            var aboutLink = _driver.FindElements(By.XPath("//a[contains(text(), 'About')]")).FirstOrDefault();
            Assert.NotNull(aboutLink);
            aboutLink.Click();

            Assert.Contains("https://en.ehuniversity.lt/about/", _driver.Url);
            Assert.Contains("About", _driver.Title);

            var header = _driver.FindElement(By.XPath("//strong[contains(text(), 'European Humanities University (EHU)')]"));
            Assert.NotNull(header);
        }

        // c. Data provider (MemberData)
        public static IEnumerable<object[]> SearchTermsData =>
            new List<object[]>
            {
                new object[] { "study programs", "/?s=study+programs", "study" },
                new object[] { "admission", "/?s=admission", "admission" }
            };

        // d. Category (Trait)
        [Theory] // c. ???????????? ??? ????????????????? ??????
        [MemberData(nameof(SearchTermsData))] // c. Data Provider
        [Trait("Category", "Search")]
        public void Test_SearchFunctionality(string searchTerm, string expectedUrlPart, string expectedResultWord)
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            var searchContainer = wait.Until(d => d.FindElement(By.CssSelector(".header-search")));
            var actions = new Actions(_driver);
            actions.MoveToElement(searchContainer).Perform();

            var searchInput = wait.Until(d => d.FindElement(By.CssSelector("input[name='s']")));
            Assert.NotNull(searchInput);

            searchInput.SendKeys(searchTerm);

            var searchButton = _driver.FindElement(By.CssSelector(".header-search__form button[type='submit']"));
            Assert.NotNull(searchButton);

            searchButton.Click();

            wait.Until(d => d.Url.Contains(expectedUrlPart));
            Assert.Contains(expectedUrlPart, _driver.Url);

            var results = _driver.FindElements(By.CssSelector("article a"));
            Assert.True(results.Any(r => r.Text.ToLower().Contains(expectedResultWord)));
        }


        // d. Category (Trait)
        [Fact]
        [Trait("Category", "Navigation")]
        public void Test_LanguageChangeToLithuanian()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            var langSwitcherMain = wait.Until(d => d.FindElement(By.CssSelector(".language-switcher > li > a")));
            var actions = new Actions(_driver);
            actions.MoveToElement(langSwitcherMain).Perform();

            var lithuanianLink = wait.Until(d => d.FindElement(By.CssSelector(".language-switcher li ul li a[href='https://lt.ehuniversity.lt/']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", lithuanianLink);

            wait.Until(d => d.Url.Contains("lt.ehuniversity.lt"));
            Assert.Contains("https://lt.ehuniversity.lt/", _driver.Url);

            var bodyText = _driver.FindElement(By.TagName("body")).Text;
            Assert.Contains("Apie", bodyText);
        }

        // d. Category (Trait)
        [Fact]
        [Trait("Category", "Contact")]
        public void Test_ContactInfoDisplayed()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/contact/");
            var bodyText = _driver.FindElement(By.TagName("body")).Text;

            Assert.Contains("franciskscarynacr@gmail.com", bodyText);
            Assert.Contains("+370 68 771365", bodyText);
            Assert.Contains("+375 29 5781488", bodyText);
            Assert.Contains("Facebook", bodyText);
            Assert.Contains("Telegram", bodyText);
            Assert.Contains("VK", bodyText);
        }
    }
}