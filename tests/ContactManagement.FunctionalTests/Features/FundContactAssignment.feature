Feature: Fund Contact Assignment
  As a user
  I want to assign contacts to funds
  So that I can manage fund relationships

  Background:
    Given the system has pre-seeded funds
    And there are existing contacts

  Scenario: Assign contact to fund
    Given a contact and a fund
    When I assign the contact to the fund
    Then the assignment should be successful
    And the contact should be linked to the fund

  Scenario: Prevent duplicate assignment
    Given a contact already assigned to a fund
    When I attempt to assign the same contact again
    Then the system should return a duplicate assignment error

  Scenario: Remove contact from fund
    Given a contact assigned to a fund
    When I remove the contact from the fund
    Then the assignment should be removed successfully