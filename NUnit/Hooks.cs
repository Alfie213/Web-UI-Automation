using Reqnroll.BoDi;
using OpenQA.Selenium;
using Reqnroll;
using Web_UI_Automation.Core;

namespace Web_UI_Automation.NUnit
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeScenario(Order = 1)]
        public void BeforeScenarioInitializeDriver()
        {
            IWebDriver driver = WebDriverManager.Instance.GetDriver();

            _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
            _objectContainer.RegisterTypeAs<TestContext, TestContext>();
        }

        /*
        [AfterScenario]
        public void AfterScenarioCleanup()
        {
             WebDriverManager.Instance.QuitDriver();
        }
        */
    }
}