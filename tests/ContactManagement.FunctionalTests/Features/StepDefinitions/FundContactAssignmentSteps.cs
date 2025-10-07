using ContactManagement.Application.DTOs;
using ContactManagement.FunctionalTests.Features.Support;
using System.Net;
using System.Text.Json;

namespace ContactManagement.FunctionalTests.Features.StepDefinitions
{
    [Binding]
    public class FundContactAssignmentSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private TestContext TestContext => _scenarioContext.Get<TestContext>();

        public FundContactAssignmentSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"there are existing contacts")]
        public async Task GivenThereAreExistingContacts()
        {
            // Generate 10 contacts programmatically
            var contacts = Enumerable.Range(1, 10)
                .Select(i => new CreateContactDto(
                    Name: $"Test Contact {i}",
                    Email: $"contact{i}@example.com",
                    PhoneNumber: $"+1{i.ToString().PadLeft(9, '0')}"))
                .ToList();

            // Create each contact via the API
            foreach (var contact in contacts)
            {
                var response = await TestContext.SendJsonRequest(
                    HttpMethod.Post,
                    "api/contacts",
                    contact);
                
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to create contact {contact.Name}. Status: {response.StatusCode}");
                }
            }

            Console.WriteLine($"Created {contacts.Count} test contacts");
        }

        [Given(@"a contact and a fund")]
        public async Task GivenAContactAndAFund()
        {
            var contactDto = new CreateContactDto(
                Name: "Test Contact",
                Email: "test.contact@example.com",
                PhoneNumber: "+1234567890");

            var contactResponse = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/contacts",
                contactDto);

            if (!contactResponse.IsSuccessStatusCode)
            {
                Assert.Fail($"Failed to create contact. Status code: {contactResponse.StatusCode}");
            }

            var contactContent = await contactResponse.Content.ReadAsStringAsync();
            var createdContact = JsonSerializer.Deserialize<ContactDto>(contactContent, TestContext.JsonOptions);
            _scenarioContext.Set(createdContact, "Contact");
            Console.WriteLine($"Created contact with ID: {createdContact?.Id}");

            // Generate a unique fund name with a timestamp to avoid conflicts
            var fundDto = new CreateFundDto(
                Name: $"Test Fund {DateTime.Now:yyyyMMdd_HHmmss}");

            var fundResponse = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/funds",
                fundDto);

            if (!fundResponse.IsSuccessStatusCode)
            {
                Assert.Fail($"Failed to create fund. Status code: {fundResponse.StatusCode}");
            }

            var fundContent = await fundResponse.Content.ReadAsStringAsync();
            var createdFund = JsonSerializer.Deserialize<FundDto>(fundContent, TestContext.JsonOptions);
            _scenarioContext.Set(createdFund, "Fund");
            Console.WriteLine($"Created fund with ID: {createdFund?.Id}");
        }

        [Given(@"a contact already assigned to a fund")]
        public async Task GivenAContactAlreadyAssignedToAFund()
        {
            await GivenAContactAndAFund();

            await WhenIAssignTheContactToTheFund();

            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            if (!response.IsSuccessStatusCode)
            {
                Assert.Fail($"Failed to assign contact to fund. Status code: {response.StatusCode}");
            }

            Console.WriteLine("Successfully created a contact assigned to a fund");
        }

        [Given(@"a contact assigned to a fund")]
        public async Task GivenAContactAssignedToAFund()
        {
            // This is the same as the previous step
            await GivenAContactAlreadyAssignedToAFund();
        }

        [Given(@"a fund with assigned contacts")]
        public async Task GivenAFundWithAssignedContacts()
        {
            // Create multiple contacts and assign them to a fund
            await GivenAContactAndAFund();
            await WhenIAssignTheContactToTheFund();

            var contactDto = new CreateContactDto(
                Name: "Second Test Contact",
                Email: "second.test@example.com",
                PhoneNumber: "+1987654321");

            var contactResponse = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/contacts",
                contactDto);

            if (!contactResponse.IsSuccessStatusCode)
            {
                Assert.Fail($"Failed to create second contact. Status code: {contactResponse.StatusCode}");
            }

            var contactContent = await contactResponse.Content.ReadAsStringAsync();
            var createdContact = JsonSerializer.Deserialize<ContactDto>(contactContent, TestContext.JsonOptions);

            var fund = _scenarioContext.Get<FundDto>("Fund");

            var assignDto = new CreateFundContactDto(
                ContactId: createdContact!.Id,
                FundId: fund.Id);

            var assignResponse = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/fundcontacts",
                assignDto);

            if (!assignResponse.IsSuccessStatusCode)
            {
                Assert.Fail($"Failed to assign second contact to fund. Status code: {assignResponse.StatusCode}");
            }

            Console.WriteLine($"Created a fund with multiple assigned contacts. Fund ID: {fund.Id}");
        }

        [When(@"I assign the contact to the fund")]
        public async Task WhenIAssignTheContactToTheFund()
        {
            var contact = _scenarioContext.Get<ContactDto>("Contact");
            var fund = _scenarioContext.Get<FundDto>("Fund");

            var assignDto = new CreateFundContactDto(
                ContactId: contact.Id,
                FundId: fund.Id);

            Console.WriteLine($"Assigning contact {contact.Id} to fund {fund.Id}");

            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/fundcontacts",
                assignDto);

            _scenarioContext.Set(response, "Response");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var fundContact = JsonSerializer.Deserialize<FundContactDto>(content, TestContext.JsonOptions);
                _scenarioContext.Set(fundContact, "FundContact");
                Console.WriteLine($"Created fund contact with ID: {fundContact?.Id}");
            }
            else
            {
                Console.WriteLine($"Assignment request returned status code: {(int)response.StatusCode}");
            }
        }

        [When(@"I attempt to assign the same contact again")]
        public async Task WhenIAttemptToAssignTheSameContactAgain()
        {
            var contact = _scenarioContext.Get<ContactDto>("Contact");
            var fund = _scenarioContext.Get<FundDto>("Fund");

            var assignDto = new CreateFundContactDto(
                ContactId: contact.Id,
                FundId: fund.Id);

            Console.WriteLine($"Attempting to assign contact {contact.Id} to fund {fund.Id} again");

            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/fundcontacts",
                assignDto);

            _scenarioContext.Set(response, "Response");
            Console.WriteLine($"Received response with status code: {(int)response.StatusCode}");
        }

        [When(@"I remove the contact from the fund")]
        public async Task WhenIRemoveTheContactFromTheFund()
        {
            var contact = _scenarioContext.Get<ContactDto>("Contact");
            var fund = _scenarioContext.Get<FundDto>("Fund");

            Console.WriteLine($"Removing contact {contact.Id} from fund {fund.Id}");

            var response = await TestContext.Client.DeleteAsync($"api/fundcontacts/{contact.Id}/funds/{fund.Id}");
            _scenarioContext.Set(response, "Response");

            Console.WriteLine($"Received response with status code: {(int)response.StatusCode}");
        }

        [When(@"I request the contacts for the fund")]
        public async Task WhenIRequestTheContactsForTheFund()
        {
            var fund = _scenarioContext.Get<FundDto>("Fund");

            Console.WriteLine($"Requesting contacts for fund {fund.Id}");

            var response = await TestContext.Client.GetAsync($"api/fundcontacts/funds/{fund.Id}/contacts");
            _scenarioContext.Set(response, "Response");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var contacts = JsonSerializer.Deserialize<List<FundContactListItemDto>>(content, TestContext.JsonOptions);
                _scenarioContext.Set(contacts, "FundContacts");
                Console.WriteLine($"Retrieved {contacts?.Count} contacts for fund {fund.Id}");
            }
            else
            {
                Console.WriteLine($"Request failed with status code: {(int)response.StatusCode}");
            }
        }

        [Then(@"the assignment should be successful")]
        public void ThenTheAssignmentShouldBeSuccessful()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Console.WriteLine("Verified response has 201 Created status code");
        }

        [Then(@"the contact should be linked to the fund")]
        public void ThenTheContactShouldBeLinkedToTheFund()
        {
            var fundContact = _scenarioContext.Get<FundContactDto>("FundContact");
            var contact = _scenarioContext.Get<ContactDto>("Contact");
            var fund = _scenarioContext.Get<FundDto>("Fund");

            Assert.NotNull(fundContact);
            Console.WriteLine("Verified fund contact is not null");

            Assert.Equal(contact.Id, fundContact.ContactId);
            Console.WriteLine($"Verified contact ID matches: {fundContact.ContactId}");

            Assert.Equal(fund.Id, fundContact.FundId);
            Console.WriteLine($"Verified fund ID matches: {fundContact.FundId}");

            Assert.Equal(contact.Name, fundContact.ContactName);
            Console.WriteLine($"Verified contact name matches: {fundContact.ContactName}");

            Assert.Equal(fund.Name, fundContact.FundName);
            Console.WriteLine($"Verified fund name matches: {fundContact.FundName}");
        }

        [Then(@"the system should return a duplicate assignment error")]
        public void ThenTheSystemShouldReturnADuplicateAssignmentError()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            Console.WriteLine("Verified response has 409 Conflict status code");
        }

        [Then(@"the assignment should be removed successfully")]
        public void ThenTheAssignmentShouldBeRemovedSuccessfully()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Console.WriteLine("Verified response has 204 No Content status code");
        }

        [Then(@"the system should return the list of contacts")]
        public void ThenTheSystemShouldReturnTheListOfContacts()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            var contacts = _scenarioContext.Get<List<FundContactListItemDto>>("FundContacts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine("Verified response has 200 OK status code");

            Assert.NotNull(contacts);
            Assert.True(contacts.Count > 0, "Contacts list should not be empty");
            Console.WriteLine($"Verified contacts list contains {contacts.Count} items");
        }
    }
}