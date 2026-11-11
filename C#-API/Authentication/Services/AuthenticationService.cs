using Enhanzer_API.Authentication.Contracts;
using Enhanzer_API.Locations.Services;
using Enhanzer_API.Locations.Contracts;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Text.Json.Nodes;

namespace Enhanzer_API.Authentication.Services;

public sealed class AuthenticationService : IAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILocationsService _locationsService;

    public AuthenticationService(HttpClient httpClient, ILocationsService locationsService)
    {
        _httpClient = httpClient;
        _locationsService = locationsService;
    }
    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        string apiUrl = "https://ez-staging-api.azurewebsites.net/api/External_Api/POS_Api/Invoke";
        var payload = new
        {
            API_Action = "GetLoginData",
            Device_Id = "D001",
            Sync_Time = "",
            Company_Code = request.Email,
            API_Body = new {
                Username = request.Email,
                Pw = request.Password,
            }
        };

        Console.WriteLine($"Sending request to {apiUrl} with payload: {JsonSerializer.Serialize(payload)}");

        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"), cancellationToken);

            response.EnsureSuccessStatusCode();

            var responseData = await response.Content.ReadFromJsonAsync<JsonNode>(cancellationToken: cancellationToken);

            var data =  responseData?["Response_Body"]?.AsArray()?.FirstOrDefault();

            string username = data?["User_Display_Name"]?.ToString() ?? string.Empty;

            IReadOnlyCollection<LocationRequest> locations;

            LocationSaveResult saveResult;

            if(string.IsNullOrEmpty(username))
            {
                return new LoginResult(LoginStatus.InvalidCredentials);
            }else
            {
                locations = data?["User_Locations"]?.AsArray()
                    ?.Select(loc => new LocationRequest(
                        loc?["Location_Code"]?.ToString() ?? string.Empty,
                        loc?["Location_Name"]?.ToString() ?? string.Empty,
                        loc?["Stock_Handle"]?.GetValue<int>() ?? 0,
                        loc?["Address"]?.ToString() ?? string.Empty,
                        loc?["Phone"]?.ToString() ?? string.Empty,
                        loc?["Status"]?.GetValue<int>() ?? 0
                    ))
                    .ToArray() ?? Array.Empty<LocationRequest>();

                saveResult = await _locationsService.SaveAsync(locations, cancellationToken);
                if(saveResult.Status == LocationSaveStatus.InvalidBatch)
                {
                    Console.WriteLine("Invalid batch of locations received from authentication service.");
                    return new LoginResult(LoginStatus.ServerError);
                }else if (saveResult.Status == LocationSaveStatus.DuplicateLocationCode || saveResult.Status == LocationSaveStatus.Saved)
                {
                    Console.WriteLine("Locations saved successfully.");
                    return new LoginResult(LoginStatus.Authenticated, new LoginResponse(username, true));
                }
                else
                {
                    Console.WriteLine("Unexpected error while saving locations.");
                    return new LoginResult(LoginStatus.ServerError);
                }
                
            }
        }
        catch (HttpRequestException ex)
        {
            // Log the exception or handle it as needed
            return new LoginResult(LoginStatus.ServerError);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            return new LoginResult(LoginStatus.ServerError);
        }
    }
}
