Feature: Fund Management
    As a user
    I want to manage funds
    So that I can maintain fund information

    Scenario: Create a valid fund
        Given I provide a fund name
        When I create a fund
        Then the fund should be created successfully

    Scenario: Fail to create fund without name
        Given I do not provide a fund name
        When I attempt to create a fund
        Then the system should return a validation error

    Scenario: Create multiple valid funds
        Given I provide multiple fund names
        When I create multiple funds
        Then the funds should be created successfully

    Scenario: Fail to create multiple funds without names
        Given I provide an empty list of fund names
        When I attempt to create multiple funds
        Then the system should return a validation error
    
    Scenario: Get all funds
        Given I request all funds
        Then the system should return the list of funds
