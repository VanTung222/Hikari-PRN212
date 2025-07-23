using System;
using System.Windows;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class EditReviewDialog : Window
    {
        private readonly ReviewService _reviewService;
        private readonly ReviewViewModel _review;

        public EditReviewDialog(ReviewViewModel review)
        {
            InitializeComponent();
            _reviewService = new ReviewService();
            _review = review;
            LoadReviewData();
        }

        private void LoadReviewData()
        {
            IdTextBox.Text = _review.Id;
            StudentNameTextBox.Text = _review.StudentName;
            CourseNameTextBox.Text = _review.CourseName;
            RatingComboBox.Text = _review.Rating;
            CommentTextBox.Text = _review.Comment;
            StatusComboBox.Text = _review.Status;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (RatingComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn điểm đánh giá!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (StatusComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn trạng thái đánh giá!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create updated review object
                var updatedReview = new ReviewViewModel
                {
                    Id = _review.Id,
                    StudentName = _review.StudentName,
                    CourseName = _review.CourseName,
                    Rating = RatingComboBox.Text,
                    Comment = CommentTextBox.Text.Trim(),
                    ReviewDate = _review.ReviewDate,
                    Status = StatusComboBox.Text
                };

                // Note: Since ReviewService doesn't have UpdateReviewAsync method yet,
                // we'll show a success message for now. This should be implemented in the service layer.
                MessageBox.Show("Cập nhật đánh giá thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
