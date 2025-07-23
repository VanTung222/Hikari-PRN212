using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class AddDiscountDialog : Window
    {
        private readonly DiscountService _discountService;
        public bool IsSuccess { get; private set; } = false;

        public AddDiscountDialog()
        {
            InitializeComponent();
            _discountService = new DiscountService();
            
            // Set default dates
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today.AddDays(30);
            
            LoadCoursesAsync();
        }

        private async void LoadCoursesAsync()
        {
            try
            {
                var courses = await _discountService.GetCoursesForDiscountAsync();
                CourseComboBox.ItemsSource = courses;
                if (courses.Any())
                {
                    CourseComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(CodeTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập mã giảm giá", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CodeTextBox.Focus();
                    return;
                }

                if (CourseComboBox.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn khóa học", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    CourseComboBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(DiscountPercentTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập phần trăm giảm giá", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DiscountPercentTextBox.Focus();
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

                // Validate discount percent format
                if (!int.TryParse(DiscountPercentTextBox.Text, out int discountPercent) || discountPercent <= 0 || discountPercent > 100)
                {
                    MessageBox.Show("Phần trăm giảm giá phải là số nguyên từ 1 đến 100", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    DiscountPercentTextBox.Focus();
                    return;
                }

                // Validate date range
                if (EndDatePicker.SelectedDate <= StartDatePicker.SelectedDate)
                {
                    MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    EndDatePicker.Focus();
                    return;
                }

                // Validate start date is not in the past
                if (StartDatePicker.SelectedDate < DateTime.Today)
                {
                    MessageBox.Show("Ngày bắt đầu không thể là ngày trong quá khứ", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    StartDatePicker.Focus();
                    return;
                }

                // Disable save button to prevent double submission
                SaveButton.IsEnabled = false;
                SaveButton.Content = "Đang lưu...";

                // Add discount
                await _discountService.AddDiscountAsync(
                    CodeTextBox.Text.Trim().ToUpper(),
                    CourseComboBox.SelectedValue.ToString(),
                    discountPercent,
                    StartDatePicker.SelectedDate.Value,
                    EndDatePicker.SelectedDate.Value
                );

                MessageBox.Show("Thêm mã giảm giá thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
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
            _discountService?.Dispose();
            base.OnClosed(e);
        }
    }
}
