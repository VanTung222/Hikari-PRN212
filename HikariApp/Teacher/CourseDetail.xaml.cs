using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class CourseDetail : Window
    {
        private string _courseId;
        private readonly CourseService _courseService;
        private readonly LessonService _lessonService;
        private Course _course;
        public ObservableCollection<Lesson> Lessons { get; set; }

        public CourseDetail(string courseId)
        {
            InitializeComponent();
            _courseId = courseId;
            _courseService = new CourseService();
            _lessonService = new LessonService();
            Lessons = new ObservableCollection<Lesson>();
            LessonsList.ItemsSource = Lessons;

            LoadCourseDetails();
            LoadLessons();
        }

        private void LoadCourseDetails()
        {
            try
            {
                _course = _courseService.GetCourseById(_courseId);

                if (_course != null)
                {
                    txtCourseTitle.Text = $"CHI TIẾT KHÓA HỌC - {_course.Title}";
                    txtCourseID.Text = _course.CourseId;
                    txtFee.Text = $"{_course.Fee:N0} VNĐ";
                    txtDuration.Text = $"{_course.Duration} giờ";
                    txtStartDate.Text = _course.StartDate.HasValue ? _course.StartDate.Value.ToDateTime(new TimeOnly(0, 0)).ToString("dd/MM/yyyy") : "N/A";
                    txtEndDate.Text = _course.EndDate.HasValue ? _course.EndDate.Value.ToDateTime(new TimeOnly(0, 0)).ToString("dd/MM/yyyy") : "N/A";
                    txtStatus.Text = (bool)_course.IsActive ? "Đang hoạt động" : "Không hoạt động";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin khóa học: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLessons()
        {
            try
            {
                Lessons.Clear();
                var lessons = _lessonService.GetLessonsByCourseId(_courseId);
                foreach (var lesson in lessons)
                {
                    Lessons.Add(lesson);
                }
                txtLessonCount.Text = $"{lessons.Count} bài";

            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách bài học: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddLesson_Click(object sender, RoutedEventArgs e)
        {
            var addLessonWindow = new AddLesson(_courseId);
            addLessonWindow.WindowState = WindowState.Maximized;
            if (addLessonWindow.ShowDialog() == true && addLessonWindow.IsSuccess)
            {
                LoadLessons();
            }
        }

        private void ViewLesson_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Lesson lesson = button?.DataContext as Lesson;
            if (lesson != null)
            {
                var viewWindow = new LessonView(lesson);
                viewWindow.WindowState = WindowState.Maximized;
                viewWindow.ShowDialog();
            }
        }

        private void EditLesson_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Lesson lesson = button?.DataContext as Lesson;
            if (lesson != null)
            {
                var editWindow = new EditLesson(lesson);
                editWindow.WindowState = WindowState.Maximized;
                if (editWindow.ShowDialog() == true && editWindow.IsSuccess)
                {
                    LoadLessons();
                }
            }
        }

        private void DeleteLesson_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Lesson lesson = button?.DataContext as Lesson;
            if (lesson != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa bài học '{lesson.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _lessonService.DeleteLesson(lesson.Id);
                    LoadLessons();
                }
            }
        }
    }
}