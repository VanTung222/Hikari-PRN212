using System.Windows;
using System.Windows.Media;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class ReviewDetailsDialog : Window
    {
        public ReviewDetailsDialog(ReviewViewModel review)
        {
            InitializeComponent();
            LoadReviewDetails(review);
        }

        private void LoadReviewDetails(ReviewViewModel review)
        {
            IdTextBlock.Text = review.Id;
            StudentNameTextBlock.Text = review.StudentName;
            CourseNameTextBlock.Text = review.CourseName;
            RatingTextBlock.Text = review.Rating;
            ReviewDateTextBlock.Text = review.ReviewDate;
            CommentTextBlock.Text = string.IsNullOrEmpty(review.Comment) ? "Không có bình luận" : review.Comment;

            // Set status with color
            StatusTextBlock.Text = review.Status;
            StatusBorder.Background = GetStatusColor(review.Status);
        }

        private Brush GetStatusColor(string status)
        {
            return status switch
            {
                "Đã duyệt" => new SolidColorBrush(Color.FromRgb(92, 184, 92)), // Green
                "Chờ duyệt" => new SolidColorBrush(Color.FromRgb(255, 193, 7)), // Yellow
                "Từ chối" => new SolidColorBrush(Color.FromRgb(220, 53, 69)), // Red
                _ => new SolidColorBrush(Color.FromRgb(108, 117, 125)) // Gray
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
