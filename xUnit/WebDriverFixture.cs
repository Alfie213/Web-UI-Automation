using Xunit;
using OpenQA.Selenium;
using System;
using Web_UI_Automation.Core;

namespace Web_UI_Automation.XUnit
{
    public class WebDriverFixture : IDisposable
    {
        public IWebDriver Driver { get; private set; }

        public WebDriverFixture()
        {
            LoggerManager.Logger.Information("XUnit WebDriverFixture initialized.");
            Driver = WebDriverManager.Instance.GetDriver();
        }

        public void Dispose()
        {
            WebDriverManager.Instance.QuitDriver();
            LoggerManager.CloseLogger();
        }
    }
}