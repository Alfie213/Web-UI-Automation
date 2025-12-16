using OpenQA.Selenium;
using Web_UI_Automation.Pages;

namespace Web_UI_Automation.NUnit
{
    public class TestContext
    {
        public IWebDriver Driver { get; }
        public EhuHomePage HomePage { get; }
        public EhuContactPage ContactPage { get; }

        public string ExpectedUrlPart { get; set; }
        public string ExpectedResultWord { get; set; }

        public TestContext(IWebDriver driver)
        {
            Driver = driver;
            HomePage = new EhuHomePage(driver);
            ContactPage = new EhuContactPage(driver);
        }
    }
}