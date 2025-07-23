using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HikariBusiness.Services;
using HikariBusiness.ViewModels;

namespace HikariApp.view
{
    /// <summary>
    /// Interaction logic for MyCourses.xaml
    /// </summary>
    public partial class MyCourses : Window
    {
        private readonly EnrollmentService _enrollmentService;
        private readonly string _currentStudentId = "S001"; // Gán cứng student ID

        public MyCourses()
        {
            InitializeComponent();
            _enrollmentService = new EnrollmentService();
            LoadMyCoursesAsync();
        }
        
        private void CoursesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var coursesWindow = new Course();
                coursesWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cartWindow = new Cart();
                cartWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Load dữ liệu khóa học đã đăng ký
        private async void LoadMyCoursesAsync()
        {
            try
            {
                // Hiển thị loading
                LoadingOverlay.Visibility = Visibility.Visible;
                MyCoursesPanel.Children.Clear();

                // Lấy dữ liệu từ database
                var enrolledCourses = await _enrollmentService.GetEnrolledCoursesWithProgressAsync(_currentStudentId);

                // Ẩn loading
                LoadingOverlay.Visibility = Visibility.Collapsed;

                if (enrolledCourses.Count == 0)
                {
                    ShowEmptyState();
                    return;
                }

                // Cập nhật thống kê
                UpdateCourseStats(enrolledCourses);

                // Hiển thị khóa học
                DisplayEnrolledCourses(enrolledCourses);
            }
            catch (Exception ex)
            {
                LoadingOverlay.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Lỗi khi tải khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowEmptyState();
            }
        }

        // Hiển thị thống kê khóa học
        private void UpdateCourseStats(List<EnrolledCourseViewModel> courses)
        {
            var totalCourses = courses.Count;
            var completedCourses = courses.Count(c => c.IsCompleted);
            var inProgressCourses = totalCourses - completedCourses;

            // Cập nhật text trong XAML
            if (FindName("TotalCoursesText") is TextBlock totalText)
                totalText.Text = $"Tổng số {totalCourses} khóa học đã đăng ký";

            if (FindName("CompletedCoursesText") is TextBlock completedText)
            {
                if (completedCourses > 0)
                    completedText.Text = $"✓ {completedCourses} khóa học đã hoàn thành";
                else
                    completedText.Text = "✓ Đang học tích cực";
            }

            if (FindName("InProgressCoursesText") is TextBlock inProgressText)
            {
                if (inProgressCourses > 0)
                    inProgressText.Text = $"⏱️ {inProgressCourses} khóa đang tiến hành";
                else
                    inProgressText.Text = "⏱️ Tất cả khóa học đã hoàn thành";
            }
        }

        // Hiển thị danh sách khóa học đã đăng ký
        private void DisplayEnrolledCourses(List<EnrolledCourseViewModel> courses)
        {
            foreach (var course in courses)
            {
                var courseCard = CreateCourseCard(course);
                MyCoursesPanel.Children.Add(courseCard);
            }
        }

        // Tạo card cho khóa học đã đăng ký
        private Border CreateCourseCard(EnrolledCourseViewModel course)
        {
            var card = new Border
            {
                Style = (Style)FindResource("MyCourseCardStyle"),
                Margin = new Thickness(10),
                Width = 320,
                Height = 200
            };

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Header với icon và title
            var headerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(15, 15, 15, 10)
            };

            var iconText = new TextBlock
            {
                Text = course.CourseIcon,
                FontSize = 24,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var titleText = new TextBlock
            {
                Text = course.Title,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap
            };

            headerPanel.Children.Add(iconText);
            headerPanel.Children.Add(titleText);
            Grid.SetRow(headerPanel, 0);

            // Description
            var descText = new TextBlock
            {
                Text = course.Description,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                Margin = new Thickness(15, 0, 15, 10),
                TextWrapping = TextWrapping.Wrap
            };
            Grid.SetRow(descText, 1);

            // Progress section
            var progressPanel = new StackPanel
            {
                Margin = new Thickness(15, 0, 15, 10)
            };

            var progressText = new TextBlock
            {
                Text = $"{course.ProgressText} ({course.FormattedProgress})",
                FontSize = 12,
                Foreground = Brushes.White,
                Margin = new Thickness(0, 0, 0, 5)
            };

            var progressBar = new ProgressBar
            {
                Value = course.ProgressPercentage,
                Height = 6,
                Style = (Style)FindResource("ModernProgressBar")
            };

            progressPanel.Children.Add(progressText);
            progressPanel.Children.Add(progressBar);
            Grid.SetRow(progressPanel, 2);

            // Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(15, 10, 15, 15),
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var continueButton = new Button
            {
                Content = course.IsCompleted ? "🏆 Đã hoàn thành" : "📚 Tiếp tục học",
                Style = (Style)FindResource("OrangeButtonStyle"),
                Margin = new Thickness(0, 0, 10, 0),
                IsEnabled = !course.IsCompleted,
                Tag = course.CourseID
            };
            continueButton.Click += ContinueButton_Click;

            var progressButton = new Button
            {
                Content = course.IsCompleted ? "🎓" : "📊",
                Width = 35,
                Height = 35,
                Background = new SolidColorBrush(Color.FromRgb(255, 215, 0)),
                Foreground = Brushes.Black,
                BorderThickness = new Thickness(0),
                FontSize = 14,
                Tag = course.CourseID
            };
            progressButton.Click += ProgressButton_Click;

            buttonPanel.Children.Add(continueButton);
            buttonPanel.Children.Add(progressButton);
            Grid.SetRow(buttonPanel, 3);

            // Set background color
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(course.BackgroundColor);
                card.Background = new SolidColorBrush(color);
            }
            catch
            {
                card.Background = new SolidColorBrush(Color.FromRgb(108, 92, 231));
            }

            grid.Children.Add(headerPanel);
            grid.Children.Add(descText);
            grid.Children.Add(progressPanel);
            grid.Children.Add(buttonPanel);

            card.Child = grid;
            return card;
        }

        // Hiển thị trạng thái trống
        private void ShowEmptyState()
        {
            MyCoursesPanel.Children.Clear();

            var emptyPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(20)
            };

            var emptyIcon = new TextBlock
            {
                Text = "📚",
                FontSize = 48,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var emptyText = new TextBlock
            {
                Text = "Bạn chưa đăng ký khóa học nào",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            };

            var browseButton = new Button
            {
                Content = "Khám phá khóa học",
                Style = (Style)FindResource("OrangeButtonStyle"),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            browseButton.Click += CoursesButton_Click;

            emptyPanel.Children.Add(emptyIcon);
            emptyPanel.Children.Add(emptyText);
            emptyPanel.Children.Add(browseButton);

            MyCoursesPanel.Children.Add(emptyPanel);
        }

        // Event handlers
        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string courseId)
            {
                MessageBox.Show($"Tiếp tục học khóa học: {courseId}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // TODO: Navigate to learning page
            }
        }

        private void ProgressButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string courseId)
            {
                MessageBox.Show($"Xem tiến độ khóa học: {courseId}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                // TODO: Show progress details
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadMyCoursesAsync();
        }
        
        // Public method to refresh from other windows
        public void RefreshMyCourses()
        {
            LoadMyCoursesAsync();
        }
    }
}
