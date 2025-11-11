using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using OpenQA.Selenium.Interactions;

namespace Web_UI_Automation.Pages
{
    // Page Object
    public class EhuHomePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Локаторы
        private By AboutLinkLocator => By.XPath("//a[contains(text(), 'About')]");
        private By SearchContainerLocator => By.CssSelector(".header-search");
        private By LangSwitcherMainLocator => By.CssSelector(".language-switcher > li > a");
        private By LithuanianLinkLocator => By.CssSelector(".language-switcher li ul li a[href='https://lt.ehuniversity.lt/']");

        public EhuHomePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void GoToHomePage()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/");
        }

        public void ClickAboutLink()
        {
            var aboutLink = _driver.FindElements(AboutLinkLocator).FirstOrDefault();
            if (aboutLink == null)
                throw new NoSuchElementException("About EHU link not found.");
            aboutLink.Click();
        }

        public SearchComponent GetSearchComponent()
        {
            return new SearchComponentBuilder(_driver).Build();
        }

        public void ChangeLanguageToLithuanian()
        {
            var langSwitcherMain = _wait.Until(d => d.FindElement(LangSwitcherMainLocator));
            new Actions(_driver).MoveToElement(langSwitcherMain).Perform();

            var lithuanianLink = _wait.Until(d => d.FindElement(LithuanianLinkLocator));
            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", lithuanianLink);
        }

        public bool IsLithuanianContentPresent()
        {
            _wait.Until(d => d.Url.Contains("lt.ehuniversity.lt"));
            return _driver.FindElement(By.TagName("body")).Text.Contains("Apie");
        }
    }
}