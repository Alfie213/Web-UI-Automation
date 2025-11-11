using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;
using Web_UI_Automation.Core;
using Web_UI_Automation.Pages;

namespace Web_UI_Automation.NUnit
{
    // a. Параллелизация: Отключена, так как используется Singleton WebDriver (OneTimeSetup)
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    [Category("EHU_Website")]
    public class NUnitTests
    {
        private IWebDriver _driver;
        private EhuHomePage _homePage;

        // b. OneTimeSetup (Singleton/Factory)
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            // Получаем IWebDriver через Singleton/Factory
            _driver = WebDriverManager.Instance.GetDriver();
            _homePage = new EhuHomePage(_driver);
        }

        // b. OneTimeTearDown (Singleton/Factory)
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            // Закрываем IWebDriver через Singleton
            WebDriverManager.Instance.QuitDriver();
        }

        [Test]
        [Category("Navigation")]
        public void Test_AboutEhuPageNavigation()
        {
            _homePage.GoToHomePage();
            _homePage.ClickAboutLink();

            // Проверки страницы About (можно вынести в EhuAboutPage PO)
            Assert.That(_driver.Url, Does.Contain("https://en.ehuniversity.lt/about/"));
            Assert.That(_driver.Title, Does.Contain("About"));

            var header = _driver.FindElement(By.XPath("//strong[contains(text(), 'European Humanities University (EHU)')]"));
            Assert.That(header, Is.Not.Null);
        }

        public static IEnumerable<TestCaseData> SearchTermsData()
        {
            yield return new TestCaseData("study programs", "/?s=study+programs", "study");
            yield return new TestCaseData("admission", "/?s=admission", "admission");
        }

        [Test]
        [Category("Search")]
        [TestCaseSource(nameof(SearchTermsData))]
        public void Test_SearchFunctionality(string searchTerm, string expectedUrlPart, string expectedResultWord)
        {
            _homePage.GoToHomePage();

            // Использование компонента, созданного Builder-ом
            var searchComponent = _homePage.GetSearchComponent();
            searchComponent.PerformSearch(searchTerm);

            Assert.That(searchComponent.CheckResults(expectedUrlPart, expectedResultWord), Is.True);
        }


        [Test]
        [Category("Navigation")]
        public void Test_LanguageChangeToLithuanian()
        {
            _homePage.GoToHomePage();
            _homePage.ChangeLanguageToLithuanian();

            Assert.That(_homePage.IsLithuanianContentPresent(), Is.True);
            Assert.That(_driver.Url, Does.Contain("https://lt.ehuniversity.lt/"));
        }


        [Test]
        [Category("Contact")]
        public void Test_ContactInfoDisplayed()
        {
            var contactPage = new EhuContactPage(_driver);
            contactPage.GoToContactPage();
            var bodyText = contactPage.GetBodyText();

            Assert.That(bodyText.Contains("franciskscarynacr@gmail.com"), Is.True);
            Assert.That(bodyText.Contains("+370 68 771365"), Is.True);
            Assert.That(bodyText.Contains("+375 29 5781488"), Is.True);
            Assert.That(bodyText.Contains("Facebook"), Is.True);
            Assert.That(bodyText.Contains("Telegram"), Is.True);
            Assert.That(bodyText.Contains("VK"), Is.True);
        }
    }
}