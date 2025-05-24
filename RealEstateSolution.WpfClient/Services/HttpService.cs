using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RealEstateSolution.WpfClient.Services
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public HttpService()
        {
            _httpClient = new HttpClient { BaseAddress = new System.Uri("http://localhost:5000") };
        }

        protected async Task<TResponse?> GetAsync<TResponse>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            return await HandleResponse<TResponse>(response);
        }

        protected async Task<TResponse?> PostAsync<TResponse, TRequest>(string url, TRequest data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return await HandleResponse<TResponse>(response);
        }

        protected async Task<TResponse?> PutAsync<TResponse, TRequest>(string url, TRequest data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);
            return await HandleResponse<TResponse>(response);
        }

        protected async Task<bool> DeleteAsync(string url)
        {
            var response = await _httpClient.DeleteAsync(url);
            return response.IsSuccessStatusCode;
        }

        private static async Task<T?> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"请求失败: {response.StatusCode}, {content}");
            }

            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }
    }
} 