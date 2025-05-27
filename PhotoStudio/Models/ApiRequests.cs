using System.Text.Json.Serialization;

namespace PhotoStudio.Models
{
    public class CreateUserRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("device_id")]
        public string DeviceId { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        [JsonPropertyName("device_info")]
        public string DeviceInfo { get; set; } = string.Empty;
    }
    
    public class ForgotPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}
