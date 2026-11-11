namespace Enhanzer_API.Authentication.Contracts;

/// <summary>Authentication token returned after a successful login.</summary>
public sealed record LoginResponse(
    string Username,
    bool LoggedIn);
