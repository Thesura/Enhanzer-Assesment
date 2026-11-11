using System.ComponentModel.DataAnnotations;

namespace Enhanzer_API.Authentication.Contracts;

/// <summary>Credentials submitted to sign in.</summary>
public sealed record LoginRequest
{
    /// <summary>The email address associated with the account.</summary>
    [Required, EmailAddress, MaxLength(254)]
    public required string Email { get; init; }

    /// <summary>The account password.</summary>
    [Required]
    public required string Password { get; init; }
}
