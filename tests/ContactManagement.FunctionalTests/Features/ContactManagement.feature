@contact_management
Feature: Contact Management
  As a user
  I want to manage contacts
  So that I can maintain contact information for funds

  Background:
    Given the system has pre-seeded funds

  @create_contact @positive
  Scenario: Create a valid contact
    Given I provide a contact name
    When I create a contact
    Then the contact should be created successfully
    And the response should contain the contact details