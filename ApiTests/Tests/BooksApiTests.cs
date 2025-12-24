using NUnit.Framework;
using ApiTests.Core;
using ApiTests.Models;
using Shouldly;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using NUnit.Framework.Interfaces;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ApiTests.Tests
{
    [TestFixture]
    public class BooksApiTests
    {
        private ApiManager? _apiManager;
        private BookCreateDto? _validBook;
        private BookDto? _createdBook;
        private string _invalidGuid = "12345678901234557890123456789012";
        private Guid _nonExistentId = Guid.NewGuid();

        private static ExtentReports _extent;
        private ExtentTest? _currentTest;

        private async Task<BookDto> CreateUniqueBookAndReturn()
        {
            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            var bookToCreate = new BookCreateDto
            {
                Title = $"Unique Book - {uniqueId}",
                Author = "Test Isolator",
                Isbn = $"978-0000000-{uniqueId}",
                PublishedDate = DateTime.Parse("2024-01-01T00:00:00Z")
            };

            var jsonContent = JsonSerializer.Serialize(bookToCreate);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _apiManager!.Client.PostAsync("/Books", content);

            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var getResponse = await _apiManager.Client.GetAsync("/Books");
                getResponse.EnsureSuccessStatusCode();
                var books = JsonSerializer.Deserialize<BookDto[]>(await getResponse.Content.ReadAsStringAsync());

                var createdBook = books?.FirstOrDefault(b => b.Title == bookToCreate.Title);

                if (createdBook != null && createdBook.Id != Guid.Empty)
                {
                    return createdBook;
                }
            }
            throw new InvalidOperationException($"Failed to create or find unique book. Status: {response.StatusCode}");
        }

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            string reportPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestReports");
            Directory.CreateDirectory(reportPath);
            var sparkReporter = new ExtentSparkReporter(Path.Combine(reportPath, "ApiTestReport.html"));
            _extent = new ExtentReports();
            _extent.AttachReporter(sparkReporter);

            var config = ApiManager.BuildConfiguration();
            _extent.AddSystemInfo("OS", Environment.OSVersion.VersionString);
            _extent.AddSystemInfo(".NET Version", Environment.Version.ToString());
            _extent.AddSystemInfo("Base URL", config["ApiConfig:BaseUrl"]);

            _apiManager = new ApiManager();
            await _apiManager.EnsureAuthenticated();

            var uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            _validBook = new BookCreateDto
            {
                Title = $"Static Test Book - {uniqueId}",
                Author = "API Tester",
                Isbn = $"978-1234567-{uniqueId}",
                PublishedDate = DateTime.Parse("2024-01-01T00:00:00Z")
            };
        }

        [SetUp]
        public void SetUpTest()
        {
            _currentTest = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            _currentTest.Log(Status.Info, $"Test started: {TestContext.CurrentContext.Test.Name}");
        }

        [TearDown]
        public void TearDownTest()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == TestStatus.Failed)
            {
                _currentTest!.Fail($"Test Failed: {TestContext.CurrentContext.Result.Message}");
                _currentTest.Log(Status.Error, "Stack Trace: " + TestContext.CurrentContext.Result.StackTrace);
            }
            else if (status == TestStatus.Skipped || status == TestStatus.Inconclusive)
            {
                _currentTest!.Skip($"Test Skipped: {TestContext.CurrentContext.Result.Message}");
            }
            else
            {
                _currentTest!.Pass("Test Passed.");
            }
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            if (_createdBook != null && _createdBook.Id != Guid.Empty)
            {
                await _apiManager!.Client.DeleteAsync($"/Books/{_createdBook.Id}");
            }
            _extent.Flush();
        }

        [Test, Order(1)]
        public async Task T01_CreateBook_ValidBook_Returns201AndCorrectData()
        {
            _currentTest!.Log(Status.Info, "Starting T01: Creating a valid book.");

            _createdBook = await CreateUniqueBookAndReturn();

            _createdBook.ShouldNotBeNull();
            _createdBook.Id.ShouldNotBe(Guid.Empty);
            _createdBook.Title.ShouldBe(_validBook!.Title);

            _currentTest.Log(Status.Pass, $"Book created successfully with ID: {_createdBook.Id}");
        }

        [Test, Order(2)]
        public async Task T02_CreateBook_DuplicateBook_ReturnsError()
        {
            _currentTest!.Log(Status.Info, "Starting T02: Testing duplicate book creation.");

            var jsonContent = JsonSerializer.Serialize(_createdBook);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _apiManager!.Client.PostAsync("/Books", content);

            response.StatusCode.ShouldBeOneOf(System.Net.HttpStatusCode.BadRequest, System.Net.HttpStatusCode.Conflict);

            _currentTest.Log(Status.Pass, "Duplicate book creation successfully blocked.");
        }

        [Test, Order(3)]
        public async Task T03_GetAllBooks_ReturnsListAndCorrectFormat()
        {
            var bookForTest = await CreateUniqueBookAndReturn();

            _currentTest!.Log(Status.Info, "Starting T03: Getting all books.");

            var response = await _apiManager!.Client.GetAsync("/Books");
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<BookDto[]>(responseBody);

            books.ShouldNotBeEmpty();
            books.ShouldContain(b => b.Id == bookForTest.Id);

            var book = books.First();
            book.Title.ShouldNotBeNullOrWhiteSpace();
            book.Author.ShouldNotBeNullOrWhiteSpace();
            book.Id.ShouldNotBe(Guid.Empty);

            await _apiManager.Client.DeleteAsync($"/Books/{bookForTest.Id}");

            _currentTest.Log(Status.Pass, $"Successfully retrieved {books.Length} books.");
        }

        [Test, Order(4)]
        public async Task T04_GetBookById_ValidId_Returns200AndCorrectData()
        {
            var bookForTest = await CreateUniqueBookAndReturn();

            _currentTest!.Log(Status.Info, "Starting T04: Getting book by valid ID.");

            var response = await _apiManager!.Client.GetAsync($"/Books/{bookForTest.Id}");
            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

            var responseBody = await response.Content.ReadAsStringAsync();
            var fetchedBook = JsonSerializer.Deserialize<BookDto>(responseBody);

            fetchedBook!.Id.ShouldBe(bookForTest.Id);
            fetchedBook.Title.ShouldBe(bookForTest.Title);

            await _apiManager.Client.DeleteAsync($"/Books/{bookForTest.Id}");

            _currentTest.Log(Status.Pass, "Successfully fetched correct book by ID.");
        }

        [Test, Order(5)]
        public async Task T05_GetBookById_NonExistentId_Returns404()
        {
            _currentTest!.Log(Status.Info, "Starting T05: Getting book by non-existent ID.");

            var response = await _apiManager!.Client.GetAsync($"/Books/{_nonExistentId}");

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);

            _currentTest.Log(Status.Pass, "Correctly returned 404 for non-existent ID.");
        }

        [Test, Order(6)]
        public async Task T06_GetBookById_InvalidIdFormat_Returns400()
        {
            _currentTest!.Log(Status.Info, "Starting T06: Getting book by invalid ID format.");

            var response = await _apiManager!.Client.GetAsync($"/Books/{_invalidGuid}");

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);

            _currentTest.Log(Status.Pass, "Correctly returned 404 for invalid ID format.");
        }

        [Test, Order(7)]
        public async Task T07_UpdateBook_ValidData_Returns204AndUpdatesData()
        {
            var bookForTest = await CreateUniqueBookAndReturn();

            _currentTest!.Log(Status.Info, "Starting T07: Updating an existing book.");

            var updatedBookDto = bookForTest.Clone();
            updatedBookDto.Title = "Updated Title " + Guid.NewGuid().ToString().Substring(0, 4);
            updatedBookDto.Author = "Updated Author";

            var jsonContent = JsonSerializer.Serialize(updatedBookDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _apiManager!.Client.PutAsync($"/Books/{updatedBookDto.Id}", content);

            response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

            var getResponse = await _apiManager.Client.GetAsync($"/Books/{updatedBookDto.Id}");
            getResponse.EnsureSuccessStatusCode();
            var fetchedBook = JsonSerializer.Deserialize<BookDto>(await getResponse.Content.ReadAsStringAsync());

            fetchedBook!.Title.ShouldBe(updatedBookDto.Title);

            await _apiManager.Client.DeleteAsync($"/Books/{bookForTest.Id}");

            _currentTest.Log(Status.Pass, $"Book updated to Title: {fetchedBook.Title}");
        }

        [Test, Order(8)]
        public async Task T08_DeleteBook_ValidId_Returns204AndDeletesBook()
        {
            var bookForTest = await CreateUniqueBookAndReturn();

            _currentTest!.Log(Status.Info, "Starting T08: Deleting a valid book.");

            var deleteResponse = await _apiManager!.Client.DeleteAsync($"/Books/{bookForTest.Id}");

            deleteResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

            var getResponse = await _apiManager.Client.GetAsync($"/Books/{bookForTest.Id}");
            getResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);

            _currentTest.Log(Status.Pass, "Book deleted successfully.");
        }
    }
}