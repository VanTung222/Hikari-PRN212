using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class AddLesson : Window
    {
        private string _courseId;
        private readonly LessonService _lessonService;
        private string _selectedVideoPath;

        public bool IsSuccess { get; private set; }

        public AddLesson(string courseId)
        {
            InitializeComponent();
            _courseId = courseId;
            _lessonService = new LessonService();
            txtCourseID.Text = courseId;
        }

    

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                // Chỉ lấy link YouTube
                string videoUrl = txtYoutubeUrl.Text.Trim();

                Lesson lesson = new Lesson
                {
                    CourseId = _courseId,
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    MediaUrl = videoUrl,
                    Duration = int.Parse(txtDuration.Text),
                    IsCompleted = false,
                    IsActive = chkIsActive.IsChecked.Value
                };

                _lessonService.AddLesson(lesson);
                IsSuccess = true;
                MessageBox.Show("Thêm bài học thành công!", "Thành công",
                               MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên bài học!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitle.Focus();
                return false;
            }

            if (!int.TryParse(txtDuration.Text, out int duration) || duration <= 0)
            {
                MessageBox.Show("Vui lòng nhập thời lượng hợp lệ!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDuration.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtYoutubeUrl.Text))
            {
                MessageBox.Show("Vui lòng nhập link YouTube cho bài học!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtYoutubeUrl.Focus();
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void YoutubeUrl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtYoutubeUrl.Text == "Dán link YouTube vào đây...")
            {
                txtYoutubeUrl.Text = "";
                txtYoutubeUrl.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void YoutubeUrl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtYoutubeUrl.Text))
            {
                txtYoutubeUrl.Text = "Dán link YouTube vào đây...";
                txtYoutubeUrl.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}