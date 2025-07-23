using System;
using System.Windows;
using System.Windows.Controls;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class AddAccountDialog : Window
    {
        private readonly AccountService _accountService;
        public bool IsSuccess { get; private set; } = false;

        public AddAccountDialog()
        {
            InitializeComponent();
            _accountService = new AccountService();
            
            // Set default role
            RoleComboBox.SelectedIndex = 0; // Student
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên đăng nhập", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    UsernameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    FullNameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập email", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EmailTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(PasswordBox.Password))
                {
                    MessageBox.Show("Vui lòng nhập mật khẩu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PasswordBox.Focus();
                    return;
                }

                if (RoleComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn vai trò", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    RoleComboBox.Focus();
                    return;
                }

                // Validate email format
                if (!IsValidEmail(EmailTextBox.Text))
                {
                    MessageBox.Show("Email không hợp lệ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EmailTextBox.Focus();
                    return;
                }

                // Validate password length
                if (PasswordBox.Password.Length < 6)
                {
                    MessageBox.Show("Mật khẩu phải có ít nhất 6 ký tự", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PasswordBox.Focus();
                    return;
                }

                // Get selected role
                var selectedRole = ((ComboBoxItem)RoleComboBox.SelectedItem).Content.ToString();

                // Get birth date if selected
                DateOnly? birthDate = null;
                if (BirthDatePicker.SelectedDate.HasValue)
                {
                    birthDate = DateOnly.FromDateTime(BirthDatePicker.SelectedDate.Value);
                }

                // Disable save button to prevent double submission
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Đang lưu...";

                // Add account
                await _accountService.AddAccountAsync(
                    UsernameTextBox.Text.Trim(),
                    FullNameTextBox.Text.Trim(),
                    EmailTextBox.Text.Trim(),
                    PasswordBox.Password,
                    selectedRole,
                    string.IsNullOrWhiteSpace(PhoneTextBox.Text) ? null : PhoneTextBox.Text.Trim(),
                    birthDate
                );

                MessageBox.Show("Thêm tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                IsSuccess = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true;
                SaveButton.Content = "Lưu";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _accountService?.Dispose();
            base.OnClosed(e);
        }
    }
}
