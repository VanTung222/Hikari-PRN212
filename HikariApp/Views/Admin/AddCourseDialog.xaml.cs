using System;
using System.Globalization;
using System.Windows;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class AddCourseDialog : Window
    {
        private readonly CourseService _courseService;
        public bool IsSuccess { get; private set; } = false;

        public AddCourseDialog()
        {
            InitializeComponent();
            _courseService = new CourseService();
            
            // Set default dates
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(30);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên khóa học", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    TitleTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập mô tả khóa học", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DescriptionTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(FeeTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập học phí", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    FeeTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(DurationTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập thời lượng", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DurationTextBox.Focus();
                    return;
                }

                if (!StartDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày bắt đầu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    StartDatePicker.Focus();
                    return;
                }

                if (!EndDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Vui lòng chọn ngày kết thúc", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EndDatePicker.Focus();
                    return;
                }

                // Validate fee format
                if (!decimal.TryParse(FeeTextBox.Text.Replace(",", "").Replace(".", ""), out decimal fee))
                {
                    MessageBox.Show("Học phí không hợp lệ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    FeeTextBox.Focus();
                    return;
                }

                // Validate duration format
                if (!int.TryParse(DurationTextBox.Text, out int duration) || duration <= 0)
                {
                    MessageBox.Show("Thời lượng phải là số nguyên dương", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DurationTextBox.Focus();
                    return;
                }

                // Validate date range
                if (EndDatePicker.SelectedDate <= StartDatePicker.SelectedDate)
                {
                    MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EndDatePicker.Focus();
                    return;
                }

                // Disable save button to prevent double submission
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Đang lưu...";

                // Add course
                await _courseService.AddCourseAsync(
                    TitleTextBox.Text.Trim(),
                    DescriptionTextBox.Text.Trim(),
                    fee,
                    duration,
                    StartDatePicker.SelectedDate.Value,
                    EndDatePicker.SelectedDate.Value
                );

                MessageBox.Show("Thêm khóa học thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                IsSuccess = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveButton.IsEnabled = true;
                SaveButton.Content = "Lưu";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _courseService?.Dispose();
            base.OnClosed(e);
        }
    }
}
