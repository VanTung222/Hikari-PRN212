using System;
using System.Windows;
using System.Windows.Media;
using HikariBusiness.Services;

namespace HikariApp.Views
{
    public partial class ForgotPasswordWindow : Window
    {
        private readonly UserService _userService;
        private readonly PasswordResetService _passwordResetService;
        private string _currentEmail;

        public ForgotPasswordWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            _passwordResetService = new PasswordResetService();
        }

        private async void BtnSendCode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate email input
                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    ShowStatus("Vui lòng nhập địa chỉ email!", Brushes.Red);
                    txtEmail.Focus();
                    return;
                }

                if (!IsValidEmail(txtEmail.Text))
                {
                    ShowStatus("Định dạng email không hợp lệ!", Brushes.Red);
                    txtEmail.Focus();
                    return;
                }

                btnSendCode.IsEnabled = false;
                ShowStatus("Đang kiểm tra email...", Brushes.Blue);

                // Check if email exists in database
                var user = await _userService.GetUserByEmailAsync(txtEmail.Text.Trim());
                if (user == null)
                {
                    ShowStatus("Email không tồn tại trong hệ thống!", Brushes.Red);
                    txtEmail.Focus();
                    return;
                }

                // Generate and send reset code
                _currentEmail = txtEmail.Text.Trim();
                var resetCode = _passwordResetService.GenerateResetCode(_currentEmail);
                
                ShowStatus("Đang gửi mã khôi phục...", Brushes.Blue);
                var emailResult = await _passwordResetService.SendResetCodeAsync(_currentEmail, resetCode);

                if (emailResult.Success)
                {
                    // Show the reset code in a message box for demo purposes
                    MessageBox.Show($"Mã khôi phục của bạn là: {emailResult.Code}\n(Mã có hiệu lực trong 15 phút)\n\nTrong ứng dụng thực tế, mã này sẽ được gửi qua email.", 
                        "Mã Khôi Phục", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    ShowStatus("Mã khôi phục đã được tạo! Vui lòng nhập mã để tiếp tục.", Brushes.Green);
                    
                    // Switch to step 2
                    pnlStep1.Visibility = Visibility.Collapsed;
                    pnlStep2.Visibility = Visibility.Visible;
                    txtResetCode.Focus();
                }
                else
                {
                    ShowStatus("Có lỗi khi tạo mã khôi phục. Vui lòng thử lại!", Brushes.Red);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi: {ex.Message}", Brushes.Red);
            }
            finally
            {
                btnSendCode.IsEnabled = true;
            }
        }

        private async void BtnResetPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtResetCode.Text))
                {
                    ShowStatus("Vui lòng nhập mã khôi phục!", Brushes.Red);
                    txtResetCode.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewPassword.Password))
                {
                    ShowStatus("Vui lòng nhập mật khẩu mới!", Brushes.Red);
                    txtNewPassword.Focus();
                    return;
                }

                if (txtNewPassword.Password.Length < 6)
                {
                    ShowStatus("Mật khẩu mới phải có ít nhất 6 ký tự!", Brushes.Red);
                    txtNewPassword.Focus();
                    return;
                }

                if (txtNewPassword.Password != txtConfirmNewPassword.Password)
                {
                    ShowStatus("Xác nhận mật khẩu không khớp!", Brushes.Red);
                    txtConfirmNewPassword.Focus();
                    return;
                }

                btnResetPassword.IsEnabled = false;
                ShowStatus("Đang xác thực mã khôi phục...", Brushes.Blue);

                // Verify reset code
                if (!_passwordResetService.VerifyResetCode(_currentEmail, txtResetCode.Text.Trim()))
                {
                    ShowStatus("Mã khôi phục không đúng hoặc đã hết hạn!", Brushes.Red);
                    txtResetCode.Focus();
                    return;
                }

                ShowStatus("Đang cập nhật mật khẩu...", Brushes.Blue);

                // Get user by email to get UserId
                var user = await _userService.GetUserByEmailAsync(_currentEmail);
                if (user == null)
                {
                    ShowStatus("Có lỗi xảy ra. Vui lòng thử lại!", Brushes.Red);
                    return;
                }

                // Update password
                var result = await _userService.UpdatePasswordAsync(user.UserId, txtNewPassword.Password);
                if (result.Success)
                {
                    // Remove used reset code
                    _passwordResetService.RemoveResetCode(_currentEmail);
                    
                    ShowStatus("Đặt lại mật khẩu thành công!", Brushes.Green);
                    
                    // Show success message and close window
                    MessageBox.Show("Mật khẩu đã được đặt lại thành công!\nBạn có thể đăng nhập với mật khẩu mới.", 
                        "Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);
                    
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
                btnResetPassword.IsEnabled = true;
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            // Switch back to step 1
            pnlStep2.Visibility = Visibility.Collapsed;
            pnlStep1.Visibility = Visibility.Visible;
            
            // Clear step 2 fields
            txtResetCode.Clear();
            txtNewPassword.Clear();
            txtConfirmNewPassword.Clear();
            
            txtEmail.Focus();
            ShowStatus("", Brushes.Black);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ShowStatus(string message, Brush color)
        {
            txtStatus.Text = message;
            txtStatus.Foreground = color;
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
            _userService?.Dispose();
            base.OnClosed(e);
        }
    }
}
