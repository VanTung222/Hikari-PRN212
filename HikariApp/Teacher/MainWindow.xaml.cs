using System;
using System.Windows;
using System.Windows.Controls;


namespace HikariApp.Teacher
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Load default view - Dashboard or Course Management
            LoadView(new ManageCourse());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;
            string buttonContent = clickedButton.Content.ToString();

            // Remove emoji and get clean text
            string cleanContent = buttonContent.Replace("📚 ", "")
                                              .Replace("📝 ", "")
                                              .Replace("📄 ", "")
                                              .Replace("📈 ", "");

            switch (cleanContent)
            {
                case "Quản Lý Khóa Học":
                    LoadView(new ManageCourse());
                    break;
                case "Quản Lý Test":
                    LoadView(new ManageTest());
                    break;
                case "Quản Lý Tài Liệu":
                    LoadView(new ManageDocument());
                    break;
                case "Quản Lý Tiến Độ":
                    LoadView(new ManageProgress());
                    break;
            }
        }

        private void LoadView(UserControl view)
        {
            MainContent.Content = view;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ?
                         WindowState.Normal : WindowState.Maximized;
        }

        private void CloseApp_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát ứng dụng?",
                "Xác nhận thoát",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        // Handle window dragging
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}