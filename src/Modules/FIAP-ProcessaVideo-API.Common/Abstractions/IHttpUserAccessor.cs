using Microsoft.Extensions.Primitives;

namespace FIAP_ProcessaVideo_API.Common.Abstractions;

public interface IHttpUserAccessor
{
    string Email { get; }
    string Username { get; }
    string AuthorizationToken { get; }
    Dictionary<string, StringValues> QueryString { get; }
}
