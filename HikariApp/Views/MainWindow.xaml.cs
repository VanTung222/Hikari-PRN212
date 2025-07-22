using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using HikariDataAccess.Entities;

namespace HikariApp.Views
{
    public partial class MainWindow : Window
    {
        private UserAccount _currentUser;

        public MainWindow(UserAccount user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserInfo();

            // Hiển thị nút quay lại nếu là học viên
            if (_currentUser.Role != null && _currentUser.Role.ToLower().Trim() == "student")
            {
                btnBackToStudent.Visibility = Visibility.Visible;
            }
            else
            {
                btnBackToStudent.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadUserInfo()
        {
            try
            {
                // Update header user info
                txtWelcome.Text = $"Chào {_currentUser.FullName}!";
                
                // Hiển thị vai trò người dùng dựa trên giá trị từ database
                string userRole;
                switch (_currentUser.Role.ToLower().Trim())
                {
                    case "student":
                        userRole = "Học Sinh";
                        break;
                    case "teacher":
                        userRole = "Giáo Viên";
                        break;
                    case "admin":
                        userRole = "Quản Trị Viên";
                        break;
                    case "coordinator":
                        userRole = "Điều Phối Viên";
                        break;
                    default:
                        userRole = _currentUser.Role; // Giữ nguyên giá trị nếu không khớp
                        break;
                }
                txtUserRole.Text = userRole;

                // Update main welcome message
                txtMainWelcome.Text = $"Chào mừng {_currentUser.FullName} đến với Hikari Learning!";
                txtUserInfo.Text = $"Bạn đang đăng nhập với vai trò {userRole}";

                // Load user avatar if exists
                if (!string.IsNullOrEmpty(_currentUser.ProfilePicture) && File.Exists(_currentUser.ProfilePicture))
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(_currentUser.ProfilePicture, UriKind.Absolute);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                        imgUserAvatar.Source = bitmap;
                    }
                    catch (Exception ex)
                    {
                        // If avatar loading fails, load default avatar
                        LoadDefaultAvatar();
                        System.Diagnostics.Debug.WriteLine($"Failed to load profile image: {ex.Message}");
                    }
                }
                else
                {
                    // No profile picture, load default avatar
                    LoadDefaultAvatar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin người dùng: {ex.Message}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var profileWindow = new ProfileWindow(_currentUser);
                profileWindow.Owner = this;
                if (profileWindow.ShowDialog() == true)
                {
                    // Refresh user info if profile was updated
                    LoadUserInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở cửa sổ thông tin cá nhân: {ex.Message}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var profileWindow = new ProfileWindow(_currentUser);
                profileWindow.Owner = this;
                profileWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", 
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Show confirmation dialog
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", 
                    "Xác nhận đăng xuất", 
                    MessageBoxButton.YesNo, 
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Close the main window and return to login
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đăng xuất: {ex.Message}", 
                    "Lỗi", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
            }
        }

        private void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            var helpMessage = @"HƯỚNG DẪN SỬ DỤNG HIKARI LEARNING

1. QUẢN LÝ THÔNG TIN CÁ NHÂN:
   - Nhấn 'Thông Tin Cá Nhân' để cập nhật thông tin
   - Có thể thay đổi ảnh đại diện, thông tin liên lạc
   - Đổi mật khẩu trong phần bảo mật

2. BẢO MẬT TÀI KHOẢN:
   - Thường xuyên đổi mật khẩu
   - Sử dụng mật khẩu mạnh (ít nhất 6 ký tự)
   - Không chia sẻ thông tin đăng nhập

3. QUÊN MẬT KHẨU:
   - Sử dụng chức năng 'Quên mật khẩu' ở màn hình đăng nhập
   - Nhập email để nhận mã khôi phục
   - Làm theo hướng dẫn để đặt lại mật khẩu

4. HỖ TRỢ:
   - Liên hệ admin nếu gặp vấn đề kỹ thuật
   - Email: support@hikarilearning.com
   - Hotline: 1900-xxxx";

            MessageBox.Show(helpMessage, "Hướng Dẫn Sử Dụng", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnBackToStudent_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại StudentDashboard
            var studentDashboard = new StudentDashboard(_currentUser);
            studentDashboard.Show();
            this.Close();
        }

        // Helper method to load default avatar
        private void LoadDefaultAvatar()
        {
            try
            {
                var defaultAvatarUri = new Uri("pack://application:,,,/assest/learning.jpg", UriKind.Absolute);
                imgUserAvatar.Source = new BitmapImage(defaultAvatarUri);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load default avatar: {ex.Message}");
                // Just continue without avatar if even default fails
            }
        }


    }
}
