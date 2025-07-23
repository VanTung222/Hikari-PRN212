using System;
using System.Windows;
using System.Windows.Media;
using HikariDataAccess.Entities;
using System.Collections.Generic; // Added for FindVisualChildren

namespace HikariApp.Views
{
    public partial class StudentDashboard : Window
    {
        private UserAccount _currentUser;

        public StudentDashboard(UserAccount user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (_currentUser != null)
            {
                // Update welcome message with user's name
                txtWelcome.Text = $"Chào mừng trở lại, {(_currentUser.FullName.Split(' ').Length > 0 ? _currentUser.FullName.Split(' ')[0] : _currentUser.Username)}!";
                
                // Update username in the header
                txtUsername.Text = _currentUser.Username;

                // Load profile picture if available
                if (!string.IsNullOrEmpty(_currentUser.ProfilePicture))
                {
                    try
                    {
                        var profileImage = new System.Windows.Media.Imaging.BitmapImage(new Uri(_currentUser.ProfilePicture));
                        var brush = new ImageBrush(profileImage);
                        
                        // Find the Ellipse inside the profile button directly
                        foreach (var child in FindVisualChildren<System.Windows.Shapes.Ellipse>(btnProfile))
                        {
                            child.Fill = brush;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load profile image: {ex.Message}");
                        // Use default image on error - already set in XAML
                    }
                }
            }
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            // Open main window instead of profile window
            var mainWindow = new MainWindow(_currentUser);
            mainWindow.Show();
            this.Close();
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
                    // Set DialogResult to true to indicate logout
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

        private void BtnMyCourses_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to My Courses view or highlight the section if we're already there
            // Implement as needed
        }

        private void BtnLessons_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Lessons view
            // Implement as needed
        }

        private void BtnExercises_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Exercises view
            // Implement as needed
        }

        private void BtnDictionary_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Dictionary view
            var dictionaryView = new DictionaryView();
            dictionaryView.Show();
        }

        private void BtnForum_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to Forum view
            // Implement as needed
        }

        private void BtnSupport_Click(object sender, RoutedEventArgs e)
        {
            // Open support dialog or contact form
            // Implement as needed
            MessageBox.Show("Tính năng hỗ trợ đang được phát triển. Vui lòng liên hệ qua email: support@hikarijapan.com", 
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnNotifications_Click(object sender, RoutedEventArgs e)
        {
            // Show notifications panel or dialog
            // Implement as needed
            MessageBox.Show("Tính năng thông báo đang được phát triển.", 
                "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Helper method to find visual children of a specific type
        private IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
} 