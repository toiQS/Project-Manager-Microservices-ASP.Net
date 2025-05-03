using PM.Shared.Dtos.auths;
using PM.Shared.Handle.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace PM.Shared.Handle.Implements
{
    public class APIService<T> : IAPIService<T> where T : class
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        // Constructor can accept baseUrl and HttpClient for flexibility
        public APIService()
        {
            _httpClient = new HttpClient();
            _baseUrl = "https://localhost:5000"; // Default to localhost if not provided
        }

        // Async GET method with improved error handling and response parsing
        public async Task<ServiceResult<T>> APIsGetAsync(string endpoint)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentNullException(nameof(endpoint), "Endpoint cannot be null or empty.");
            }

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_baseUrl}/{endpoint}"),
                    Method = HttpMethod.Get,
                };

                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return ServiceResult<T>.Error($"Error: {content}");
                }

                var context = await response.Content.ReadAsStringAsync();
                // Attempting to deserialize response to type T
                try
                {
                    var result = JsonSerializer.Deserialize<T>(context);
                    return result != null
                        ? ServiceResult<T>.Success(result) // Returning the deserialized response
                        : ServiceResult<T>.Error("Failed to parse response.");
                }
                catch (JsonException jsonEx)
                {
                    return ServiceResult<T>.Error($"Failed to deserialize response: {jsonEx.Message}");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.FromException(ex);
            }
        }

        public async Task<ServiceResult<T>> APIsPostAsync(string endpoint, object dataInput)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                return ServiceResult<T>.Error("Endpoint cannot be null or empty.");
            }

            try
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri($"{_baseUrl}/{endpoint}"),
                    Method = HttpMethod.Post,
                    Content = JsonContent.Create(dataInput)
                };

                var response = await _httpClient.SendAsync(request);

                // Ensure the request itself succeeded
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return ServiceResult<T>.Error($"HTTP Error: {response.StatusCode}, Content: {errorContent}");
                }

                try
                {
                    var responseContext = await response.Content.ReadFromJsonAsync<ServiceResult<T>>();

                    if (responseContext == null)
                    {
                        return ServiceResult<T>.Error("Failed to deserialize the response.");
                    }

                    // Handle the service-level status inside the response body
                    if (responseContext.Status != ResultStatus.Success)
                    {
                        return ServiceResult<T>.Error(responseContext.Message);
                    }

                    return ServiceResult<T>.Success(responseContext.Data);
                }
                catch (JsonException jsonEx)
                {
                    return ServiceResult<T>.Error($"Failed to deserialize JSON: {jsonEx.Message}");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<T>.FromException(ex);
            }
        }

    }
}
