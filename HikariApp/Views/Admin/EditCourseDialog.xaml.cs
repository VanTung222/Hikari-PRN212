using System;
using System.Globalization;
using System.Windows;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class EditCourseDialog : Window
    {
        private readonly CourseService _courseService;
        private readonly CourseViewModel _course;

        public EditCourseDialog(CourseViewModel course)
        {
            InitializeComponent();
            _courseService = new CourseService();
            _course = course;
            LoadCourseData();
        }

        private void LoadCourseData()
        {
            IdTextBox.Text = _course.Id;
            TitleTextBox.Text = _course.Title;
            DescriptionTextBox.Text = _course.Description;
            
            // Parse fee (remove "VNĐ" and format)
            if (decimal.TryParse(_course.Fee.Replace("VNĐ", "").Replace(",", "").Trim(), out decimal fee))
            {
                FeeTextBox.Text = fee.ToString();
            }
            
            // Parse duration (remove "giờ")
            if (int.TryParse(_course.Duration.Replace("giờ", "").Trim(), out int duration))
            {
                DurationTextBox.Text = duration.ToString();
            }
            
            // Parse dates
            if (DateTime.TryParseExact(_course.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                StartDatePicker.SelectedDate = startDate;
            }
            
            if (DateTime.TryParseExact(_course.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                EndDatePicker.SelectedDate = endDate;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên khóa học!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(FeeTextBox.Text, out decimal fee) || fee < 0)
                {
                    MessageBox.Show("Vui lòng nhập học phí hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(DurationTextBox.Text, out int duration) || duration <= 0)
                {
                    MessageBox.Show("Vui lòng nhập thời lượng hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!StartDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!EndDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (EndDatePicker.SelectedDate <= StartDatePicker.SelectedDate)
                {
                    MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create updated course object
                var updatedCourse = new CourseViewModel
                {
                    Id = _course.Id,
                    Title = TitleTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    Fee = fee.ToString("N0") + " VNĐ",
                    Duration = duration + " giờ",
                    StartDate = StartDatePicker.SelectedDate.Value.ToString("dd/MM/yyyy"),
                    EndDate = EndDatePicker.SelectedDate.Value.ToString("dd/MM/yyyy"),
                    Status = _course.Status,
                    EnrollmentCount = _course.EnrollmentCount,
                    ReviewCount = _course.ReviewCount
                };

                // Update course
                bool success = await _courseService.UpdateCourseAsync(
    _course.Id,
    TitleTextBox.Text.Trim(),
    DescriptionTextBox.Text.Trim(),
    fee,
    duration,
    StartDatePicker.SelectedDate.Value,
    EndDatePicker.SelectedDate.Value
);
                

                if (success)
                {
                    MessageBox.Show("Cập nhật khóa học thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật khóa học!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
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
