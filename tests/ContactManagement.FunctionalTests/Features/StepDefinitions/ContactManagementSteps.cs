using ContactManagement.Application.DTOs;
using ContactManagement.FunctionalTests.Features.Support;
using System.Net;
using System.Text.Json;

namespace ContactManagement.FunctionalTests.Features.StepDefinitions
{
    [Binding]
    public class ContactManagementSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private TestContext TestContext => _scenarioContext.Get<TestContext>();

        public ContactManagementSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"the system has pre-seeded funds")]
        public void GivenTheSystemHasPreSeededFunds()
        {
            // Implementation will be added later
            // This would typically verify or setup test data for funds
        }

        [Given(@"I provide a contact name")]
        public void GivenIProvideAContactName()
        {
            // Create a contact DTO with valid data
            var contactDto = new CreateContactDto(
                Name: "John Doe",
                Email: "john.doe@example.com",
                PhoneNumber: "+1234567890");

            _scenarioContext.Set(contactDto, "ContactDto");

            // Log the action for debugging
            Console.WriteLine($"Created contact DTO with name: {contactDto.Name}");
        }

        [Given(@"I do not provide a contact name")]
        public void GivenIDoNotProvideAContactName()
        {
            // Create a contact DTO with empty name
            var contactDto = new CreateContactDto(
                Name: string.Empty,
                Email: "no.name@example.com",
                PhoneNumber: "+1234567890");

            _scenarioContext.Set(contactDto, "ContactDto");

            // Log the action for debugging
            Console.WriteLine("Created contact DTO with empty name");
        }

        [Given(@"an existing contact")]
        public async Task GivenAnExistingContact()
        {
            // Create a contact DTO with valid data
            var contactDto = new CreateContactDto(
                Name: "John Doe",
                Email: "john.doe@example.com",
                PhoneNumber: "+1234567890");

            _scenarioContext.Set(contactDto, "ContactDto");

            // Send the request to create a contact
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/contacts",
                contactDto);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var options = TestContext.JsonOptions;
                var createdContact = JsonSerializer.Deserialize<ContactDto>(content, options);

                if (createdContact != null)
                {
                    _scenarioContext.Set(createdContact, "ExistingContact");
                    Console.WriteLine($"Created contact with ID: {createdContact.Id}");
                }
            }
            else
            {
                Assert.Fail($"Failed to create contact. Status code: {response.StatusCode}");
            }
        }

        [Given(@"a contact not assigned to any fund")]
        public void GivenAContactNotAssignedToAnyFund()
        {
            // Implementation will be added later
        }

        [Given(@"a contact assigned to a fund")]
        public void GivenAContactAssignedToAFund()
        {
            // Implementation will be added later
        }

        [When(@"I create a contact")]
        public async Task WhenICreateAContact()
        {
            // Get the contact DTO from the scenario context
            var contactDto = _scenarioContext.Get<CreateContactDto>("ContactDto");

            Console.WriteLine("Sending request to create contact");

            // Send the request to create a contact
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/contacts",
                contactDto);

            _scenarioContext.Set(response, "Response");

            // If successful, store the created contact
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Contact creation successful with status code: {(int)response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();
                var options = TestContext.JsonOptions;
                var createdContact = JsonSerializer.Deserialize<ContactDto>(content, options);

                if (createdContact != null)
                {
                    _scenarioContext.Set(createdContact, "CreatedContact");
                    Console.WriteLine($"Created contact with ID: {createdContact.Id}");
                }
            }
            else
            {
                Console.WriteLine($"Contact creation failed with status code: {(int)response.StatusCode}");
            }
        }

        [When(@"I attempt to create a contact")]
        public async Task WhenIAttemptToCreateAContact()
        {
            // Get the contact DTO from the scenario context
            var contactDto = _scenarioContext.Get<CreateContactDto>("ContactDto");

            Console.WriteLine("Sending request to create contact (expected to fail)");

            // Send the request to create a contact
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Post,
                "api/contacts",
                contactDto);

            _scenarioContext.Set(response, "Response");

            Console.WriteLine($"Received response with status code: {(int)response.StatusCode}");
        }

        [When(@"I update the contact's email")]
        public async Task WhenIUpdateTheContactsEmail()
        {
            var existingContact = _scenarioContext.Get<ContactDto>("ExistingContact");
            
            // Create an update DTO with new email
            var updateContactDto = new UpdateContactDto(
                Name: existingContact.Name,
                Email: "updated.email@example.com",
                PhoneNumber: existingContact.PhoneNumber);
            
            _scenarioContext.Set(updateContactDto, "UpdateContactDto");
            
            Console.WriteLine($"Updating contact with ID: {existingContact.Id}");
            Console.WriteLine($"New email: {updateContactDto.Email}");
            
            // Send the request to update the contact
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Put,
                $"api/contacts/{existingContact.Id}",
                updateContactDto);
            
            _scenarioContext.Set(response, "Response");
            
            // If successful, store the updated contact
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Contact update successful with status code: {(int)response.StatusCode}");
                
                var content = await response.Content.ReadAsStringAsync();
                var options = TestContext.JsonOptions;
                var updatedContact = JsonSerializer.Deserialize<ContactDto>(content, options);
                
                if (updatedContact != null)
                {
                    _scenarioContext.Set(updatedContact, "UpdatedContact");
                    Console.WriteLine($"Updated contact with ID: {updatedContact.Id}");
                }
            }
            else
            {
                Console.WriteLine($"Contact update failed with status code: {(int)response.StatusCode}");
            }
        }
        
        [When(@"I update the contact's email with an invalid email")]
        public async Task WhenIUpdateTheContactsEmailWithAnInvalidEmail()
        {
            var existingContact = _scenarioContext.Get<ContactDto>("ExistingContact");
            
            // Create an update DTO with invalid email
            var updateContactDto = new UpdateContactDto(
                Name: existingContact.Name,
                Email: "invalid-email-format",
                PhoneNumber: existingContact.PhoneNumber);
            
            _scenarioContext.Set(updateContactDto, "UpdateContactDto");
            
            Console.WriteLine($"Updating contact with ID: {existingContact.Id}");
            Console.WriteLine($"Invalid email: {updateContactDto.Email}");
            
            // Send the request to update the contact
            var response = await TestContext.SendJsonRequest(
                HttpMethod.Put,
                $"api/contacts/{existingContact.Id}",
                updateContactDto);
            
            _scenarioContext.Set(response, "Response");
            
            Console.WriteLine($"Received response with status code: {(int)response.StatusCode}");
        }

        [When(@"I delete the contact")]
        public async Task WhenIDeleteTheContact()
        {
            // Implementation will be added later
        }

        [When(@"I attempt to delete the contact")]
        public async Task WhenIAttemptToDeleteTheContact()
        {
            // Implementation will be added later
        }

        [Then(@"the contact should be created successfully")]
        public void ThenTheContactShouldBeCreatedSuccessfully()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");

            // Assert that the response has a 201 Created status code
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Console.WriteLine("Verified response has 201 Created status code");

            // Assert that the Location header is present
            Assert.True(response.Headers.Location != null, "Location header should be present");
            Console.WriteLine($"Verified Location header is present: {response.Headers.Location}");
        }

        [Then(@"the response should contain the contact details")]
        public void ThenTheResponseShouldContainTheContactDetails()
        {
            // Get the created contact and original DTO from the scenario context
            var createdContact = _scenarioContext.Get<ContactDto>("CreatedContact");
            var contactDto = _scenarioContext.Get<CreateContactDto>("ContactDto");

            // Assert that the created contact is not null
            Assert.NotNull(createdContact);
            Console.WriteLine("Verified created contact is not null");

            // Assert that the created contact contains the expected data
            Assert.Equal(contactDto.Name, createdContact.Name);
            Console.WriteLine($"Verified name matches: {createdContact.Name}");

            Assert.Equal(contactDto.Email, createdContact.Email);
            Console.WriteLine($"Verified email matches: {createdContact.Email}");

            Assert.Equal(contactDto.PhoneNumber, createdContact.PhoneNumber);
            Console.WriteLine($"Verified phone number matches: {createdContact.PhoneNumber}");

            // Verify ID and timestamps
            Assert.NotEqual(Guid.Empty, createdContact.Id);
            Console.WriteLine($"Verified ID is not empty: {createdContact.Id}");

            Assert.True(createdContact.CreatedAt > DateTime.MinValue);
            Console.WriteLine($"Verified CreatedAt is valid: {createdContact.CreatedAt}");
        }

        [Then(@"the system should return a validation error")]
        public void ThenTheSystemShouldReturnAValidationError()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");

            // Assert that the response has either a 400 Bad Request or 422 Unprocessable Entity status code
            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest || 
                response.StatusCode == HttpStatusCode.UnprocessableEntity,
                $"Expected status code to be 400 or 422, but got {(int)response.StatusCode}");
            
            Console.WriteLine($"Verified response has error status code: {(int)response.StatusCode}");
        }

        [Then(@"the contact should be updated successfully")]
        public void ThenTheContactShouldBeUpdatedSuccessfully()
        {
            var response = _scenarioContext.Get<HttpResponseMessage>("Response");
            
            // Assert that the response has a 200 OK status code
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine("Verified response has 200 OK status code");
        }

        [Then(@"the response should reflect the changes")]
        public void ThenTheResponseShouldReflectTheChanges()
        {
            // Get the updated contact and update DTO from the scenario context
            var updatedContact = _scenarioContext.Get<ContactDto>("UpdatedContact");
            var updateContactDto = _scenarioContext.Get<UpdateContactDto>("UpdateContactDto");
            var originalContact = _scenarioContext.Get<ContactDto>("ExistingContact");
            
            // Assert that the updated contact is not null
            Assert.NotNull(updatedContact);
            Console.WriteLine("Verified updated contact is not null");
            
            // Assert that the updated contact contains the expected data
            Assert.Equal(updateContactDto.Name, updatedContact.Name);
            Console.WriteLine($"Verified name matches: {updatedContact.Name}");
            
            Assert.Equal(updateContactDto.Email, updatedContact.Email);
            Console.WriteLine($"Verified email matches: {updatedContact.Email}");
            
            Assert.Equal(updateContactDto.PhoneNumber, updatedContact.PhoneNumber);
            Console.WriteLine($"Verified phone number matches: {updatedContact.PhoneNumber}");
            
            // Verify ID and timestamps
            Assert.Equal(originalContact.Id, updatedContact.Id);
            Console.WriteLine($"Verified ID is unchanged: {updatedContact.Id}");
            
            Assert.Equal(originalContact.CreatedAt, updatedContact.CreatedAt);
            Console.WriteLine($"Verified CreatedAt is unchanged: {updatedContact.CreatedAt}");
            
            Assert.NotNull(updatedContact.UpdatedAt);
            Assert.True(updatedContact.UpdatedAt > originalContact.CreatedAt);
            Console.WriteLine($"Verified UpdatedAt is after CreatedAt: {updatedContact.UpdatedAt}");
        }

        [Then(@"the contact should be removed successfully")]
        public void ThenTheContactShouldBeRemovedSuccessfully()
        {
            // Implementation will be added later
        }

        [Then(@"the system should return a business rule violation error")]
        public void ThenTheSystemShouldReturnABusinessRuleViolationError()
        {
            // Implementation will be added later
        }
    }
}

