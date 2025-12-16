using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using Reqnroll.Assist;
using Web_UI_Automation.Core;
using Web_UI_Automation.Pages;
using Shouldly;
using System.Linq;

namespace Web_UI_Automation.NUnit
{
    [Binding]
    public class EhuWebsiteSteps
    {
        private readonly TestContext _context;
        private readonly IWebDriver _driver;
        private readonly SearchComponent _searchComponent;

        public EhuWebsiteSteps(IWebDriver driver, TestContext context)
        {
            _driver = driver;
            _context = context;
            _searchComponent = _context.HomePage.GetSearchComponent();
        }

        [Given(@"I navigate to the EHU home page")]
        public void GivenINavigateToTheEhuHomePage()
        {
            LoggerManager.Logger.Information("Step: Navigate to EHU home page.");
            _context.HomePage.GoToHomePage();
        }

        [Given(@"I navigate to the EHU contact page")]
        public void GivenINavigateToTheEhuContactPage()
        {
            LoggerManager.Logger.Information("Step: Navigate to EHU contact page.");
            _context.ContactPage.GoToContactPage();
        }

        [When(@"I click the 'About' link")]
        public void WhenIClickTheAboutLink()
        {
            LoggerManager.Logger.Information("Step: Click 'About' link.");
            _context.HomePage.ClickAboutLink();
        }

        [Then(@"the page URL should contain '(.*)'")]
        public void ThenThePageUrlShouldContain(string expectedUrl)
        {
            LoggerManager.Logger.Debug($"Step: Assert URL contains: {expectedUrl}");
            _driver.Url.ShouldContain(expectedUrl);
        }

        [Then(@"the page title should contain '(.*)'")]
        public void ThenThePageTitleShouldContain(string expectedTitle)
        {
            LoggerManager.Logger.Debug($"Step: Assert Title contains: {expectedTitle}");
            _driver.Title.ShouldContain(expectedTitle);
        }

        [Then(@"I should see the header '(.*)'")]
        public void ThenIShouldSeeTheHeader(string expectedHeader)
        {
            LoggerManager.Logger.Debug($"Step: Assert Header contains: {expectedHeader}");
            var header = _driver.FindElement(By.XPath($"//strong[contains(text(), '{expectedHeader}')]"));
            header.ShouldNotBeNull();
        }

        [When(@"I search for a study program with the following data")]
        public void WhenISearchForAStudyProgramWithTheFollowingData(Table table)
        {
            var searches = table.CreateSet<(string searchTerm, string expectedUrlPart, string expectedResultWord)>();

            foreach (var search in searches)
            {
                LoggerManager.Logger.Information($"Step: Searching for: {search.searchTerm}");

                _context.HomePage.GoToHomePage();
                _searchComponent.PerformSearch(search.searchTerm);

                _context.ExpectedUrlPart = search.expectedUrlPart;
                _context.ExpectedResultWord = search.expectedResultWord;

                if (!_searchComponent.CheckResults(search.expectedUrlPart, search.expectedResultWord))
                {
                    throw new AssertionException($"Search failed for term: {search.searchTerm}");
                }
            }
        }

        [Then(@"the search results page should contain the expected URL part")]
        public void ThenTheSearchResultsPageShouldContainTheExpectedUrlPart()
        {
            _driver.Url.ShouldContain(_context.ExpectedUrlPart);
        }

        [Then(@"at least one result should contain the expected keyword")]
        public void ThenAtLeastOneResultShouldContainTheExpectedKeyword()
        {
            LoggerManager.Logger.Information("Data Driven Search checks completed successfully.");
        }


        [When(@"I change the language to Lithuanian")]
        public void WhenIChangeTheLanguageToLithuanian()
        {
            LoggerManager.Logger.Information("Step: Changing language to Lithuanian.");
            _context.HomePage.ChangeLanguageToLithuanian();
        }

        [Then(@"the URL should contain 'https://lt.ehuniversity.lt/'")]
        public void ThenTheUrlShouldContain()
        {
            _driver.Url.ShouldContain("https://lt.ehuniversity.lt/");
        }

        [Then(@"and the page body should contain the word '(.*)'")]
        public void ThenAndThePageBodyShouldContainTheWord(string expectedWord)
        {
            LoggerManager.Logger.Debug($"Step: Assert body contains: {expectedWord}");
            _context.HomePage.IsLithuanianContentPresent().ShouldBeTrue($"Page body did not contain '{expectedWord}'.");
        }

        [When(@"I check the contact details")]
        public void WhenICheckTheContactDetails()
        {
        }

        [Then(@"I should see the email '(.*)'")]
        public void ThenIShouldSeeTheEmail(string email)
        {
            _context.ContactPage.GetBodyText().ShouldContain(email);
        }

        [Then(@"I should see the phone number '(.*)'")]
        public void ThenIShouldSeeThePhoneNumber(string phone)
        {
            _context.ContactPage.GetBodyText().ShouldContain(phone);
        }

        [Then(@"I should see the social network link '(.*)'")]
        public void ThenIShouldSeeTheSocialNetworkLink(string network)
        {
            _context.ContactPage.GetBodyText().ShouldContain(network);
        }

        [Then(@"and the test should be inconclusive because of maintenance")]
        public void ThenAndTheTestShouldBeInconclusiveBecauseOfMaintenance()
        {
            LoggerManager.Logger.Warning("Step: Skipping test due to maintenance flag.");
            Assert.Inconclusive("Skipped by BDD Scenario due to maintenance flag.");
        }
    }
}