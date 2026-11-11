using System.Text.Json.Serialization;

namespace Enhanzer_API.Locations.Contracts;

/// <summary>Location returned by the API.</summary>
public sealed record LocationResponse(
    [property: JsonPropertyName("Location_Code")] string LocationCode,
    [property: JsonPropertyName("Location_Name")] string LocationName,
    [property: JsonPropertyName("Stock_Handle")] int StockHandle,
    [property: JsonPropertyName("Address")] string Address,
    [property: JsonPropertyName("Phone")] string Phone,
    [property: JsonPropertyName("Status")] int Status);
