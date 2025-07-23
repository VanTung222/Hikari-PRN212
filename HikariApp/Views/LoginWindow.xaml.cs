using System;
using System.Windows;
using System.Windows.Media;
using HikariBusiness.Services;
using HikariDataAccess.Entities;
using System.Threading.Tasks;
using System.ComponentModel;

namespace HikariApp.Views
{
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        private readonly UserService _userService;
        private readonly DatabaseTestService _dbTestService;

        public event EventHandler<UserAccount> LoginSuccessful;
        public event PropertyChangedEventHandler PropertyChanged;
        private UserAccount _loggedInUser;
        
        public UserAccount LoggedInUser 
        { 
            get => _loggedInUser;
            private set 
            { 
                _loggedInUser = value;
                OnPropertyChanged(nameof(LoggedInUser));
            }
        }

        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            _dbTestService = new DatabaseTestService();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtUsernameEmail.Text))
                {
                    ShowStatus("Vui lòng nhập tên đăng nhập hoặc email!", Brushes.Red);
                    txtUsernameEmail.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPassword.Password))
                {
                    ShowStatus("Vui lòng nhập mật khẩu!", Brushes.Red);
                    txtPassword.Focus();
                    return;
                }

                await AttemptLoginAsync();
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi không mong muốn: {ex.Message}", Brushes.Red);
            }
        }

        private async Task AttemptLoginAsync()
        {
            // Disable login button during processing
            btnLogin.IsEnabled = false;
            ShowStatus("Đang đăng nhập...", Brushes.Blue);

            try
            {
                // Add a small delay to show the loading message
                await Task.Delay(300);

                // Attempt login
                var result = await _userService.LoginAsync(txtUsernameEmail.Text.Trim(), txtPassword.Password);

                if (result.Success)
                {
                    LoggedInUser = result.User;
                    ShowStatus("Đăng nhập thành công! Đang chuyển hướng...", Brushes.Green);
                    
                    // Small delay to show success message before navigating
                    await Task.Delay(500);
                    
                    // Raise login successful event
                    LoginSuccessful?.Invoke(this, LoggedInUser);
                    
                    // Navigate to appropriate dashboard based on user role
                    await NavigateByRoleAsync(LoggedInUser);
                }
                else
                {
                    ShowStatus(result.Message, Brushes.Red);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi khi đăng nhập: {ex.Message}", Brushes.Red);
                throw;
            }
            finally
            {
                btnLogin.IsEnabled = true;
            }
        }

        private async Task NavigateByRoleAsync(UserAccount user)
        {
            try
            {
                string role = user.Role.ToLower().Trim();
                System.Diagnostics.Debug.WriteLine($"[NAVIGATION DEBUG] Navigating for role: '{role}'");
                
                if (role == "student")
                {
                    var studentDashboard = new StudentDashboard(user);
                    studentDashboard.Show();
                    System.Diagnostics.Debug.WriteLine($"[NAVIGATION DEBUG] Opened StudentDashboard for user: {user.Username}");
                }
                else if (role == "teacher")
                {
                    // Placeholder for TeacherDashboard
                    MessageBox.Show("Giao diện Giáo viên đang được phát triển.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (role == "admin")
                {
                    // Placeholder for AdminDashboard
                    MessageBox.Show("Giao diện Quản trị viên đang được phát triển.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (role == "coordinator")
                {
                    // Placeholder for CoordinatorDashboard
                    MessageBox.Show("Giao diện Điều phối viên đang được phát triển.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Không tìm thấy giao diện cho vai trò: '{role}'", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
                // Close the login window
                Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NAVIGATION DEBUG] Error: {ex.Message}");
                MessageBox.Show($"Lỗi khi chuyển hướng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterLink_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.Owner = this;
            if (registerWindow.ShowDialog() == true)
            {
                ShowStatus("Đăng ký thành công! Vui lòng đăng nhập.", Brushes.Green);
            }
        }

        private void ForgotPasswordLink_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var forgotPasswordWindow = new ForgotPasswordWindow();
            forgotPasswordWindow.Owner = this;
            forgotPasswordWindow.ShowDialog();
        }

        private void ShowStatus(string message, Brush color)
        {
            txtStatus.Text = message;
            txtStatus.Foreground = color;
        }

        // Test database connection and data - for debugging
        private async void BtnTestDatabase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ShowStatus("Đang kiểm tra kết nối cơ sở dữ liệu...", Brushes.Blue);
                
                var result = await _dbTestService.TestDatabaseConnection();
                ShowStatus("Kết nối cơ sở dữ liệu: " + (result.Contains("successful") ? "Thành công" : "Thất bại"), 
                    result.Contains("successful") ? Brushes.Green : Brushes.Red);
                
                // Also test searching for the user we're trying to login with
                if (!string.IsNullOrWhiteSpace(txtUsernameEmail.Text))
                {
                    var searchResult = await _dbTestService.SearchUser(txtUsernameEmail.Text.Trim());
                    ShowStatus($"Kết quả tìm kiếm người dùng:\n{searchResult}", 
                        searchResult.Contains("found") ? Brushes.Green : Brushes.Orange);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi kiểm tra cơ sở dữ liệu: {ex.Message}", Brushes.Red);
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _userService?.Dispose();
            _dbTestService?.Dispose();
            base.OnClosed(e);
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
