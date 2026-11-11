using Enhanzer_API.Locations.Contracts;
using Enhanzer_API.Locations.Entities;
using Enhanzer_API.Locations.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Enhanzer_API.Locations.Services;

public sealed class LocationsService(LocationsDbContext dbContext) : ILocationsService
{
    private const int MaximumBatchSize = 1000;

    public async Task<IReadOnlyList<LocationResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Locations
            .AsNoTracking()
            .OrderBy(location => location.LocationCode)
            .Select(location => new LocationResponse(
                location.LocationCode,
                location.LocationName,
                location.StockHandle,
                location.Address,
                location.Phone,
                location.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<LocationSaveResult> SaveAsync(
        IReadOnlyCollection<LocationRequest> locations,
        CancellationToken cancellationToken)
    {
        if (locations.Count is 0 or > MaximumBatchSize)
        {
            return new LocationSaveResult(LocationSaveStatus.InvalidBatch);
        }

        var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (locations.Any(location => !seenCodes.Add(location.LocationCode)))
        {
            return new LocationSaveResult(LocationSaveStatus.DuplicateLocationCode);
        }

        var codes = locations.Select(location => location.LocationCode).ToArray();
        var existingCodeFound = await dbContext.Locations
            .AsNoTracking()
            .AnyAsync(location => codes.Contains(location.LocationCode), cancellationToken);

        if (existingCodeFound)
        {
            return new LocationSaveResult(LocationSaveStatus.DuplicateLocationCode);
        }

        dbContext.Locations.AddRange(locations.Select(location => new Location
        {
            LocationCode = location.LocationCode,
            LocationName = location.LocationName,
            StockHandle = location.StockHandle,
            Address = location.Address,
            Phone = location.Phone,
            Status = location.Status
        }));

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicateKey(exception))
        {
            return new LocationSaveResult(LocationSaveStatus.DuplicateLocationCode);
        }

        return new LocationSaveResult(LocationSaveStatus.Saved);
    }

    private static bool IsDuplicateKey(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current is SqlException { Number: 2601 or 2627 })
            {
                return true;
            }
        }

        return false;
    }
}
