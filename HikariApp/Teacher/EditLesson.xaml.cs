using System.Windows;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class EditLesson : Window
    {
        private readonly Lesson _lesson;
        private readonly LessonService _lessonService = new LessonService();
        public bool IsSuccess { get; private set; }

        public EditLesson(Lesson lesson)
        {
            InitializeComponent();
            _lesson = lesson;
            LoadLessonData();
        }

        private void LoadLessonData()
        {
            txtCourseID.Text = _lesson.CourseId;
            txtTitle.Text = _lesson.Title;
            txtDescription.Text = _lesson.Description;
            txtDuration.Text = _lesson.Duration?.ToString() ?? "";
            txtYoutubeUrl.Text = _lesson.MediaUrl;
            chkIsActive.IsChecked = _lesson.IsActive ?? false;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên bài học!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitle.Focus();
                return;
            }
            if (!int.TryParse(txtDuration.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Vui lòng nhập thời lượng hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDuration.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtYoutubeUrl.Text))
            {
                MessageBox.Show("Vui lòng nhập link YouTube cho bài học!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtYoutubeUrl.Focus();
                return;
            }
            // Cập nhật lại lesson
            _lesson.Title = txtTitle.Text;
            _lesson.Description = txtDescription.Text;
            _lesson.Duration = duration;
            _lesson.MediaUrl = txtYoutubeUrl.Text.Trim();
            _lesson.IsActive = chkIsActive.IsChecked ?? false;
            _lessonService.UpdateLesson(_lesson);
            IsSuccess = true;
            MessageBox.Show("Cập nhật bài học thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
} 