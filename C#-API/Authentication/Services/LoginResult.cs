using Enhanzer_API.Authentication.Contracts;

namespace Enhanzer_API.Authentication.Services;

public enum LoginStatus
{
    Authenticated,
    InvalidCredentials,
    ServerError,
    NotConfigured
}

public sealed record LoginResult(LoginStatus Status, LoginResponse? Response = null);
