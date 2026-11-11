using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Enhanzer_API.Locations.Contracts;

/// <summary>Location fields accepted when saving a location.</summary>
public sealed record LocationRequest
{
    /// <summary>The unique location code.</summary>
    [JsonPropertyName("Location_Code")]
    [MaxLength(100)]
    public string LocationCode { get; init; }

    /// <summary>The display name of the location.</summary>
    [JsonPropertyName("Location_Name")]
    [MaxLength(200)]
    public string LocationName { get; init; }

    /// <summary>Whether this location handles stock.</summary>
    [JsonPropertyName("Stock_Handle")]
    public int StockHandle { get; init; }

    /// <summary>The location address.</summary>
    [JsonPropertyName("Address")]
    [MaxLength(1000)]
    public string Address { get; init; }

    /// <summary>The location phone number.</summary>
    [JsonPropertyName("Phone")]
    [MaxLength(50)]
    public string Phone { get; init; }

    /// <summary>The location status code.</summary>
    [JsonPropertyName("Status")]
    public int Status { get; init; }

    public LocationRequest(
        string locationCode = "",
        string locationName = "",
        int stockHandle = 0,
        string address = "",
        string phone = "",
        int status = 0)
    {
        LocationCode = locationCode;
        LocationName = locationName;
        StockHandle = stockHandle;
        Address = address;
        Phone = phone;
        Status = status;
    }
}
