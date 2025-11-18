using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Web_UI_Automation.Core
{
    public sealed class WebDriverManager
    {
        private static WebDriverManager _instance = null;
        private static readonly object _lock = new object();

        private IWebDriver _driver;

        private WebDriverManager()
        {
            LoggerManager.Logger.Information("WebDriverManager (Singleton) initialized.");
        }

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

        public IWebDriver GetDriver()
        {
            if (_driver == null)
            {
                var options = new ChromeOptions();
                //options.AddArgument("--headless=new");
                _driver = new ChromeDriver(options);
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                LoggerManager.Logger.Information("New ChromeDriver instance created.");
            }
            return _driver;
        }

        public void QuitDriver()
        {
            if (_driver != null)
            {
                _driver.Quit();
                _driver = null;
                LoggerManager.Logger.Warning("ChromeDriver instance closed.");
            }
        }
    }
}