using OpenQA.Selenium;

namespace Web_UI_Automation.Pages
{
    // Page Object
    public class EhuContactPage
    {
        private readonly IWebDriver _driver;

        public EhuContactPage(IWebDriver driver)
        {
            _driver = driver;
        }

        public void GoToContactPage()
        {
            _driver.Navigate().GoToUrl("https://en.ehu.lt/contact/");
        }

        public string GetBodyText()
        {
            return _driver.FindElement(By.TagName("body")).Text;
        }
    }
}