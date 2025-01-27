Feature: Video Processing Request
As a user of the system
I want to submit a video file for processing
So that the video can be stored and queued for further processing

    Scenario: Successful video processing request
        Given I have a valid video file named "video.mp4"
        When I request video processing
        Then the process should succeed

    Scenario: Invalid file extension
        Given I have an invalid video file named "video.txt"
        When I request video processing
        Then an error should occur with the message "O arquivo enviado não possui uma extensão válida. Formatos aceitos: .mp4,.mkv"

    Scenario: Missing video file
        Given I do not provide a video file
        When I request video processing
        Then an error should occur with the message "O arquivo não foi informado."

    Scenario: Upload failure
        Given I have a valid video file named "video.mp4"
        And the upload service is unavailable
        When I request video processing
        Then an error should occur with the message "Error uploading the video."