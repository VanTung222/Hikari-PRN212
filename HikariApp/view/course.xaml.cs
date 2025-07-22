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
using HikariDataAccess;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using HikariBusiness.Services;
using HikariApp.UserControls;

namespace HikariApp.view
{
    /// <summary>
    /// Interaction logic for Course.xaml
    /// </summary>
    public partial class Course : Window
    {
        private HikariContext _context;
        private CourseService _courseService;
        private CartService _cartService;
        private const string CURRENT_STUDENT_ID = "S001"; // Hardcoded for demo
        
        public Course()
        {
            InitializeComponent();
            _context = new HikariContext();
            _courseService = new CourseService();
            _cartService = new CartService();
            LoadCoursesFromDatabase();
        }
        
        private async void LoadCoursesFromDatabase()
        {
            await RefreshAllCourseCards();
        }
        
        // Refresh all course cards and their button states
        public async Task RefreshAllCourseCards()
        {
            try
            {
                // Show loading indicator
                ShowLoading(true);
                
                // Load courses from database using service
                var courses = await _courseService.GetAllActiveCoursesAsync();
                
                // Clear existing courses
                CoursePanel.Children.Clear();
                
                if (courses.Any())
                {
                    // Create course cards using UserControl
                    foreach (var course in courses)
                    {
                        await CreateCourseCard(course);
                    }
                    
                    // Update cart count in header
                    await UpdateCartCount();
                }
                else
                {
                    // Show empty state
                    ShowEmptyState();
                }
                
                ShowLoading(false);
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                
                // Show hardcoded courses as fallback
                ShowHardcodedCourses();
            }
        }
        
        private async Task CreateCourseCard(HikariDataAccess.Entities.Course course)
        {
            try
            {
                // Create CourseCard UserControl
                var courseCard = new CourseCard();
                courseCard.SetCourse(course);
                
                // Determine button state
                var isEnrolled = await _courseService.IsCourseEnrolledAsync(CURRENT_STUDENT_ID, course.CourseID);
                var isInCart = await _cartService.IsCourseInCartAsync(CURRENT_STUDENT_ID, course.CourseID);
                
                if (isEnrolled)
                {
                    courseCard.SetButtonState(CourseButtonState.ContinueLearning);
                }
                else if (isInCart)
                {
                    courseCard.SetButtonState(CourseButtonState.InCart);
                }
                else
                {
                    courseCard.SetButtonState(CourseButtonState.AddToCart);
                }
                
                // Subscribe to events
                courseCard.AddToCartClicked += CourseCard_AddToCartClicked;
                courseCard.ContinueLearningClicked += CourseCard_ContinueLearningClicked;
                
                // Add to panel
                CoursePanel.Children.Add(courseCard);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo course card: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        
        private async void CourseCard_AddToCartClicked(object sender, HikariDataAccess.Entities.Course course)
        {
            try
            {
                var success = await _cartService.AddToCartAsync(CURRENT_STUDENT_ID, course.CourseID);
                
                if (success)
                {
                    // Update button state
                    var courseCard = sender as CourseCard;
                    courseCard?.SetButtonState(CourseButtonState.InCart);
                    
                    // Update cart count
                    await UpdateCartCount();
                    
                    // Show notification
                    ShowNotification($"Đã thêm '{course.Title}' vào giỏ hàng!");
                }
                else
                {
                    MessageBox.Show("Khóa học đã có trong giỏ hàng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm vào giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void CourseCard_ContinueLearningClicked(object sender, HikariDataAccess.Entities.Course course)
        {
            try
            {
                var myCoursesWindow = new MyCourses();
                myCoursesWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task UpdateCartCount()
        {
            try
            {
                var count = await _cartService.GetCartCountAsync(CURRENT_STUDENT_ID);
                // Update cart count in UI if you have a cart count display
                // CartCountText.Text = count.ToString();
            }
            catch (Exception ex)
            {
                // Handle silently
            }
        }
        
        private void ShowNotification(string message)
        {
            try
            {
                // Simple notification - you can enhance this
                MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                // Handle silently
            }
        }
        
        private void ShowEmptyState()
        {
            var emptyStack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(50)
            };
            
            emptyStack.Children.Add(new TextBlock
            {
                Text = "📚",
                FontSize = 60,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 20)
            });
            
            emptyStack.Children.Add(new TextBlock
            {
                Text = "Chưa có khóa học nào",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            });
            
            emptyStack.Children.Add(new TextBlock
            {
                Text = "Vui lòng thêm dữ liệu khóa học vào database",
                FontSize = 14,
                Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                HorizontalAlignment = HorizontalAlignment.Center
            });
            
            CoursePanel.Children.Add(emptyStack);
        }
        
        private void ShowHardcodedCourses()
        {
            // Keep the existing hardcoded courses as fallback
            // This will be shown if database connection fails
        }
        
        private void ShowLoading(bool show)
        {
            // Simple loading implementation
            if (show)
            {
                CoursePanel.Children.Clear();
                var loadingText = new TextBlock
                {
                    Text = "Đang tải khóa học...",
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(50)
                };
                CoursePanel.Children.Add(loadingText);
            }
        }
        
        private System.Windows.Media.Brush GetRandomCourseColor()
        {
            var colors = new string[]
            {
                "#6C5CE7", "#00BCD4", "#E91E63", "#4CAF50", "#FF9800", "#9C27B0"
            };
            var random = new Random();
            var color = colors[random.Next(colors.Length)];
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }
        
        private string GetCourseIcon(string title)
        {
            if (title.ToLower().Contains("c#") || title.ToLower().Contains("programming"))
                return "💻";
            if (title.ToLower().Contains("web") || title.ToLower().Contains("asp.net"))
                return "🌐";
            if (title.ToLower().Contains("data") || title.ToLower().Contains("python"))
                return "📊";
            if (title.ToLower().Contains("mobile") || title.ToLower().Contains("app"))
                return "📱";
            if (title.ToLower().Contains("security") || title.ToLower().Contains("an ninh"))
                return "🔒";
            return "📚";
        }
        
        private string GetCourseCategory(string title)
        {
            if (title.ToLower().Contains("c#") || title.ToLower().Contains("programming"))
                return "Programming";
            if (title.ToLower().Contains("web") || title.ToLower().Contains("asp.net"))
                return "Web Development";
            if (title.ToLower().Contains("data") || title.ToLower().Contains("python"))
                return "Data Science";
            if (title.ToLower().Contains("mobile") || title.ToLower().Contains("app"))
                return "Mobile Development";
            if (title.ToLower().Contains("security") || title.ToLower().Contains("an ninh"))
                return "Cybersecurity";
            return "General";
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
        
        private void MyCoursesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var myCoursesWindow = new MyCourses();
                myCoursesWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở khóa học của tôi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
}
