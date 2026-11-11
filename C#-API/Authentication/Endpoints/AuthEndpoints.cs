using Enhanzer_API.Authentication.Contracts;
using Enhanzer_API.Authentication.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Enhanzer_API.Authentication.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/auth/login", LoginAsync)
            .WithName("Login")
            .WithSummary("Sign in with email and password")
            .WithDescription("Validates credentials and returns an access token when authentication is configured.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status501NotImplemented);

        return endpoints;
    }

    private static async Task<Results<Ok<LoginResponse>, UnauthorizedHttpResult, ProblemHttpResult>> LoginAsync(
        LoginRequest request,
        IAuthenticationService authenticationService,
        CancellationToken cancellationToken)
    {
        var result = await authenticationService.LoginAsync(request, cancellationToken);

        return result.Status switch
        {
            LoginStatus.Authenticated when result.Response is not null =>
                TypedResults.Ok(result.Response),
            LoginStatus.InvalidCredentials =>
                TypedResults.Unauthorized(),
            LoginStatus.ServerError =>
                TypedResults.Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Authentication service error",
                    detail: "An error occurred while processing the authentication request."),
            LoginStatus.NotConfigured =>
                TypedResults.Problem(
                    statusCode: StatusCodes.Status501NotImplemented,
                    title: "Authentication is not configured",
                    detail: "Credential validation and token issuance have not been configured."),
            _ => throw new InvalidOperationException("The authentication service returned an invalid login result.")
        };
    }
}
