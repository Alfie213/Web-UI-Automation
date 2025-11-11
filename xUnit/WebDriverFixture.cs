using Xunit;
using OpenQA.Selenium;
using System;
using Web_UI_Automation.Core;

namespace Web_UI_Automation.XUnit
{
    // b. Setup/TearDown
    public class WebDriverFixture : IDisposable
    {
        public IWebDriver Driver { get; private set; }

        public WebDriverFixture()
        {
            Driver = WebDriverManager.Instance.GetDriver();
        }

        // b. TearDown: IDisposable для IClassFixture
        public void Dispose()
        {
            // Закрываем IWebDriver через Singleton
            WebDriverManager.Instance.QuitDriver();
        }
    }
}