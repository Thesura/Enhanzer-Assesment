namespace Enhanzer_API.Locations.Services;

public enum LocationSaveStatus
{
    Saved,
    InvalidBatch,
    DuplicateLocationCode
}

public sealed record LocationSaveResult(LocationSaveStatus Status);
