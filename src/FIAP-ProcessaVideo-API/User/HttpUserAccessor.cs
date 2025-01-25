using FIAP_ProcessaVideo_API.Common.Abstractions;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;

namespace FIAP_ProcessaVideo_API.User;

public class HttpUserAccessor : IHttpUserAccessor
{
    private readonly IHttpContextAccessor _accessor;

    public HttpUserAccessor(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public string Email => _accessor.HttpContext.User.FindFirstValue("email")!;
    public string Username => _accessor.HttpContext.User.FindFirstValue("cognito:username")!;
    public string AuthorizationToken => _accessor.HttpContext.Request.Headers["Authorization"]!;
    public Dictionary<string, StringValues> QueryString => _accessor.HttpContext.Request.Query.ToDictionary();
}