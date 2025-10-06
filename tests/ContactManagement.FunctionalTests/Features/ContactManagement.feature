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

  @create_contact @negative
  Scenario: Fail to create contact without name
    Given I do not provide a contact name
    When I attempt to create a contact
    Then the system should return a validation error

  @update_contact @positive
  Scenario: Update contact information
    Given an existing contact
    When I update the contact's email
    Then the contact should be updated successfully
    And the response should reflect the changes

  @update_contact @negative
  Scenario: Update contact information with invalid email
    Given an existing contact
    When I update the contact's email with an invalid email
    Then the system should return a validation error

  @delete_contact @positive
  Scenario: Delete an unassigned contact
    Given a contact not assigned to any fund
    When I delete the contact
    Then the contact should be removed successfully

  @delete_contact @negative
  Scenario: Fail to delete assigned contact
    Given a contact assigned to a fund
    When I attempt to delete the contact
    Then the system should return a validation error