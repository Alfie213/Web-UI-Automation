using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Interactions;

namespace Web_UI_Automation.NUnit
{
    // d. Category
    [TestFixture]
    [Category("EHU_Website")]
    public class Class1
    {
        private IWebDriver _driver;

        // b. Test setup
        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            //options.AddArgument("--headless=new");
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        // b. Test tear down
        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        // d. Category
        [Test]
        [Category("Navigation")]
        public void Test_AboutEhuPageNavigation()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");
            var aboutLink = _driver.FindElements(By.XPath("//a[contains(text(), 'About')]")).FirstOrDefault();
            Assert.That(aboutLink, Is.Not.Null);
            aboutLink.Click();

            Assert.That(_driver.Url, Does.Contain("https://en.ehuniversity.lt/about/"));
            Assert.That(_driver.Title, Does.Contain("About"));

            var header = _driver.FindElement(By.XPath("//strong[contains(text(), 'European Humanities University (EHU)')]"));
            Assert.That(header, Is.Not.Null);
        }

        // c. Data provider (TestCaseSource)
        public static IEnumerable<TestCaseData> SearchTermsData()
        {
            yield return new TestCaseData("study programs", "/?s=study+programs", "study");
            yield return new TestCaseData("admission", "/?s=admission", "admission");
        }

        // d. Category
        [Test]
        [Category("Search")]
        [TestCaseSource(nameof(SearchTermsData))]
        public void Test_SearchFunctionality(string searchTerm, string expectedUrlPart, string expectedResultWord)
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            var searchContainer = wait.Until(d => d.FindElement(By.CssSelector(".header-search")));
            var actions = new Actions(_driver);
            actions.MoveToElement(searchContainer).Perform();

            var searchInput = wait.Until(d => d.FindElement(By.CssSelector("input[name='s']")));
            Assert.That(searchInput, Is.Not.Null);

            searchInput.SendKeys(searchTerm);

            var searchButton = _driver.FindElement(By.CssSelector(".header-search__form button[type='submit']"));
            Assert.That(searchButton, Is.Not.Null);

            searchButton.Click();

            wait.Until(d => d.Url.Contains(expectedUrlPart));
            Assert.That(_driver.Url, Does.Contain(expectedUrlPart));

            var results = _driver.FindElements(By.CssSelector("article a"));
            Assert.That(results.Any(r => r.Text.ToLower().Contains(expectedResultWord)), Is.True);
        }


        // d. Category
        [Test]
        [Category("Navigation")]
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
            Assert.That(_driver.Url, Does.Contain("https://lt.ehuniversity.lt/"));

            var bodyText = _driver.FindElement(By.TagName("body")).Text;
            Assert.That(bodyText.Contains("Apie"), Is.True);
        }

        // d. Category
        [Test]
        [Category("Contact")]
        public void Test_ContactInfoDisplayed()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/contact/");
            var bodyText = _driver.FindElement(By.TagName("body")).Text;

            Assert.That(bodyText.Contains("franciskscarynacr@gmail.com"), Is.True);
            Assert.That(bodyText.Contains("+370 68 771365"), Is.True);
            Assert.That(bodyText.Contains("+375 29 5781488"), Is.True);
            Assert.That(bodyText.Contains("Facebook"), Is.True);
            Assert.That(bodyText.Contains("Telegram"), Is.True);
            Assert.That(bodyText.Contains("VK"), Is.True);
        }
    }
}