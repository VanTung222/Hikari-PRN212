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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private readonly DashboardService _dashboardService;

        public DashboardPage()
        {
            InitializeComponent();
            _dashboardService = new DashboardService();
            LoadDashboardDataAsync();
            this.Unloaded += DashboardPage_Unloaded;
        }

        private async void LoadDashboardDataAsync()
        {
            try
            {
                // Load dashboard statistics
                var statistics = await _dashboardService.GetDashboardStatisticsAsync();

                // Update statistics cards with real data
                UserCountText.Text = statistics.TotalUsers.ToString();
                CourseCountText.Text = statistics.TotalCourses.ToString();
                PaymentCountText.Text = statistics.TotalPayments.ToString();
                ReviewCountText.Text = statistics.TotalReviews.ToString();

                // Load recent courses and populate the list
                var recentCourses = await _dashboardService.GetRecentCoursesAsync(5);
                PopulateRecentCourses(recentCourses);

                // Update donut chart center total
                var totalItems = statistics.TotalUsers + statistics.TotalCourses + statistics.TotalPayments + statistics.TotalReviews;
                // Note: You would need to add x:Name to the center text in XAML to update it dynamically

                // Load chart data
                var monthlyRevenue = await _dashboardService.GetMonthlyRevenueDataAsync(6);
                var userGrowth = await _dashboardService.GetUserGrowthDataAsync(6);

                // You can bind these to UI elements or use them for charts
                // For now, we'll show a message with the loaded data count
                MessageBox.Show($"Dashboard loaded successfully!\n" +
                              $"Total Users: {statistics.TotalUsers}\n" +
                              $"Total Courses: {statistics.TotalCourses}\n" +
                              $"Total Payments: {statistics.TotalPayments}\n" +
                              $"Total Reviews: {statistics.TotalReviews}\n" +
                              $"Recent Courses: {recentCourses.Count}",
                              "Dashboard Data", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu dashboard: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PopulateRecentCourses(List<RecentCourseViewModel> recentCourses)
        {
            RecentCoursesPanel.Children.Clear();
            
            foreach (var course in recentCourses.Take(5))
            {
                var border = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0xEE, 0xEE, 0xEE)),
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(0, 12, 0, 12)
                };
                
                var textBlock = new TextBlock
                {
                    Text = $"• {course.Title}",
                    FontSize = 14,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66))
                };
                
                border.Child = textBlock;
                RecentCoursesPanel.Children.Add(border);
            }
        }

        private void DashboardPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _dashboardService?.Dispose();
        }
    }
}
