using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HikariBusiness.Services;
using HikariDataAccess.Entities;

namespace HikariApp
{
    /// <summary>
    /// Interaction logic for TestListPage.xaml
    /// </summary>
    public partial class TestListPage : Window
    {
        public TestListPage()
        {
            InitializeComponent();
            LoadTestData();
        }

        private void LoadTestData()
        {
            try
            {
                TestManager testManager = new TestManager();
                List<Test> tests = testManager.GetAvailableTests();
                TestListView.ItemsSource = tests;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Không thể tải danh sách bài kiểm tra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DoTestButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                int testId = (int)btn.Tag;
                MessageBox.Show($"Bạn đã chọn làm bài kiểm tra có ID: {testId}", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
