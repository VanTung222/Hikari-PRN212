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
using System.Text.RegularExpressions;
using Microsoft.Web.WebView2.Core;
using HikariBusiness.Services;
using HikariDataAccess.Entities;

namespace HikariApp
{
    /// <summary>
    /// Interaction logic for CourseDetailPage.xaml
    /// </summary>
    public partial class CourseDetailPage : Window
    {
        private string _courseId;
        private Course _currentCourse;
        private Lesson _currentlyPlayingLesson; // To track the lesson that is actually playing
        private readonly LessonManager _lessonManager;
        private readonly ProgressManager _progressManager;

        public CourseDetailPage(string courseId)
        {
            InitializeComponent();
            _courseId = courseId;
            _lessonManager = new LessonManager();
            _progressManager = new ProgressManager(); // Initialize progress manager
            LoadCourseDetails();

            // Gán Volume cho MediaElement
            LessonMediaElement.Volume = VolumeSlider.Value;
            InitializeWebView2();
        }

        private async void InitializeWebView2()
        {
            await YouTubeWebView.EnsureCoreWebView2Async(null);
        }

        private void LoadCourseDetails()
        {
            try
            {
                CourseManager courseManager = new CourseManager();
                _currentCourse = courseManager.GetCourseById(_courseId);

                if (_currentCourse == null)
                {
                    MessageBox.Show("Không tìm thấy khóa học này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                // Hiển thị thông tin khóa học
                CourseTitleTextBlock.Text = _currentCourse.Title;
                CourseDescriptionTextBlock.Text = _currentCourse.Description;

                // Tải danh sách bài học
                string studentId = AppSession.CurrentStudentId;
                List<Lesson> lessons = _lessonManager.GetLessonsForCourse(_courseId, studentId);

                LessonsListView.ItemsSource = lessons;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chi tiết khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void LessonsListView_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (LessonsListView.SelectedItem is Lesson selectedLesson)
            {
                _currentlyPlayingLesson = selectedLesson; // Set the currently playing lesson

                if (selectedLesson != null && !string.IsNullOrEmpty(selectedLesson.MediaUrl))
                {
                    try
                    {
                        if (IsYouTubeUrl(selectedLesson.MediaUrl))
                        {
                            PlayYouTubeVideoInWebView(selectedLesson.MediaUrl);
                        }
                        else
                        {
                            PlayLocalVideo(selectedLesson.MediaUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Không thể phát video: {ex.Message}", "Lỗi phát video", MessageBoxButton.OK, MessageBoxImage.Error);
                        ResetVideoDisplay();
                    }
                }
                else
                {
                    MessageBox.Show("Bài học này không có video để xem.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetVideoDisplay();
                }
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            LessonMediaElement.Play();
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            LessonMediaElement.Pause();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            LessonMediaElement.Stop();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (LessonMediaElement != null)
            {
                LessonMediaElement.Volume = VolumeSlider.Value;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng CourseDetailPage và quay lại trang CourseListPage
        }

        private void ExerciseIcon_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Lesson lesson)
            {
                if (lesson.Exercise != null)
                {
                    string currentStudentId = AppSession.CurrentStudentId;
                    string currentEnrollmentId = AppSession.CurrentEnrollmentId;

                    var exerciseWindow = new ExerciseWindow(lesson.Exercise, currentStudentId, currentEnrollmentId);
                    exerciseWindow.Owner = this;
                    exerciseWindow.ShowDialog();

                    // Ngăn sự kiện click lan ra các control cha (như ListViewItem)
                    e.Handled = true; 
                }
            }
        }

        // Helper method to check if URL is a YouTube link
        private bool IsYouTubeUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return url.Contains("youtube.com") || url.Contains("youtu.be");
        }

        private void LessonMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            // Use the lesson that was actually playing, not the currently selected one
            if (_currentlyPlayingLesson is Lesson finishedLesson)
            {
                // Step 1: Mark the lesson as completed for watching the video
                string studentId = AppSession.CurrentStudentId;
                string enrollmentId = AppSession.CurrentEnrollmentId;
                _progressManager.MarkLessonAsCompleted(studentId, finishedLesson.Id, enrollmentId);

                // Step 2: If there's an exercise, prompt the user to do it
                if (finishedLesson.Exercise != null)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "Bạn đã hoàn thành bài học. Bạn có muốn làm bài tập ngay bây giờ không?",
                        "Xác nhận",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var exerciseWindow = new ExerciseWindow(finishedLesson.Exercise, studentId, enrollmentId);
                        exerciseWindow.Owner = this;
                        exerciseWindow.ShowDialog();
                    }
                }
            }
        }

        private void PlayLocalVideo(string mediaUrl)
        {
            ResetVideoDisplay();
            try
            {
                LessonMediaElement.Source = new Uri(mediaUrl);
                LessonMediaElement.Visibility = Visibility.Visible;
                MediaControls.Visibility = Visibility.Visible;
                LessonMediaElement.Play();
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Địa chỉ video không hợp lệ.", "Lỗi định dạng", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void PlayYouTubeVideoInWebView(string youtubeUrl)
        {
            ResetVideoDisplay();
            try
            {
                string embedUrl = ConvertToEmbedUrl(youtubeUrl);
                if (YouTubeWebView != null && YouTubeWebView.CoreWebView2 != null)
                {
                    YouTubeWebView.CoreWebView2.Navigate(embedUrl);
                    YouTubeWebView.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Trình phát video chưa sẵn sàng. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải video YouTube: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ConvertToEmbedUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            // Regular expression to extract video ID from various YouTube URL formats
            var regex = new Regex(@"(?:https?://)?(?:www\.)?(?:youtube\.com\/(?:watch\?v=|embed\/|v\/)|youtu\.be\/)([^\s&?#<"">']{11})");
            var match = regex.Match(url);

            if (match.Success)
            {
                string videoId = match.Groups[1].Value;
                return $"https://www.youtube.com/embed/{videoId}?autoplay=1";
            }

            // Return original URL if no match is found
            return url;
        }


        // Helper method to reset video display to default state
        private void ResetVideoDisplay()
        {
            LessonMediaElement.Source = null;
            NoVideoText.Text = "Chọn một bài học để xem video";
            NoVideoText.Visibility = Visibility.Visible;
        }
    }
}
