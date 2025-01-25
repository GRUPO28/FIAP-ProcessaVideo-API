namespace FIAP_ProcessaVideo_API.Infrastructure.Configurations;

public class AwsSettings
{
    public string Region { get; set; } = string.Empty;
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public CognitoSettings Cognito { get; set; } = new();
}

public class CognitoSettings
{
    public string UserPoolId { get; set; }
    public string IdentityPoolId { get; set; }
    public string AppClientId { get; set; }
}
