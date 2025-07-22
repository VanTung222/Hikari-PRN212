using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using DataAccessLayer.Entities;
using System.Web;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;

namespace HikariApp.Teacher
{
    public partial class LessonView : Window
    {
        private readonly Lesson _lesson;
        public LessonView(Lesson lesson)
        {
            InitializeComponent();
            _lesson = lesson;
            LoadLessonData();
        }

        private void LoadLessonData()
        {
            txtLessonTitle.Text = _lesson.Title;
            txtDescription.Text = _lesson.Description ?? "Không có mô tả";
            txtDuration.Text = _lesson.Duration.HasValue ? $"{_lesson.Duration} phút" : "N/A";
            txtStatus.Text = (_lesson.IsCompleted ?? false) ? "Đã hoàn thành" : "Chưa hoàn thành";
    
            // Hiển thị video YouTube nếu có
            if (!string.IsNullOrWhiteSpace(_lesson.MediaUrl) && (_lesson.MediaUrl.Contains("youtube.com") || _lesson.MediaUrl.Contains("youtu.be")))
            {
                string embedUrl = ConvertToEmbedUrl(_lesson.MediaUrl);
                youtubePlayer.NavigationCompleted += YoutubePlayer_NavigationCompleted;
                youtubePlayer.Source = new System.Uri(embedUrl);
            }
        }

        private void YoutubePlayer_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (!e.IsSuccess)
            {
                // Hiển thị thông báo lỗi đẹp trên UI
                youtubePlayer.Visibility = Visibility.Collapsed;
                MessageBox.Show($"Không thể tải video YouTube (mã lỗi: {e.WebErrorStatus}).\nCó thể do mạng hoặc YouTube chặn nhúng.", "Lỗi video", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ConvertToEmbedUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                string videoId = query["v"];
                if (!string.IsNullOrEmpty(videoId))
                    return $"https://www.youtube.com/embed/{videoId}";
                // Nếu là dạng youtu.be/xxxx
                if (uri.Host.Contains("youtu.be"))
                    return $"https://www.youtube.com/embed{uri.AbsolutePath}";
            }
            catch { }
            return url;
        }

        private void YoutubeUrl_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_lesson.MediaUrl))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(_lesson.MediaUrl) { UseShellExecute = true });
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
} 