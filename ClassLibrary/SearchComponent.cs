using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;
using OpenQA.Selenium.Interactions;
using System;

namespace Web_UI_Automation.Pages
{
    // Паттерн Builder: Компонент, созданный билдером
    public class SearchComponent
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        // Приватный конструктор (доступен только Билдеру)
        internal SearchComponent(IWebDriver driver, WebDriverWait wait)
        {
            _driver = driver;
            _wait = wait;
        }

        // Локаторы
        private By SearchContainerLocator => By.CssSelector(".header-search");
        private By SearchInputLocator => By.CssSelector("input[name='s']");
        private By SearchButtonLocator => By.CssSelector(".header-search__form button[type='submit']");
        private By SearchResultsLocator => By.CssSelector("article a");

        public void PerformSearch(string searchTerm)
        {
            // Наведение курсора (для открытия поиска)
            var searchContainer = _wait.Until(d => d.FindElement(SearchContainerLocator));
            new Actions(_driver).MoveToElement(searchContainer).Perform();

            var searchInput = _wait.Until(d => d.FindElement(SearchInputLocator));
            if (searchInput == null)
                throw new NoSuchElementException("Search input not found.");

            searchInput.SendKeys(searchTerm);

            var searchButton = _driver.FindElement(SearchButtonLocator);
            if (searchButton == null)
                throw new NoSuchElementException("Search button not found.");

            searchButton.Click();
        }

        public bool CheckResults(string expectedUrlPart, string expectedResultWord)
        {
            _wait.Until(d => d.Url.Contains(expectedUrlPart));

            var results = _driver.FindElements(SearchResultsLocator);
            return _driver.Url.Contains(expectedUrlPart) &&
                   results.Any(r => r.Text.ToLower().Contains(expectedResultWord));
        }
    }

    // Паттерн Builder: Класс, который строит компонент
    public class SearchComponentBuilder
    {
        private readonly IWebDriver _driver;

        public SearchComponentBuilder(IWebDriver driver)
        {
            _driver = driver;
        }

        public SearchComponent Build()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            return new SearchComponent(_driver, wait);
        }
    }
}