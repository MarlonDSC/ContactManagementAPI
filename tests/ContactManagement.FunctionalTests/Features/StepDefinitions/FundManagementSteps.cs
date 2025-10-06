using ContactManagement.Application.DTOs;
using ContactManagement.FunctionalTests.Features.Support;
using System.Net;
using System.Text.Json;

namespace ContactManagement.FunctionalTests.Features.StepDefinitions
{
    [Binding]
    public class FundManagementSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private TestContext TestContext => _scenarioContext.Get<TestContext>();

        public FundManagementSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"I provide a fund name")]
        public void GivenIProvideAFundName()
        {
            // Create a fund DTO with valid data
            var fundDto = new CreateFundDto("Test Fund");
            _scenarioContext.Set(fundDto, "FundDto");
            Console.WriteLine($"Created fund DTO with name: {fundDto.Name}");
        }

        [Given(@"I do not provide a fund name")]
        public void GivenIDoNotProvideAFundName()
        {
            // Create a fund DTO with empty name
            var fundDto = new CreateFundDto(string.Empty);
            _scenarioContext.Set(fundDto, "FundDto");
            Console.WriteLine("Created fund DTO with empty name");
        }

        [Given(@"I provide multiple fund names")]
        public void GivenIProvideMultipleFundNames()
        {
            // Create a list of fund names
            var fundNames = new List<string> { "Fund A", "Fund B", "Fund C" };
            _scenarioContext.Set(fundNames, "FundNames");
            Console.WriteLine($"Created list of {fundNames.Count} fund names");
        }

        [Given(@"I provide an empty list of fund names")]
        public void GivenIProvideAnEmptyListOfFundNames()
        {
            // Create an empty list of fund names
            var fundNames = new List<string>();
            _scenarioContext.Set(fundNames, "FundNames");
            Console.WriteLine("Created empty list of fund names");
        }

        [When(@"I create a fund")]
        public async Task WhenICreateAFund()
        {
            // Get the fund DTO from the scenario context
            var fundDto = _scenarioContext.Get<CreateFundDto>("FundDto");

            Console.WriteLine("Sending request to create fund");

            // Send the request to create a fund
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/funds",
                fundDto);

            _scenarioContext.Set(response, "Response");

            // If successful, store the created fund
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Fund creation successful with status code: {(int)response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var options = TestContext.JsonOptions;
                var createdFund = JsonSerializer.Deserialize<FundDto>(content, options);

                if (createdFund != null)
                {
                    _scenarioContext.Set(createdFund, "CreatedFund");
                    Console.WriteLine($"Created fund with ID: {createdFund.Id}");
                }
            }
            else
            {
                Console.WriteLine($"Fund creation failed with status code: {(int)response.StatusCode}");
            }
        }

        [When(@"I attempt to create a fund")]
        public async Task WhenIAttemptToCreateAFund()
        {
            // Get the fund DTO from the scenario context
            var fundDto = _scenarioContext.Get<CreateFundDto>("FundDto");

            Console.WriteLine("Sending request to create fund (expected to fail)");

            // Send the request to create a fund
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/funds",
                fundDto);

            _scenarioContext.Set(response, "Response");

            Console.WriteLine($"Received response with status code: {(int)response.StatusCode}");
        }

        [When(@"I create multiple funds")]
        public async Task WhenICreateMultipleFunds()
        {
            // Get the fund names from the scenario context
            var fundNames = _scenarioContext.Get<List<string>>("FundNames");

            Console.WriteLine($"Sending request to create {fundNames.Count} funds");

            // Send the request to create multiple funds
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/funds/batch",
                fundNames);

            _scenarioContext.Set(response, "Response");

            // If successful, store the created funds
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Multiple funds creation successful with status code: {(int)response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var options = TestContext.JsonOptions;
                var createdFunds = JsonSerializer.Deserialize<List<FundDto>>(content, options);

                if (createdFunds != null)
                {
                    _scenarioContext.Set(createdFunds, "CreatedFunds");
                    Console.WriteLine($"Created {createdFunds.Count} funds");
                }
            }
            else
            {
                Console.WriteLine($"Multiple funds creation failed with status code: {(int)response.StatusCode}");
            }
        }

        [When(@"I attempt to create multiple funds")]
        public async Task WhenIAttemptToCreateMultipleFunds()
        {
            // Get the fund names from the scenario context
            var fundNames = _scenarioContext.Get<List<string>>("FundNames");

            Console.WriteLine($"Sending request to create {fundNames.Count} funds (expected to fail)");

            // Send the request to create multiple funds
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/funds/batch",
                fundNames);

            _scenarioContext.Set(response, "Response");

            Console.WriteLine($"Received response with status code: {(int)response.StatusCode}");
        }

        [Then(@"the fund should be created successfully")]
        public void ThenTheFundShouldBeCreatedSuccessfully()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");

            // Assert that the response has a 201 Created status code
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Console.WriteLine("Verified response has 201 Created status code");

            // Assert that the Location header is present
            Assert.True(response.Headers.Location != null, "Location header should be present");
            Console.WriteLine($"Verified Location header is present: {response.Headers.Location}");

            // Get the created fund and original DTO from the scenario context
            var createdFund = _scenarioContext.Get<FundDto>("CreatedFund");
            var fundDto = _scenarioContext.Get<CreateFundDto>("FundDto");

            // Assert that the created fund is not null
            Assert.NotNull(createdFund);
            Console.WriteLine("Verified created fund is not null");

            // Assert that the created fund contains the expected data
            Assert.Equal(fundDto.Name, createdFund.Name);
            Console.WriteLine($"Verified name matches: {createdFund.Name}");

            // Verify ID and timestamps
            Assert.NotEqual(Guid.Empty, createdFund.Id);
            Console.WriteLine($"Verified ID is not empty: {createdFund.Id}");

            Assert.True(createdFund.CreatedAt > DateTime.MinValue);
            Console.WriteLine($"Verified CreatedAt is valid: {createdFund.CreatedAt}");
        }

        [Then(@"the funds should be created successfully")]
        public void ThenTheFundsShouldBeCreatedSuccessfully()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");

            // Assert that the response has a 201 Created status code
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Console.WriteLine("Verified response has 201 Created status code");

            // Get the created funds and original fund names from the scenario context
            var createdFunds = _scenarioContext.Get<List<FundDto>>("CreatedFunds");
            var fundNames = _scenarioContext.Get<List<string>>("FundNames");

            // Assert that the created funds collection is not null
            Assert.NotNull(createdFunds);
            Console.WriteLine("Verified created funds collection is not null");

            // Assert that the number of created funds matches the number of fund names
            Assert.Equal(fundNames.Count, createdFunds.Count);
            Console.WriteLine($"Verified number of created funds ({createdFunds.Count}) matches number of fund names ({fundNames.Count})");

            // Assert that each created fund has a valid ID and timestamp
            foreach (var fund in createdFunds)
            {
                Assert.NotEqual(Guid.Empty, fund.Id);
                Assert.True(fund.CreatedAt > DateTime.MinValue);
                Console.WriteLine($"Verified fund '{fund.Name}' has valid ID and timestamp");
            }

            // Assert that the created funds contain the expected names
            foreach (var name in fundNames)
            {
                Assert.Contains(createdFunds, f => f.Name == name);
                Console.WriteLine($"Verified fund with name '{name}' was created");
            }
        }

        // Using the existing step definition from ContactManagementSteps.cs

        [Given(@"I request all funds")]
        public async Task GivenIRequestAllFunds()
        {
            // First seed some funds to ensure we have data to retrieve
            await SeedTestFunds();

            Console.WriteLine("Sending request to get all funds");

            // Send the request to get all funds
            var response = await TestContext.Client.GetAsync("api/funds");

            _scenarioContext.Set(response, "Response");

            // If successful, store the retrieved funds
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Get all funds successful with status code: {(int)response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var options = TestContext.JsonOptions;
                var funds = JsonSerializer.Deserialize<IEnumerable<FundDto>>(content, options);

                if (funds != null)
                {
                    _scenarioContext.Set(funds, "RetrievedFunds");
                    Console.WriteLine($"Retrieved {funds.Count()} funds");
                }
            }
            else
            {
                Console.WriteLine($"Get all funds failed with status code: {(int)response.StatusCode}");
            }
        }

        private async Task SeedTestFunds()
        {
            // Create a list of test funds
            var testFunds = new List<string> { "Test Fund 1", "Test Fund 2", "Test Fund 3" };
            _scenarioContext.Set(testFunds, "TestFundNames");

            Console.WriteLine($"Seeding {testFunds.Count} test funds");

            // Send the request to create the test funds
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/funds/batch",
                testFunds);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Test funds seeded successfully");
                
                // Store the created funds for later verification
                var content = await response.Content.ReadAsStringAsync();
                var options = TestContext.JsonOptions;
                var createdFunds = JsonSerializer.Deserialize<List<FundDto>>(content, options);
                
                if (createdFunds != null)
                {
                    _scenarioContext.Set(createdFunds, "SeededFunds");
                }
            }
            else
            {
                Console.WriteLine($"Failed to seed test funds: {(int)response.StatusCode}");
            }
        }
        
        [Then(@"the system should return the list of funds")]
        public void ThenTheSystemShouldReturnTheListOfFunds()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            
            // Assert that the response has a 200 OK status code
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine("Verified response has 200 OK status code");
            
            // Get the retrieved funds and test fund names
            var retrievedFunds = _scenarioContext.Get<IEnumerable<FundDto>>("RetrievedFunds");
            var testFundNames = _scenarioContext.Get<List<string>>("TestFundNames");
            var seededFunds = _scenarioContext.Get<List<FundDto>>("SeededFunds");
            
            // Assert that the retrieved funds collection is not null
            Assert.NotNull(retrievedFunds);
            Console.WriteLine("Verified retrieved funds collection is not null");
            
            // Assert that all test funds are in the retrieved funds
            foreach (var name in testFundNames)
            {
                Assert.Contains(retrievedFunds, f => f.Name == name);
                Console.WriteLine($"Verified fund with name '{name}' was retrieved");
            }
            
            // Assert that all seeded funds are in the retrieved funds
            foreach (var seededFund in seededFunds)
            {
                Assert.Contains(retrievedFunds, f => f.Id == seededFund.Id);
                Console.WriteLine($"Verified fund with ID '{seededFund.Id}' was retrieved");
            }
        }
    }
}
