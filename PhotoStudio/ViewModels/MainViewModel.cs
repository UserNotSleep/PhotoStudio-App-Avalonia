using System;
using System.Windows.Input;

namespace PhotoStudio.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _userName;
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }
        
        private string _welcomeMessage;
        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }
        
        public MainViewModel()
        {
            InitializeAsync();
        }
        
        private async void InitializeAsync()
        {
            try
            {
                // Получаем информацию о пользователе из хранилища
                var userId = await App.Current.Storage.GetItemAsync<string>("user_id");
                var token = await App.Current.Storage.GetItemAsync<string>("auth_token");
                
                if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token))
                {
                    // Загрузка профиля пользователя с сервера или из кэша
                    // var profile = await _apiService.GetUserProfileAsync(userId, token);
                    // UserName = profile.FullName;
                    
                    // Временное решение, пока не реализовано получение профиля
                    UserName = "Пользователь";
                    WelcomeMessage = $"Добро пожаловать, {UserName}!";
                }
                else
                {
                    // Если нет данных пользователя, возвращаемся на экран входа
                    NavigateToLogin();
                }
            }
            catch (Exception ex)
            {
                // Обработка ошибок инициализации
                System.Diagnostics.Debug.WriteLine($"Ошибка инициализации главного экрана: {ex.Message}");
                NavigateToLogin();
            }
        }
        
        private void NavigateToLogin()
        {
            // Получение главного окна
            var mainWindow = App.Current.MainWindow;
            
            // Проверка, что окно инициализировано
            if (mainWindow == null)
            {
                return;
            }
            
            // Создаем представление авторизации
            var authViewModel = new AuthViewModel();
            
            // Устанавливаем DataContext для главного окна
            mainWindow.DataContext = authViewModel;
        }
    }
}
