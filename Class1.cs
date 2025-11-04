using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Web_UI_Automation
{
    [TestFixture]
    public class Class1
    {
        private IWebDriver _driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            //options.AddArgument("--headless=new");
            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
        }

        [Test]
        public void Test_AboutEhuPageNavigation()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");
            var aboutLink = _driver.FindElements(By.XPath("//a[contains(text(), 'About')]")).FirstOrDefault();
            Assert.That(aboutLink, Is.Not.Null, "About EHU link not found.");
            aboutLink.Click();

            Assert.That(_driver.Url, Does.Contain("https://en.ehuniversity.lt/about/"));
            Assert.That(_driver.Title, Does.Contain("About"));

            var header = _driver.FindElement(By.XPath("//strong[contains(text(), 'European Humanities University (EHU)')]"));
            Assert.That(header, Is.Not.Null, "Header text not found on About EHU page.");
        }

        [Test]
        public void Test_SearchFunctionality()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            var searchContainer = wait.Until(d => d.FindElement(By.CssSelector(".header-search")));
            var actions = new OpenQA.Selenium.Interactions.Actions(_driver);
            actions.MoveToElement(searchContainer).Perform();

            var searchInput = wait.Until(d => d.FindElement(By.CssSelector("input[name='s']")));
            Assert.That(searchInput, Is.Not.Null, "Search input not found.");

            searchInput.SendKeys("study programs");

            var searchButton = _driver.FindElement(By.CssSelector(".header-search__form button[type='submit']"));
            Assert.That(searchButton, Is.Not.Null, "Search button not found.");

            searchButton.Click();

            wait.Until(d => d.Url.Contains("/?s=study+programs"));
            Assert.That(_driver.Url, Does.Contain("/?s=study+programs"));

            var results = _driver.FindElements(By.CssSelector("article a"));
            Assert.That(results.Any(r => r.Text.ToLower().Contains("study")), Is.True, "No relevant search results found.");
        }


        [Test]
        public void Test_LanguageChangeToLithuanian()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            var langSwitcherMain = wait.Until(d => d.FindElement(By.CssSelector(".language-switcher > li > a")));
            var actions = new OpenQA.Selenium.Interactions.Actions(_driver);
            actions.MoveToElement(langSwitcherMain).Perform();

            var lithuanianLink = wait.Until(d => d.FindElement(By.CssSelector(".language-switcher li ul li a[href='https://lt.ehuniversity.lt/']")));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", lithuanianLink);

            wait.Until(d => d.Url.Contains("lt.ehuniversity.lt"));
            Assert.That(_driver.Url, Does.Contain("https://lt.ehuniversity.lt/"));

            var bodyText = _driver.FindElement(By.TagName("body")).Text;
            Assert.That(bodyText.Contains("Apie"), Is.True, "Lithuanian content not detected.");
        }


        [Test]
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