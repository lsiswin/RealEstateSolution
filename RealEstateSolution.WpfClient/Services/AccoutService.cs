using RealEstateSolution.AuthService.Models;
using System.Threading.Tasks;

namespace RealEstateSolution.WpfClient.Services
{
    public class AccoutService : HttpService
    {
        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            return await PostAsync<AuthResponse, LoginRequest>("/api/auth/login", request);
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
        {
            return await PostAsync<AuthResponse, RegisterRequest>("/api/auth/register", request);
        }

        public async Task<bool> SendVerificationCodeAsync(string phone)
        {
            var response = await PostAsync<AuthResponse, object>($"/api/auth/sendcode?phone={phone}", null!);
            return response?.Success ?? false;
        }

        public async Task<bool> LogoutAsync(string accessToken)
        {
            var request = new LogoutRequest { AccessToken = accessToken };
            var response = await PostAsync<AuthResponse, LogoutRequest>("/api/auth/logout", request);
            return response?.Success ?? false;
        }

        public async Task<AuthResponse?> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var request = new RefreshTokenRequest
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return await PostAsync<AuthResponse, RefreshTokenRequest>("/api/auth/refresh", request);
        }
    }
} 