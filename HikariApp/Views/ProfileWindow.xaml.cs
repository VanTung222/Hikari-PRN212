using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using HikariBusiness.Services;
using HikariDataAccess.Entities;

namespace HikariApp.Views
{
    public partial class ProfileWindow : Window
    {
        private readonly UserService _userService;
        private UserAccount _currentUser;
        private string _profilePicturePath;

        public ProfileWindow(UserAccount user)
        {
            InitializeComponent();
            _userService = new UserService();
            _currentUser = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            try
            {
                // Load basic user information
                txtUsername.Text = _currentUser.Username;
                txtFullName.Text = _currentUser.FullName;
                txtEmail.Text = _currentUser.Email;
                txtRole.Text = _currentUser.Role == "student" ? "Học Sinh" : "Giáo Viên";
                txtPhone.Text = _currentUser.Phone ?? "";
                
                if (_currentUser.BirthDate.HasValue)
                {
                    dpBirthDate.SelectedDate = _currentUser.BirthDate.Value.ToDateTime(TimeOnly.MinValue);
                }

                // Load profile picture
                if (!string.IsNullOrEmpty(_currentUser.ProfilePicture) && File.Exists(_currentUser.ProfilePicture))
                {
                    LoadProfilePicture(_currentUser.ProfilePicture);
                }

                // Show teacher-specific fields if user is a teacher
                if (_currentUser.Role.ToLower() == "teacher" && _currentUser.Teacher != null)
                {
                    pnlTeacherInfo.Visibility = Visibility.Visible;
                    txtSpecialization.Text = _currentUser.Teacher.Specialization ?? "";
                    txtExperienceYears.Text = _currentUser.Teacher.ExperienceYears?.ToString() ?? "";
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi khi tải thông tin: {ex.Message}", Brushes.Red);
            }
        }

        private void LoadProfilePicture(string imagePath)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imagePath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imgProfilePicture.Source = bitmap;
                _profilePicturePath = imagePath;
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi khi tải ảnh đại diện: {ex.Message}", Brushes.Red);
            }
        }

        private void BtnChangeAvatar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Chọn ảnh đại diện",
                    Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp",
                    FilterIndex = 1
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    // Create profile pictures directory if it doesn't exist
                    var profilePicturesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProfilePictures");
                    Directory.CreateDirectory(profilePicturesDir);

                    // Generate unique filename
                    var extension = Path.GetExtension(openFileDialog.FileName);
                    var fileName = $"{_currentUser.UserId}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var destinationPath = Path.Combine(profilePicturesDir, fileName);

                    // Copy file to profile pictures directory
                    File.Copy(openFileDialog.FileName, destinationPath, true);

                    // Load the new profile picture
                    LoadProfilePicture(destinationPath);
                    
                    ShowStatus("Ảnh đại diện đã được thay đổi. Nhấn 'Lưu Thông Tin' để cập nhật.", Brushes.Green);
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi khi thay đổi ảnh đại diện: {ex.Message}", Brushes.Red);
            }
        }

        private void BtnRemoveAvatar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Thay thế bằng hình ảnh mặc định thay vì đặt null
                var defaultAvatarUri = new Uri("pack://application:,,,/assest/learning.jpg", UriKind.Absolute);
                imgProfilePicture.Source = new BitmapImage(defaultAvatarUri);
                _profilePicturePath = null;
                ShowStatus("Ảnh đại diện đã được xóa. Nhấn 'Lưu Thông Tin' để cập nhật.", Brushes.Orange);
            }
            catch (Exception ex)
            {
                ShowStatus($"Lỗi khi xóa ảnh đại diện: {ex.Message}", Brushes.Red);
            }
        }

        private async void BtnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtFullName.Text))
                {
                    ShowStatus("Vui lòng nhập họ và tên!", Brushes.Red);
                    txtFullName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtEmail.Text))
                {
                    ShowStatus("Vui lòng nhập email!", Brushes.Red);
                    txtEmail.Focus();
                    return;
                }

                // Validate email format
                if (!IsValidEmail(txtEmail.Text))
                {
                    ShowStatus("Định dạng email không hợp lệ!", Brushes.Red);
                    txtEmail.Focus();
                    return;
                }

                btnSaveProfile.IsEnabled = false;
                ShowStatus("Đang cập nhật thông tin...", Brushes.Blue);

                // Update profile
                var birthDate = dpBirthDate.SelectedDate.HasValue ? 
                    DateOnly.FromDateTime(dpBirthDate.SelectedDate.Value) : (DateOnly?)null;

                var result = await _userService.UpdateProfileAsync(
                    _currentUser.UserId,
                    txtFullName.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtPhone.Text.Trim(),
                    birthDate,
                    _profilePicturePath
                );

                if (result.Success)
                {
                    // Update teacher specialization if applicable
                    if (_currentUser.Role.ToLower() == "teacher" && pnlTeacherInfo.Visibility == Visibility.Visible)
                    {
                        if (int.TryParse(txtExperienceYears.Text, out int experienceYears))
                        {
                            var teacherResult = await _userService.UpdateTeacherSpecializationAsync(
                                _currentUser.UserId,
                                txtSpecialization.Text.Trim(),
                                experienceYears
                            );

                            if (!teacherResult.Success)
                            {
                                ShowStatus($"Cập nhật thông tin cá nhân thành công, nhưng có lỗi khi cập nhật thông tin giáo viên: {teacherResult.Message}", Brushes.Orange);
                                return;
                            }
                        }
                    }

                    // Refresh user data
                    _currentUser = await _userService.GetUserByIdAsync(_currentUser.UserId);
                    ShowStatus("Cập nhật thông tin thành công!", Brushes.Green);
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
                btnSaveProfile.IsEnabled = true;
            }
        }

        private async void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(txtCurrentPassword.Password))
                {
                    ShowStatus("Vui lòng nhập mật khẩu hiện tại!", Brushes.Red);
                    txtCurrentPassword.Focus();
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

                btnChangePassword.IsEnabled = false;
                ShowStatus("Đang đổi mật khẩu...", Brushes.Blue);

                // Verify current password
                var isCurrentPasswordValid = await _userService.VerifyCurrentPasswordAsync(_currentUser.UserId, txtCurrentPassword.Password);
                if (!isCurrentPasswordValid)
                {
                    ShowStatus("Mật khẩu hiện tại không đúng!", Brushes.Red);
                    txtCurrentPassword.Focus();
                    return;
                }

                // Update password
                var result = await _userService.UpdatePasswordAsync(_currentUser.UserId, txtNewPassword.Password);
                if (result.Success)
                {
                    ShowStatus("Đổi mật khẩu thành công!", Brushes.Green);
                    
                    // Clear password fields
                    txtCurrentPassword.Clear();
                    txtNewPassword.Clear();
                    txtConfirmNewPassword.Clear();
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
                btnChangePassword.IsEnabled = true;
            }
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
