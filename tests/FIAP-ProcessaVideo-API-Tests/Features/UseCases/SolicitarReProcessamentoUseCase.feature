Feature: Solicitar Reprocessamento de Vídeo
As a user
I want to request video reprocessing
So that I can reprocess a video that needs to be re-analyzed

    Scenario: Successfully request video reprocessing
        Given a video with ID "123" exists in the repository
        And the video with ID "123" exists in S3
        And  a message should be sent to the SQS queue
        When I request reprocessing for the video with ID "123"
        Then the result should be true

    Scenario: Request reprocessing for a video that does not exist in the repository
        Given a video with ID "123" does not exist in the repository
        When I request reprocessing for the video that does not exist in the repository with ID "123"
        Then an exception of type ApplicationNotificationException should be thrown with the message "Vídeo não encontrado."

    Scenario: Request reprocessing for a video that does not exist in S3
        Given a video with ID "123" exists in the repository
        And the video with ID "123" does not exist in S3
        When  I request reprocessing for the video that does not exist in S3 with ID "123"
        Then an exception of type ApplicationNotificationException should be thrown with the message "Vídeo não encontrado."

    Scenario: Request reprocessing without providing a video ID
        Given the video ID is not provided
        When I request reprocessing without providing the video ID
        Then an exception of type ApplicationNotificationException should be thrown with the message "Item da fila não informado."