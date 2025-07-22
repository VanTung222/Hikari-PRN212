using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class ManageTest : UserControl
    {
        public ObservableCollection<Test> Tests { get; set; }
        private readonly TestService _testService;

        public ManageTest()
        {
            InitializeComponent();
            _testService = new TestService();
            Tests = new ObservableCollection<Test>();
            TestList.ItemsSource = Tests;
            LoadTests();
            LoadStatistics();
        }

        private void LoadTests()
        {
            try
            {
                Tests.Clear();
                var tests = _testService.GetAllTests();
                foreach (var test in tests)
                {
                    Tests.Add(test);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách test: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadStatistics()
        {
            try
            {
                var tests = _testService.GetAllTests();
                txtTotalTests.Text = tests.Count.ToString();
                txtActiveTests.Text = tests.Count(t => (bool)t.IsActive).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thống kê: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateTest_Click(object sender, RoutedEventArgs e)
        {
            var createWindow = new CreateTest();
            createWindow.WindowState = WindowState.Maximized;
            if (createWindow.ShowDialog() == true && createWindow.IsSuccess)
            {
                LoadTests();
                LoadStatistics();
            }
        }

        private void EditTest_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Test test = button?.DataContext as Test;

            if (test != null)
            {
                var editWindow = new CreateTest(test);
                editWindow.WindowState = WindowState.Maximized;
                if (editWindow.ShowDialog() == true && editWindow.IsSuccess)
                {
                    LoadTests();
                    LoadStatistics();
                }
            }
        }

        private void ViewResults_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Test test = button?.DataContext as Test;

            if (test != null)
            {
                // Lấy danh sách câu hỏi của test
                var questionService = new HikariBusiness.TeacherService.QuestionService();
                var questions = questionService.GetQuestionsByTestId(test.Id);
                var viewTestWindow = new ViewTest(test, questions);
                viewTestWindow.WindowState = WindowState.Maximized;
                viewTestWindow.ShowDialog();
            }
        }

        private void DeleteTest_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Test test = button?.DataContext as Test;

            if (test != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa test '{test.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _testService.DeleteTest(test.Id);
                        MessageBox.Show("Xóa test thành công!", "Thành công",
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadTests();
                        LoadStatistics();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
