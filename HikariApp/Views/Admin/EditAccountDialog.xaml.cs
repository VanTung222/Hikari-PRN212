using System;
using System.Text.RegularExpressions;
using System.Windows;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class EditAccountDialog : Window
    {
        private readonly AccountService _accountService;
        private readonly AccountViewModel _account;

        public EditAccountDialog(AccountViewModel account)
        {
            InitializeComponent();
            _accountService = new AccountService();
            _account = account;
            LoadAccountData();
        }

        private void LoadAccountData()
        {
            IdTextBox.Text = _account.Id;
            UsernameTextBox.Text = _account.Username;
            FullNameTextBox.Text = _account.FullName;
            EmailTextBox.Text = _account.Email;
            PhoneTextBox.Text = _account.Phone;
            RoleTextBox.Text = _account.Role;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(FullNameTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập họ tên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    FullNameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập email.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EmailTextBox.Focus();
                    return;
                }

                // Validate email format
                if (!IsValidEmail(EmailTextBox.Text))
                {
                    MessageBox.Show("Email không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EmailTextBox.Focus();
                    return;
                }

                // Validate phone number if provided
                if (!string.IsNullOrWhiteSpace(PhoneTextBox.Text) && !IsValidPhoneNumber(PhoneTextBox.Text))
                {
                    MessageBox.Show("Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại 10-11 chữ số.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PhoneTextBox.Focus();
                    return;
                }

                // Update account
                bool success = await _accountService.UpdateAccountAsync(
                    _account.Id,
                    FullNameTextBox.Text.Trim(),
                    EmailTextBox.Text.Trim(),
                    PhoneTextBox.Text?.Trim()
                );

                if (success)
                {
                    MessageBox.Show("Cập nhật thông tin tài khoản thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Không thể cập nhật thông tin tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật tài khoản: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return emailRegex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            try
            {
                var phoneRegex = new Regex(@"^[0-9]{10,11}$");
                return phoneRegex.IsMatch(phone);
            }
            catch
            {
                return false;
            }
        }
    }
}
