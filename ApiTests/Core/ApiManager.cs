using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;
using ApiTests.Models;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace ApiTests.Core
{
    public class ApiManager
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string? _accessToken;

        public static IConfiguration BuildConfiguration()
        {
            var basePath = TestContext.CurrentContext.TestDirectory;

            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            return new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public ApiManager()
        {
            _configuration = BuildConfiguration();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_configuration["ApiConfig:BaseUrl"]
                                              ?? throw new InvalidOperationException("ApiConfig:BaseUrl is missing in configuration."));
        }

        public async Task EnsureAuthenticated()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                await GetAuthToken();
            }
            if (!string.IsNullOrEmpty(_accessToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
            }
            else
            {
                throw new InvalidOperationException("Failed to retrieve access token.");
            }
        }

        private async Task GetAuthToken()
        {
            var authUrl = _configuration["ApiConfig:AuthUrl"];
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _configuration["AuthCredentials:ClientId"] ?? ""),
                new KeyValuePair<string, string>("client_secret", _configuration["AuthCredentials:ClientSecret"] ?? ""),
                new KeyValuePair<string, string>("scope", _configuration["AuthCredentials:Scope"] ?? ""),
                new KeyValuePair<string, string>("grant_type", _configuration["AuthCredentials:GrantType"] ?? "")
            });

            using var client = new HttpClient();
            var response = await client.PostAsync(authUrl, content);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var authToken = JsonSerializer.Deserialize<AuthToken>(responseBody);
            _accessToken = authToken?.access_token;
        }

        public HttpClient Client => _httpClient;
    }
}