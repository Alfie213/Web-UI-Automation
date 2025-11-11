using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Web_UI_Automation.Core
{
    // Singleton (Обязательно) и Factory (Опционально)
    public sealed class WebDriverManager
    {
        // Singleton: Приватный статический экземпляр
        private static WebDriverManager _instance = null;
        private static readonly object _lock = new object();

        // Factory: Экземпляр IWebDriver
        private IWebDriver _driver;

        // Singleton: Приватный конструктор
        private WebDriverManager() { }

        // Singleton: Метод доступа к экземпляру
        public static WebDriverManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new WebDriverManager();
                        }
                    }
                }
                return _instance;
            }
        }

        // Factory: Метод создания IWebDriver
        public IWebDriver GetDriver()
        {
            if (_driver == null)
            {
                var options = new ChromeOptions();
                //options.AddArgument("--headless=new");
                _driver = new ChromeDriver(options);
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            }
            return _driver;
        }

        public void QuitDriver()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
            }
        }
    }
}