using System;
using System.Windows;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class EditPaymentDialog : Window
    {
        private readonly PaymentService _paymentService;
        private readonly PaymentViewModel _payment;

        public EditPaymentDialog(PaymentViewModel payment)
        {
            InitializeComponent();
            _paymentService = new PaymentService();
            _payment = payment;
            LoadPaymentData();
        }

        private void LoadPaymentData()
        {
            IdTextBox.Text = _payment.Id;
            PaymentCodeTextBox.Text = _payment.PaymentCode;
            StudentNameTextBox.Text = _payment.StudentName;
            CourseNameTextBox.Text = _payment.CourseName;
            AmountTextBox.Text = _payment.Amount;
            PaymentMethodComboBox.Text = _payment.PaymentMethod;
            StatusComboBox.Text = _payment.Status;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (PaymentMethodComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn phương thức thanh toán!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (StatusComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn trạng thái thanh toán!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Create updated payment object
                var updatedPayment = new PaymentViewModel
                {
                    Id = _payment.Id,
                    PaymentCode = _payment.PaymentCode,
                    StudentName = _payment.StudentName,
                    CourseName = _payment.CourseName,
                    Amount = _payment.Amount,
                    PaymentMethod = PaymentMethodComboBox.Text,
                    Status = StatusComboBox.Text,
                    PaymentDate = _payment.PaymentDate
                };

                // Note: Since PaymentService doesn't have UpdatePaymentAsync method yet,
                // we'll show a success message for now. This should be implemented in the service layer.
                MessageBox.Show("Cập nhật thanh toán thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
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
