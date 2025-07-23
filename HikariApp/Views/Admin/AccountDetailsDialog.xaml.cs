using System.Windows;
using System.Windows.Media;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class AccountDetailsDialog : Window
    {
        public AccountDetailsDialog(AccountViewModel account)
        {
            InitializeComponent();
            LoadAccountDetails(account);
        }

        private void LoadAccountDetails(AccountViewModel account)
        {
            IdTextBlock.Text = account.Id;
            UsernameTextBlock.Text = account.Username;
            FullNameTextBlock.Text = account.FullName;
            EmailTextBlock.Text = account.Email;
            PhoneTextBlock.Text = string.IsNullOrEmpty(account.Phone) ? "Chưa cập nhật" : account.Phone;
            RegistrationDateTextBlock.Text = account.RegistrationDate;
            CourseCountTextBlock.Text = account.CourseCount.ToString();

            // Set role with color
            RoleTextBlock.Text = account.Role;
            RoleBorder.Background = GetRoleColor(account.Role);

            // Set status with color
            StatusTextBlock.Text = account.Status;
            StatusBorder.Background = GetStatusColor(account.Status);
        }

        private Brush GetRoleColor(string role)
        {
            return role switch
            {
                "Admin" => new SolidColorBrush(Color.FromRgb(220, 53, 69)), // Red
                "Teacher" => new SolidColorBrush(Color.FromRgb(74, 144, 226)), // Blue
                "Student" => new SolidColorBrush(Color.FromRgb(92, 184, 92)), // Green
                _ => new SolidColorBrush(Color.FromRgb(108, 117, 125)) // Gray
            };
        }

        private Brush GetStatusColor(string status)
        {
            return status switch
            {
                "Hoạt động" => new SolidColorBrush(Color.FromRgb(92, 184, 92)), // Green
                "Bị khóa" => new SolidColorBrush(Color.FromRgb(220, 53, 69)), // Red
                _ => new SolidColorBrush(Color.FromRgb(108, 117, 125)) // Gray
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
