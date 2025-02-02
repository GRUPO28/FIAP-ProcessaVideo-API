Feature: User processing queue request
As a user of the system
I want to sibmit a request for see my queue
So i can see all my process situation

Scenario: Successful queue request
    Given A valid user e-mail "Teste1@gmail.com"
    When I request the queue
    Then I should see at least data

Scenario: No data found in queue request
    Given A valid user e-mail whith no data in DB "Teste1@gmail.com"
    When I request the queue
    Then I should see an exception "Nenhum dado encontrado para o usuário informado."

Scenario: User not sent
    Given User e-mail was not sent
    When I request the queue
    Then I should see an exception "Usuário não informado."
 