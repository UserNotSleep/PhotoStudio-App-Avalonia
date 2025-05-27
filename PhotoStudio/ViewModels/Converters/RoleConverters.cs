using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using static PhotoStudio.ViewModels.AuthViewModel;

namespace PhotoStudio.ViewModels.Converters
{
    public class RoleTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role)
            {
                return role switch
                {
                    UserRole.Photographer => "Фотограф",
                    UserRole.Client => "Клиент",
                    UserRole.Admin => "Администратор",
                    _ => "Не выбрана"
                };
            }
            
            var roleStr = value as string;
            var paramRoleStr = parameter as string;
            
            if (roleStr != null && paramRoleStr != null)
            {
                return roleStr == paramRoleStr ? "Выбрано" : "Выбрать";
            }
            
            return "Не выбрана";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class RoleBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role && parameter is string roleStr)
            {
                bool isSelected = (role == UserRole.Photographer && roleStr == "Photographer") ||
                                 (role == UserRole.Client && roleStr == "Client");

                return isSelected 
                    ? roleStr == "Photographer" ? new SolidColorBrush(Color.Parse("#EFF6FF")) : new SolidColorBrush(Color.Parse("#F5F3FF"))
                    : new SolidColorBrush(Colors.White);
            }
            
            var roleStr1 = value as string;
            var paramRoleStr = parameter as string;
            
            if (roleStr1 != null && paramRoleStr != null)
            {
                return roleStr1 == paramRoleStr ? 
                    new SolidColorBrush(Color.Parse("#EFF6FF")) : 
                    new SolidColorBrush(Color.Parse("#FFFFFF"));
            }
            
            return new SolidColorBrush(Colors.White);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RoleBorderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role && parameter is string roleStr)
            {
                bool isSelected = (role == UserRole.Photographer && roleStr == "Photographer") ||
                                 (role == UserRole.Client && roleStr == "Client");

                return isSelected 
                    ? roleStr == "Photographer" ? new SolidColorBrush(Color.Parse("#3B82F6")) : new SolidColorBrush(Color.Parse("#8B5CF6"))
                    : new SolidColorBrush(Color.Parse("#E5E7EB"));
            }
            
            var roleStr1 = value as string;
            var paramRoleStr = parameter as string;
            
            if (roleStr1 != null && paramRoleStr != null)
            {
                return roleStr1 == paramRoleStr ? 
                    new SolidColorBrush(paramRoleStr == "photographer" ? Color.Parse("#2563EB") : Color.Parse("#9333EA")) : 
                    new SolidColorBrush(Color.Parse("#E5E7EB"));
            }
            
            return new SolidColorBrush(Color.Parse("#E5E7EB"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RoleSelectedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role && parameter is string roleStr)
            {
                return (role == UserRole.Photographer && roleStr == "Photographer") ||
                       (role == UserRole.Client && roleStr == "Client");
            }
            
            // If no parameter, just check if any role is selected
            if (value is UserRole selectedRole && parameter == null)
            {
                return selectedRole != UserRole.None;
            }
            
            var roleStr1 = value as string;
            var paramRoleStr = parameter as string;
            
            if (targetType == typeof(bool) && roleStr1 != null)
            {
                return !string.IsNullOrEmpty(roleStr1);
            }
            
            if (roleStr1 != null && paramRoleStr != null)
            {
                return roleStr1 == paramRoleStr ? 
                    new SolidColorBrush(paramRoleStr == "photographer" ? Color.Parse("#2563EB") : Color.Parse("#9333EA")) : 
                    new SolidColorBrush(Colors.Transparent);
            }
            
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RoleContinueTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UserRole role)
            {
                return role switch
                {
                    UserRole.Photographer => "Продолжить как фотограф",
                    UserRole.Client => "Продолжить как клиент",
                    _ => "Продолжить"
                };
            }
            
            var roleStr = value as string;
            
            if (roleStr != null)
            {
                if (string.IsNullOrEmpty(roleStr))
                    return "Выберите роль";
                
                return roleStr == "photographer" ? 
                    "Продолжить как фотограф" : 
                    "Продолжить как клиент";
            }
            
            return "Продолжить";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class DashboardBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentView = value as string;
            var paramView = parameter as string;
            
            return currentView == paramView ? 
                new SolidColorBrush(Color.Parse("#2563EB")) : 
                new SolidColorBrush(Colors.Transparent);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class DashboardForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var currentView = value as string;
            var paramView = parameter as string;
            
            return currentView == paramView ? 
                new SolidColorBrush(Colors.White) : 
                new SolidColorBrush(Color.Parse("#6B7280"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
