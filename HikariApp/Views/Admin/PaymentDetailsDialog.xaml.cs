using System.Windows;
using System.Windows.Media;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class PaymentDetailsDialog : Window
    {
        public PaymentDetailsDialog(PaymentViewModel payment)
        {
            InitializeComponent();
            LoadPaymentDetails(payment);
        }

        private void LoadPaymentDetails(PaymentViewModel payment)
        {
            IdTextBlock.Text = payment.Id;
            PaymentCodeTextBlock.Text = payment.PaymentCode;
            StudentNameTextBlock.Text = payment.StudentName;
            CourseNameTextBlock.Text = payment.CourseName;
            AmountTextBlock.Text = payment.Amount;
            PaymentMethodTextBlock.Text = payment.PaymentMethod;
            PaymentDateTextBlock.Text = payment.PaymentDate;

            // Set status with color
            StatusTextBlock.Text = payment.Status;
            StatusBorder.Background = GetStatusColor(payment.Status);
        }

        private Brush GetStatusColor(string status)
        {
            return status switch
            {
                "Hoàn thành" => new SolidColorBrush(Color.FromRgb(92, 184, 92)), // Green
                "Đang xử lý" => new SolidColorBrush(Color.FromRgb(255, 193, 7)), // Yellow
                "Thất bại" => new SolidColorBrush(Color.FromRgb(220, 53, 69)), // Red
                "Đã hủy" => new SolidColorBrush(Color.FromRgb(108, 117, 125)), // Gray
                _ => new SolidColorBrush(Color.FromRgb(108, 117, 125)) // Gray
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
