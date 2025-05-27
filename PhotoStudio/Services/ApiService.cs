using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PhotoStudio.Models;

namespace PhotoStudio.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://api.photostudio.com"; // Замените на реальный URL API
        
        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private string _authToken;
        
        public ApiService(string baseUrl = "http://45.9.75.37:5000")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }
        
        // Установка токена авторизации для запросов, требующих аутентификации
        public void SetAuthToken(string token)
        {
            _authToken = token;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        
        // Регистрация нового пользователя
        public async Task<(CreateUserResponse? Response, string? Error)> CreateUserAsync(CreateUserRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/users", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var createUserResponse = await response.Content.ReadFromJsonAsync<CreateUserResponse>();
                    return (createUserResponse, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    return (null, errorResponse?.Error ?? "Неизвестная ошибка при создании пользователя");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        
        // Аутентификация пользователя
        public async Task<(LoginResponse? Response, string? Error)> LoginAsync(LoginRequest request)
        {
            try
            {
                // Поскольку у нас нет отдельного эндпоинта для логина с токеном,
                // мы сначала получаем данные пользователя по имени, затем проверяем пароль
                // (в реальном приложении пароль должен проверяться на сервере)
                var users = await GetUsersAsync();
                if (users.Error != null)
                {
                    return (null, users.Error);
                }
                
                var user = users.Response.Find(u => u.Username == request.Username);
                if (user == null)
                {
                    return (null, "Пользователь не найден");
                }
                
                // В реальном приложении вместо этого должен быть запрос на сервер для проверки пароля
                // Создаем имитацию ответа логина
                var loginResponse = new LoginResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Token = "sample-auth-token-" + Guid.NewGuid().ToString(),
                    Message = "Вход выполнен успешно"
                };
                
                // Записываем время входа
                await RecordLoginAsync(user.Id, request.DeviceInfo);
                
                SetAuthToken(loginResponse.Token);
                return (loginResponse, null);
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        
        // Отправка запроса на восстановление пароля
        public async Task<(ApiSuccessResponse? Response, string? Error)> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                // Поскольку у нас нет эндпоинта для восстановления пароля,
                // мы создаем имитацию ответа
                var response = new ApiSuccessResponse
                {
                    Message = "Инструкции по восстановлению пароля отправлены на указанный email"
                };
                
                // Здесь обычно был бы запрос к API
                // Имитируем задержку сети
                await Task.Delay(1000);
                
                return (response, null);
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        
        // Получение списка пользователей
        public async Task<(List<UserResponse>? Response, string? Error)> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/users");
                
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
                    return (users, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    return (null, errorResponse?.Error ?? "Ошибка при получении списка пользователей");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        
        // Получение данных пользователя по ID
        public async Task<(UserResponse? Response, string? Error)> GetUserAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/users/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserResponse>();
                    return (user, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    return (null, errorResponse?.Error ?? "Пользователь не найден");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        
        // Обновление данных пользователя
        public async Task<(ApiSuccessResponse? Response, string? Error)> UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/api/users/{userId}", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiSuccessResponse>();
                    return (result, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    return (null, errorResponse?.Error ?? "Ошибка при обновлении данных пользователя");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        
        // Запись информации о входе пользователя
        public async Task<(ApiSuccessResponse? Response, string? Error)> RecordLoginAsync(int userId, string deviceInfo = "")
        {
            try
            {
                var request = new { device_info = deviceInfo };
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/users/{userId}/login", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiSuccessResponse>();
                    return (result, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    return (null, errorResponse?.Error ?? "Ошибка при записи информации о входе");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        
        // Получение активности пользователя
        public async Task<(List<dynamic>? Response, string? Error)> GetUserActivitiesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/activities/user/{userId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var activities = await response.Content.ReadFromJsonAsync<List<dynamic>>();
                    return (activities, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    return (null, errorResponse?.Error ?? "Ошибка при получении активности пользователя");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        
        // Получение статистики системы
        public async Task<(dynamic? Response, string? Error)> GetStatisticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/statistics");
                
                if (response.IsSuccessStatusCode)
                {
                    var statistics = await response.Content.ReadFromJsonAsync<dynamic>();
                    return (statistics, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorContent);
                    return (null, errorResponse?.Error ?? "Ошибка при получении статистики");
                }
            }
            catch (Exception ex)
            {
                return (null, $"Ошибка при подключении к API: {ex.Message}");
            }
        }
        // Примечание: метод LoginAsync уже существует в классе, 
        // поэтому мы удалили дублирующуюся реализацию
    }
}
