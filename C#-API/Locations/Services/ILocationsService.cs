using Enhanzer_API.Locations.Contracts;

namespace Enhanzer_API.Locations.Services;

public interface ILocationsService
{
    Task<IReadOnlyList<LocationResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<LocationSaveResult> SaveAsync(
        IReadOnlyCollection<LocationRequest> locations,
        CancellationToken cancellationToken);
}
