@web @ehu @smoke
Feature: EHU Website Main Functionality Tests
  As a website visitor
  I want to use the main features of the EHU website
  So that I can find information, search, and change language.

Scenario: 1. Successful Navigation to About Page and Content Verification
  Given I navigate to the EHU home page
  When I click the 'About' link
  Then the page URL should contain 'https://en.ehuniversity.lt/about/'
  And the page title should contain 'About'
  And I should see the header 'European Humanities University (EHU)'

Scenario: 2. Search Functionality Verification (Data Driven)
  Given I navigate to the EHU home page
  When I search for a study program with the following data
    | searchTerm | expectedUrlPart | expectedResultWord |
    | study programs | /?s=study+programs | study |
    | admission | /?s=admission | admission |
  Then the search results page should contain the expected URL part
  And at least one result should contain the expected keyword

Scenario: 3. Language Change to Lithuanian Verification
  Given I navigate to the EHU home page
  When I change the language to Lithuanian
  Then the URL should contain 'https://lt.ehuniversity.lt/'
  And the page body should contain the word 'Apie'

@skip
Scenario: 4. Contact Information Display (Skipped Test)
  Given I navigate to the EHU contact page
  When I check the contact details
  Then I should see the email 'franciskscarynacr@gmail.com'
  And I should see the phone number '+370 68 771365'
  And I should see the social network link 'Facebook'
  And the test should be inconclusive because of maintenance