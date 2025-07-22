using System.Windows;
using System.Windows.Media;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    public partial class DiscountDetailsDialog : Window
    {
        public DiscountDetailsDialog(DiscountViewModel discount)
        {
            InitializeComponent();
            LoadDiscountDetails(discount);
        }

        private void LoadDiscountDetails(DiscountViewModel discount)
        {
            IdTextBlock.Text = discount.Id;
            CodeTextBlock.Text = discount.Code;
            CourseNameTextBlock.Text = discount.CourseName;
            TypeTextBlock.Text = discount.Type;
            DiscountPercentTextBlock.Text = discount.DiscountPercent;
            StartDateTextBlock.Text = discount.StartDate;
            EndDateTextBlock.Text = discount.EndDate;

            // Set status with color
            StatusTextBlock.Text = discount.Status;
            StatusBorder.Background = GetStatusColor(discount.Status);
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
