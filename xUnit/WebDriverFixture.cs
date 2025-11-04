using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

namespace Web_UI_Automation.XUnit
{
    // b. Setup/TearDown (класс для настройки/очистки WebDriver)
    public class WebDriverFixture : IDisposable
    {
        public IWebDriver Driver { get; private set; }

        public WebDriverFixture()
        {
            var options = new ChromeOptions();
            //options.AddArgument("--headless=new");
            Driver = new ChromeDriver(options);
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        // b. TearDown (вызывается xUnit после всех тестов, использующих этот Fixture)
        public void Dispose()
        {
            Driver.Quit();
        }
    }
}