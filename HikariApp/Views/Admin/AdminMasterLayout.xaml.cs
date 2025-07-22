using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace HikariApp.Views.Admin
{
    /// <summary>
    /// Interaction logic for AdminMasterLayout.xaml
    /// </summary>
    public partial class AdminMasterLayout : Window
    {
        public AdminMasterLayout()
        {
            InitializeComponent();
            // Mặc định vào trang Dashboard
            MainContentFrame.Navigate(new DashboardPage());
            SetActiveButton(DashboardBtn, "📊 Bảng Điều Khiển");
        }

        private void NavigateToPage(object sender, RoutedEventArgs e)
        {
            if (sender is Button clickedButton)
            {
                switch (clickedButton.Name)
                {
                    case "DashboardBtn":
                        MainContentFrame.Navigate(new DashboardPage());
                        SetActiveButton(clickedButton, "📊 Bảng Điều Khiển");
                        break;

                    case "AccountManagementBtn":
                        MainContentFrame.Navigate(new AccountManagementPage());
                        SetActiveButton(clickedButton, "👥 Quản Lý Tài Khoản");
                        break;

                    case "CourseManagementBtn":
                        MainContentFrame.Navigate(new CourseManagementPage());
                        SetActiveButton(clickedButton, "📚 Quản Lý Khóa Học");
                        break;

                    case "PaymentManagementBtn":
                        MainContentFrame.Navigate(new PaymentManagementPage());
                        SetActiveButton(clickedButton, "💰 Quản Lý Thanh Toán");
                        break;

                    case "ReviewManagementBtn":
                        MainContentFrame.Navigate(new ReviewManagementPage());
                        SetActiveButton(clickedButton, "⭐ Quản Lý Đánh Giá");
                        break;

                    case "DiscountManagementBtn":
                        MainContentFrame.Navigate(new DiscountManagementPage());
                        SetActiveButton(clickedButton, "🏷️ Quản Lý Giảm Giá");
                        break;
                }
            }
        }

        private void SetActiveButton(Button activeButton, string title)
        {
            ResetButtonStyles();
            activeButton.Style = (Style)FindResource("ActiveSidebarButtonStyle");
            PageTitle.Text = title;
        }

        private void ResetButtonStyles()
        {
            Style normalStyle = (Style)FindResource("SidebarButtonStyle");

            DashboardBtn.Style = normalStyle;
            AccountManagementBtn.Style = normalStyle;
            CourseManagementBtn.Style = normalStyle;
            PaymentManagementBtn.Style = normalStyle;
            ReviewManagementBtn.Style = normalStyle;
            DiscountManagementBtn.Style = normalStyle;
        }
    }
}
