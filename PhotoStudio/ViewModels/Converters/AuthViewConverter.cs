using System;
using System.Globalization;
using Avalonia.Data.Converters;
using PhotoStudio.Views;

namespace PhotoStudio.ViewModels.Converters
{
    public class AuthViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Когда параметр задан, проверяем, соответствует ли текущее представление указанному параметру
            if (value is AuthViewModel.AuthView view && parameter is string viewStr)
            {
                bool isSelected = (view == AuthViewModel.AuthView.Login && viewStr == "Login") ||
                                 (view == AuthViewModel.AuthView.Register && viewStr == "Register") ||
                                 (view == AuthViewModel.AuthView.ForgotPassword && viewStr == "ForgotPassword") ||
                                 (view == AuthViewModel.AuthView.RoleSelection && viewStr == "RoleSelection");

                return isSelected;
            }
            
            // Когда параметр не задан, создаем соответствующее представление на основе значения
            if (value is AuthViewModel.AuthView authView && parameter == null)
            {
                return authView switch
                {
                    AuthViewModel.AuthView.Login => new LoginView(),
                    AuthViewModel.AuthView.Register => new RegisterView(),
                    AuthViewModel.AuthView.ForgotPassword => new ForgotPasswordView(),
                    AuthViewModel.AuthView.RoleSelection => new RoleSelectionView(),
                    AuthViewModel.AuthView.App => new AppSuccessView(),
                    _ => new LoginView()
                };
            }
            
            // Если ничего не подходит, возвращаем представление по умолчанию
            if (parameter == null)
            {
                return new LoginView();
            }
            
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}