using System;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace PhotoStudio.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        // Текущий вид в панели управления
        [Reactive] public string CurrentView { get; set; }
        
        // Роль пользователя (фотограф или клиент)
        [Reactive] public string UserRole { get; set; }
        
        // Команды для навигации
        public ICommand NavigateToView { get; }
        public ICommand ChangeRoleCommand { get; }
        
        public DashboardViewModel()
        {
            // Устанавливаем начальный вид
            CurrentView = UserRole == "photographer" ? "dashboard" : "feed";
            
            // Инициализируем команды
            NavigateToView = ReactiveCommand.Create<string>(OnNavigateToView);
            ChangeRoleCommand = ReactiveCommand.Create(OnChangeRole);
        }
        
        // Обработчик команды навигации к выбранному виду
        private void OnNavigateToView(string view)
        {
            CurrentView = view;
        }
        
        // Обработчик команды смены роли
        private void OnChangeRole()
        {
            // Здесь будет логика для возврата к экрану выбора роли
            // Временная реализация для тестирования
            if (UserRole == "photographer")
            {
                UserRole = "client";
                CurrentView = "feed";
            }
            else
            {
                UserRole = "photographer";
                CurrentView = "dashboard";
            }
        }
    }
}
