Feature: User using video entity
As a user of the system
I want to create a video entity

Scenario: Successful created
    Given a valid e-mail "Teste@gmail.com"
    Given a valid Url "www.video.com"
    Given a valid status "Pronto"
    When creating the entity
    Then no exception should be thrown

    Scenario: Faill creating because e-mail
        Given a invalid e-mail ""
        Given a valid Url "www.video.com"
        Given a valid status "Pronto"
        When creating the entity
        Then exception should be thrown

    Scenario: Faill creating because Url
        Given a valid e-mail "Teste@gmail.com"
        Given a invalid Url ""
        Given a valid status "Pronto"
        When creating the entity
        Then exception should be thrown

