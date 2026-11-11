using Enhanzer_API.Locations.Contracts;
using Enhanzer_API.Locations.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Enhanzer_API.Locations.Endpoints;

public static class LocationsEndpoints
{
    public static IEndpointRouteBuilder MapLocationsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var locations = endpoints.MapGroup("/api/locations")
            .WithTags("Locations");

        locations.MapGet("", GetAllAsync)
            .WithName("GetLocations")
            .WithSummary("Retrieve all locations")
            .WithDescription("Returns locations ordered by location code.")
            .Produces<IReadOnlyList<LocationResponse>>(StatusCodes.Status200OK);

        return endpoints;
    }

    private static async Task<Ok<IReadOnlyList<LocationResponse>>> GetAllAsync(
        ILocationsService locationsService,
        CancellationToken cancellationToken)
    {
        var locations = await locationsService.GetAllAsync(cancellationToken);
        return TypedResults.Ok(locations);
    }
}
