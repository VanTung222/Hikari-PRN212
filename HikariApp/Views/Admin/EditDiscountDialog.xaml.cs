using System;
using System.Globalization;
using System.Windows;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class EditDiscountDialog : Window
    {
        private readonly DiscountService _discountService;
        private readonly DiscountViewModel _discount;

        public EditDiscountDialog(DiscountViewModel discount)
        {
            InitializeComponent();
            _discountService = new DiscountService();
            _discount = discount;
            LoadDiscountData();
        }

        private void LoadDiscountData()
        {
            IdTextBox.Text = _discount.Id;
            CodeTextBox.Text = _discount.Code;
            CourseNameTextBox.Text = _discount.CourseName;
            TypeComboBox.Text = _discount.Type;
            DiscountPercentTextBox.Text = _discount.DiscountPercent.Replace("%", "").Trim();
            
            // Parse dates
            if (DateTime.TryParseExact(_discount.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                StartDatePicker.SelectedDate = startDate;
            }
            
            if (DateTime.TryParseExact(_discount.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                EndDatePicker.SelectedDate = endDate;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(CodeTextBox.Text))
                {
                    MessageBox.Show("Vui lòng nhập mã code!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (TypeComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn loại giảm giá!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(DiscountPercentTextBox.Text, out decimal discountValue) || discountValue <= 0)
                {
                    MessageBox.Show("Vui lòng nhập giá trị giảm hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                // Create updated discount object
                var updatedDiscount = new DiscountViewModel
                {
                    Id = _discount.Id,
                    Code = CodeTextBox.Text.Trim(),
                    CourseName = _discount.CourseName,
                    Type = TypeComboBox.Text,
                    DiscountPercent = discountValue + "%",
                    StartDate = StartDatePicker.SelectedDate.Value.ToString("dd/MM/yyyy"),
                    EndDate = EndDatePicker.SelectedDate.Value.ToString("dd/MM/yyyy"),
                    Status = _discount.Status
                };

                // Update discount
                bool success = await _discountService.UpdateDiscountAsync(
    _discount.Id,
    CodeTextBox.Text.Trim(),
    _discount.CourseName, // ⚠️ Cần sửa thành CourseId, nếu có CourseId!
    (int)discountValue,
    StartDatePicker.SelectedDate.Value,
    EndDatePicker.SelectedDate.Value
);


                if (success)
                {
                    MessageBox.Show("Cập nhật mã giảm giá thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Có lỗi xảy ra khi cập nhật mã giảm giá!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
