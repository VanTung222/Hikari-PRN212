using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class ManageCourse : UserControl
    {
        public ObservableCollection<Course> Courses { get; set; }
        private readonly CourseService _courseService;

        public ManageCourse()
        {
            InitializeComponent();
            _courseService = new CourseService();
            Courses = new ObservableCollection<Course>();
            CourseList.ItemsSource = Courses;
            LoadCourses();
        }

        private void LoadCourses()
        {
            try
            {
                Courses.Clear();
                var courses = _courseService.GetAllCourses();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa học: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCourse_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditCourse();
            addWindow.WindowState = WindowState.Maximized;
            if (addWindow.ShowDialog() == true && addWindow.IsSuccess)
            {
                LoadCourses();
            }
        }

        private void EditCourse_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Course course = button?.DataContext as Course;

            if (course != null)
            {
                var editWindow = new AddEditCourse(course);
                editWindow.WindowState = WindowState.Maximized;
                if (editWindow.ShowDialog() == true && editWindow.IsSuccess)
                {
                    LoadCourses();
                }
            }
        }

        private void ViewCourse_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Course course = button?.DataContext as Course;

            if (course != null)
            {
                var viewWindow = new CourseDetail(course.CourseId);
                viewWindow.WindowState = WindowState.Maximized;
                viewWindow.ShowDialog();
            }
        }

        private void DeleteCourse_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Course course = button?.DataContext as Course;

            if (course != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa khóa học '{course.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _courseService.DeleteCourse(course.CourseId);
                        MessageBox.Show("Xóa khóa học thành công!", "Thành công",
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCourses();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string keyword = SearchTextBox.Text.Trim();
            if (string.IsNullOrEmpty(keyword) || keyword == "Tìm kiếm khóa học...")
            {
                LoadCourses();
                return;
            }
            try
            {
                Courses.Clear();
                var courses = _courseService.GetAllCourses()
                    .Where(c => !string.IsNullOrEmpty(c.Title) && c.Title.IndexOf(keyword, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                foreach (var course in courses)
                {
                    Courses.Add(course);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == "Tìm kiếm khóa học...")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Tìm kiếm khóa học...";
                tb.Foreground = Brushes.Gray;
            }
        }
    }
}