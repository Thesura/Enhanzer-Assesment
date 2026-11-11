using Enhanzer_API.Authentication.Contracts;

namespace Enhanzer_API.Authentication.Services;

public interface IAuthenticationService
{
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
}
