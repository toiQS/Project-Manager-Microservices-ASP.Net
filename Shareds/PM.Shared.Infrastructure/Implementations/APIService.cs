using PM.Shared.Infrastructure.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;

namespace PM.Shared.Infrastructure.Implementations
{
    public class APIService<T> : IAPIService<T> where T : class
    {
        private readonly string _baseUrl;
        private readonly HttpClient _httpClient;

        public APIService(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = baseUrl?.TrimEnd('/') ?? throw new ArgumentNullException(nameof(baseUrl));
        }

        public Task<T> APIsGetAsync(string endpoint)
            => SendRequestAsync<T>(HttpMethod.Get, endpoint);

        public Task<T> APIsPostAsync(string endpoint, object data)
            => SendRequestAsync<T>(HttpMethod.Post, endpoint, data);

        public Task<T> APIsPutAsync(string endpoint, object data)
            => SendRequestAsync<T>(HttpMethod.Put, endpoint, data);

        public Task<T> APIsPatchAsync(string endpoint, object data)
            => SendRequestAsync<T>(HttpMethod.Patch, endpoint, data);

        public Task<T> APIsDeleteAsync(string endpoint, object data)
            => SendRequestAsync<T>(HttpMethod.Delete, endpoint, data);

        private async Task<T> SendRequestAsync<TResponse>(HttpMethod method, string endpoint, object? data = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("Endpoint cannot be null or empty.", nameof(endpoint));

            var request = new HttpRequestMessage(method, $"{_baseUrl}/{endpoint}");

            if (data != null)
            {
                request.Content = JsonContent.Create(data);
            }

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"HTTP {(int)response.StatusCode}: {errorContent}");
            }

            var responseStream = await response.Content.ReadAsStreamAsync();

            var result = await JsonSerializer.DeserializeAsync<T>(responseStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (result == null)
                throw new InvalidOperationException("Failed to deserialize response.");

            return result;
        }
    }
}
