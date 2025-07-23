using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using HikariBusiness.Services;

namespace HikariApp.Views
{
    public partial class RegisterWindow : Window
    {
        private readonly UserService _userService;

        public RegisterWindow()
        {
            InitializeComponent();
            _userService = new UserService();
        }

        private void RbTeacher_Checked(object sender, RoutedEventArgs e)
        {
            if (pnlTeacherInfo != null)
                pnlTeacherInfo.Visibility = Visibility.Visible;
        }

        private void RbTeacher_Unchecked(object sender, RoutedEventArgs e)
        {
            if (pnlTeacherInfo != null)
                pnlTeacherInfo.Visibility = Visibility.Collapsed;
        }

        private async void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (!ValidateInput())
                    return;

                // Disable register button during processing
                btnRegister.IsEnabled = false;
                ShowStatus("Đang đăng ký...", Brushes.Blue);

                // Prepare registration data
                var role = rbStudent.IsChecked == true ? "Student" : "Teacher";
                var birthDate = dpBirthDate.SelectedDate.HasValue ? 
                    DateOnly.FromDateTime(dpBirthDate.SelectedDate.Value) : (DateOnly?)null;

                string specialization = null;
                int? experienceYears = null;

                if (rbTeacher.IsChecked == true)
                {
                    specialization = txtSpecialization.Text.Trim();
                    if (int.TryParse(txtExperienceYears.Text.Trim(), out int years))
                        experienceYears = years;
                }

                // Attempt registration
                var result = await _userService.RegisterUserAsync(
                    txtUsername.Text.Trim(),
                    txtFullName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtPassword.Password,
                    role,
                    txtPhone.Text.Trim(),
                    birthDate,
                    specialization,
                    experienceYears
                );

                if (result.Success)
                {
                    ShowStatus("Đăng ký thành công!", Brushes.Green);
                    MessageBox.Show($"Đăng ký thành công!\nMã người dùng của bạn: {result.UserId}", 
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    DialogResult = true;
                    Close();
                }
                else
                {
                    ShowStatus(result.Message, Brushes.Red);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi: {ex.Message}", Brushes.Red);
            }
            finally
            {
                btnRegister.IsEnabled = true;
            }
        }

        private bool ValidateInput()
        {
            // Username validation
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowStatus("Vui lòng nhập tên đăng nhập!", Brushes.Red);
                txtUsername.Focus();
                return false;
            }

            if (txtUsername.Text.Trim().Length < 3)
            {
                ShowStatus("Tên đăng nhập phải có ít nhất 3 ký tự!", Brushes.Red);
                txtUsername.Focus();
                return false;
            }

            // Full name validation
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                ShowStatus("Vui lòng nhập họ và tên!", Brushes.Red);
                txtFullName.Focus();
                return false;
            }

            // Email validation
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowStatus("Vui lòng nhập email!", Brushes.Red);
                txtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text.Trim()))
            {
                ShowStatus("Email không hợp lệ!", Brushes.Red);
                txtEmail.Focus();
                return false;
            }

            // Phone validation
            if (!string.IsNullOrWhiteSpace(txtPhone.Text) && !IsValidPhone(txtPhone.Text.Trim()))
            {
                ShowStatus("Số điện thoại không hợp lệ!", Brushes.Red);
                txtPhone.Focus();
                return false;
            }

            // Password validation
            if (string.IsNullOrWhiteSpace(txtPassword.Password))
            {
                ShowStatus("Vui lòng nhập mật khẩu!", Brushes.Red);
                txtPassword.Focus();
                return false;
            }

            if (txtPassword.Password.Length < 6)
            {
                ShowStatus("Mật khẩu phải có ít nhất 6 ký tự!", Brushes.Red);
                txtPassword.Focus();
                return false;
            }

            // Confirm password validation
            if (txtPassword.Password != txtConfirmPassword.Password)
            {
                ShowStatus("Mật khẩu xác nhận không khớp!", Brushes.Red);
                txtConfirmPassword.Focus();
                return false;
            }

            // Teacher-specific validation
            if (rbTeacher.IsChecked == true)
            {
                if (string.IsNullOrWhiteSpace(txtSpecialization.Text))
                {
                    ShowStatus("Vui lòng nhập chuyên môn!", Brushes.Red);
                    txtSpecialization.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(txtExperienceYears.Text) || 
                    !int.TryParse(txtExperienceYears.Text.Trim(), out int years) || 
                    years < 0)
                {
                    ShowStatus("Vui lòng nhập số năm kinh nghiệm hợp lệ!", Brushes.Red);
                    txtExperienceYears.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhone(string phone)
        {
            var regex = new Regex(@"^[0-9]{10,11}$");
            return regex.IsMatch(phone);
        }

        private void ShowStatus(string message, Brush color)
        {
            txtStatus.Text = message;
            txtStatus.Foreground = color;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _userService?.Dispose();
            base.OnClosed(e);
        }
    }
}
