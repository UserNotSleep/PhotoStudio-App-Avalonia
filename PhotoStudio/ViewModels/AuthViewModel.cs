using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using PhotoStudio.Models;
using PhotoStudio.Services;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PhotoStudio.ViewModels
{
    // Модель для запроса авторизации
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string DeviceId { get; set; }
    }
    
    // Модель ответа от сервера при авторизации
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
    public class AuthViewModel : ViewModelBase
    {
        // Сервис для работы с API
        private readonly ApiService _apiService;
        
        // Свойства для индикации загрузки и отображения ошибок
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }
    
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }
        
        // Enumeration for different authentication views
        public enum AuthView
        {
            Login,
            Register,
            ForgotPassword,
            RoleSelection,
            App
        }

        // Enumeration for user roles
        public enum UserRole
        {
            None,
            Photographer,
            Client,
            Admin
        }

        private AuthView _currentView = AuthView.Login;
        public AuthView CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }
        
        private UserRole _selectedRole = UserRole.None;
        public UserRole SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }
        
        private bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set => SetProperty(ref _showPassword, value);
        }
        
        private bool _showConfirmPassword;
        public bool ShowConfirmPassword
        {
            get => _showConfirmPassword;
            set => SetProperty(ref _showConfirmPassword, value);
        }

        // Login form properties
        private string _loginEmail = string.Empty;
        public string LoginEmail
        {
            get => _loginEmail;
            set => SetProperty(ref _loginEmail, value);
        }
        
        private string _loginPassword = string.Empty;
        public string LoginPassword
        {
            get => _loginPassword;
            set => SetProperty(ref _loginPassword, value);
        }
        
        private bool _rememberMe;
        public bool RememberMe
        {
            get => _rememberMe;
            set => SetProperty(ref _rememberMe, value);
        }
        
        // Register form properties
        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }
        
        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }
        
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        
        private string _phone = string.Empty;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }
        
        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        
        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }
        
        private string _city = string.Empty;
        public string City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        
        private bool _agreeTerms;
        public bool AgreeTerms
        {
            get => _agreeTerms;
            set => SetProperty(ref _agreeTerms, value);
        }
        
        private bool _agreeMarketing;
        public bool AgreeMarketing
        {
            get => _agreeMarketing;
            set => SetProperty(ref _agreeMarketing, value);
        }
        
        // Forgot password properties
        private string _forgotEmail = string.Empty;
        public string ForgotEmail
        {
            get => _forgotEmail;
            set => SetProperty(ref _forgotEmail, value);
        }

        // Информация о пользователе после успешной аутентификации
        private int _userId;
        public int UserId
        {
            get => _userId;
            set => SetProperty(ref _userId, value);
        }

        private string _username;
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        private string _deviceId;
        public string DeviceId
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        private string _token;
        public string Token
        {
            get => _token;
            set => SetProperty(ref _token, value);
        }

        // Commands
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand SelectRoleCommand { get; }
        public ICommand ContinueCommand { get; }
        public ICommand GoogleLoginCommand { get; }
        public ICommand FacebookLoginCommand { get; }
        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }
        public ICommand NavigateToForgotPasswordCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; }

        // Создаем класс команды, который будет использоваться вместо ReactiveCommand
        public class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool>? _canExecute;

            public RelayCommand(Action execute, Func<bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;

            public void Execute(object? parameter) => _execute();

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        // Класс асинхронной команды
        public class AsyncRelayCommand : ICommand
        {
            private readonly Func<Task> _execute;
            private readonly Func<bool>? _canExecute;
            private bool _isExecuting;

            public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);

            public async void Execute(object? parameter)
            {
                if (_isExecuting)
                    return;

                _isExecuting = true;
                RaiseCanExecuteChanged();

                try
                {
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
            }

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        // Класс команды с параметром
        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            private readonly Func<T, bool>? _canExecute;

            public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
                if (parameter is T typedParameter || parameter == null && default(T) == null)
                    return _canExecute?.Invoke((T)parameter!) ?? true;
                return false;
            }

            public void Execute(object? parameter)
            {
                if (parameter is T typedParameter || parameter == null && default(T) == null)
                    _execute((T)parameter!);
            }

            public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public AuthViewModel()
        {
            // Инициализация API сервиса
            // В реальном приложении URL должен быть в конфигурации
            _apiService = new ApiService("http://45.9.75.37:5000");
            
            // Инициализация команд
            LoginCommand = new AsyncRelayCommand(LoginAsync);
            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            ForgotPasswordCommand = new AsyncRelayCommand(SendForgotPasswordEmailAsync);
            SelectRoleCommand = new RelayCommand<UserRole>(SelectRole);
            ContinueCommand = new RelayCommand(ContinueToApp);
            GoogleLoginCommand = new AsyncRelayCommand(LoginWithGoogleAsync);
            FacebookLoginCommand = new AsyncRelayCommand(LoginWithFacebookAsync);
            
            NavigateToLoginCommand = new RelayCommand(() => CurrentView = AuthView.Login);
            NavigateToRegisterCommand = new RelayCommand(() => CurrentView = AuthView.Register);
            NavigateToForgotPasswordCommand = new RelayCommand(() => CurrentView = AuthView.ForgotPassword);
            
            TogglePasswordVisibilityCommand = new RelayCommand(() => ShowPassword = !ShowPassword);
            ToggleConfirmPasswordVisibilityCommand = new RelayCommand(() => ShowConfirmPassword = !ShowConfirmPassword);
        }

        // Вспомогательный метод для выполнения асинхронных операций с обработкой ошибок и индикацией загрузки
        private async Task ExecuteWithBusyIndicatorAsync(Func<Task> action)
        {
            if (IsBusy)
                return;
                
            try
            {
                ErrorMessage = null;
                IsBusy = true;
                await action();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        // Вспомогательный метод для выполнения асинхронных операций с возвратом результата
        private async Task<T> ExecuteWithBusyIndicatorAsync<T>(Func<Task<T>> action, T defaultValue = default)
        {
            if (IsBusy)
                return defaultValue;
                
            try
            {
                ErrorMessage = null;
                IsBusy = true;
                var result = await action();
                return result is null ? defaultValue : result;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
                return defaultValue;
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        // Получение информации об устройстве
        private string GetDeviceInfo()
        {
            try
            {
                return $"{Environment.OSVersion} - {Environment.MachineName}";
            }
            catch
            {
                return "Desktop Client";
            }
        }
        
        private async Task LoginAsync()
        {
            await ExecuteWithBusyIndicatorAsync(async () =>
            {
                // Проверка входных данных
                if (string.IsNullOrWhiteSpace(LoginEmail))
                {
                    ErrorMessage = "Пожалуйста, введите email";
                    return;
                }
        
                if (string.IsNullOrWhiteSpace(LoginPassword))
                {
                    ErrorMessage = "Пожалуйста, введите пароль";
                    return;
                }
        
                // Вызов API для аутентификации
                var request = new LoginRequest
                {
                    Username = LoginEmail, // Используем email как username
                    Password = LoginPassword
                };
        
                var (response, error) = await _apiService.LoginAsync(request);
        
                if (response != null)
                {
                    // Сохраняем информацию о пользователе
                    UserId = response.Id;
                    Username = response.Username;
                    Token = response.Token ?? string.Empty;
                    
                    // Переходим к основному приложению
                    CurrentView = AuthView.App;
                }
                else
                {
                    ErrorMessage = error ?? "Неизвестная ошибка при входе";
                }
            });
        }

        private async Task RegisterAsync()
        {
            await ExecuteWithBusyIndicatorAsync(async () =>
            {
                // Проверка входных данных
                if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
                {
                    ErrorMessage = "Пожалуйста, введите имя и фамилию";
                    return;
                }
        
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Пожалуйста, введите email";
                    return;
                }
        
                if (string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Пожалуйста, введите пароль";
                    return;
                }
        
                if (Password != ConfirmPassword)
                {
                    ErrorMessage = "Пароли не совпадают";
                    return;
                }
        
                if (!AgreeTerms)
                {
                    ErrorMessage = "Необходимо принять условия использования";
                    return;
                }
        
                // Создание пользователя через API
                var username = $"{FirstName.ToLower()}.{LastName.ToLower()}"; // Простая логика генерации username
                var request = new CreateUserRequest
                {
                    Username = username,
                    Email = Email,
                    // В реальном приложении нужно добавить DeviceId или сгенерировать его
                    DeviceId = Guid.NewGuid().ToString()
                };
        
                var (response, error) = await _apiService.CreateUserAsync(request);
        
                if (response != null)
                {
                    // Сохраняем информацию о пользователе
                    UserId = response.Id;
                    Username = response.Username;
                    DeviceId = response.DeviceId;
                    
                    // Переходим к выбору роли
                    CurrentView = AuthView.RoleSelection;
                }
                else
                {
                    ErrorMessage = error ?? "Неизвестная ошибка при регистрации";
                }
            });
        }
        
        private async Task SendForgotPasswordEmailAsync()
        {
            await ExecuteWithBusyIndicatorAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(ForgotEmail))
                {
                    ErrorMessage = "Пожалуйста, введите email";
                    return;
                }
        
                // Здесь был бы вызов API для восстановления пароля
                // Но поскольку в предоставленном API нет такого эндпоинта,
                // просто имитируем успешную отправку
        
                await Task.Delay(1000); // Имитация задержки сети
                
                // Переход обратно к форме входа
                CurrentView = AuthView.Login;
                // Уведомление пользователя об отправке инструкций
                ErrorMessage = "Инструкции по восстановлению пароля отправлены на указанный email";
            });
        }
        
        private void SelectRole(UserRole role)
        {
            SelectedRole = role;
        }
        
        private void ContinueToApp()
        {
            // Navigate to the main app screen after selecting a role
            CurrentView = AuthView.App;
        }
        
        private async Task LoginWithGoogleAsync()
        {
            await ExecuteWithBusyIndicatorAsync(async () =>
            {
                // Здесь была бы интеграция с Google OAuth
                // Сейчас просто имитируем успешный вход
        
                await Task.Delay(1000); // Имитация задержки аутентификации
                
                // Переход к основному приложению
                CurrentView = AuthView.App;
            });
        }
        
        private async Task LoginWithFacebookAsync()
        {
            await ExecuteWithBusyIndicatorAsync(async () =>
            {
                // Здесь была бы интеграция с Facebook OAuth
                // Сейчас просто имитируем успешный вход

                await Task.Delay(1000); // Имитация задержки аутентификации
                
                // Переход к основному приложению
                CurrentView = AuthView.App;
            });
        }
    }
}
