using System.Windows;
using System.Windows.Media;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class CourseDetailsDialog : Window
    {
        public CourseDetailsDialog(CourseViewModel course)
        {
            InitializeComponent();
            LoadCourseDetails(course);
        }

        private void LoadCourseDetails(CourseViewModel course)
        {
            IdTextBlock.Text = course.Id;
            TitleTextBlock.Text = course.Title;
            DescriptionTextBlock.Text = string.IsNullOrEmpty(course.Description) ? "Chưa có mô tả" : course.Description;
            FeeTextBlock.Text = course.Fee;
            DurationTextBlock.Text = course.Duration;
            StartDateTextBlock.Text = course.StartDate;
            EndDateTextBlock.Text = course.EndDate;
            EnrollmentCountTextBlock.Text = course.EnrollmentCount.ToString();
            ReviewCountTextBlock.Text = course.ReviewCount.ToString();

            // Set status with color
            StatusTextBlock.Text = course.Status;
            StatusBorder.Background = GetStatusColor(course.Status);
        }

        private Brush GetStatusColor(string status)
        {
            return status switch
            {
                "Hoạt động" => new SolidColorBrush(Color.FromRgb(92, 184, 92)), // Green
                "Không hoạt động" => new SolidColorBrush(Color.FromRgb(220, 53, 69)), // Red
                _ => new SolidColorBrush(Color.FromRgb(108, 117, 125)) // Gray
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
