using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class ViewLesson : Window
    {
        private Lesson _lesson;
        private readonly LessonService _lessonService;
        private DispatcherTimer _timer;
        private bool _isPlaying = false;
        private bool _isDragging = false;

        public ViewLesson(Lesson lesson)
        {
            InitializeComponent();
            _lesson = lesson;
            _lessonService = new LessonService();

            InitializeTimer();
            LoadLessonData();
            LoadVideo();
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
        }

        private void LoadLessonData()
        {
            txtLessonTitle.Text = _lesson.Title;
            txtCourseTitle.Text = $"Khóa học: {_lesson.CourseId}";
            txtDescription.Text = _lesson.Description ?? "Không có mô tả";
            txtDuration.Text = $"{_lesson.Duration} phút";

            // Update status
            if ((bool)_lesson.IsCompleted)
            {
                txtStatus.Text = "Đã hoàn thành";
                StatusBorder.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                txtStatus.Text = "Chưa hoàn thành";
                StatusBorder.Background = System.Windows.Media.Brushes.Orange;
            }
        }

        private void LoadVideo()
        {
            try
            {
                if (!string.IsNullOrEmpty(_lesson.MediaUrl))
                {
                    string videoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _lesson.MediaUrl);
                    if (File.Exists(videoPath))
                    {
                        VideoPlayer.Source = new Uri(videoPath);
                        txtMessage.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txtMessage.Text = "Không tìm thấy file video!";
                        txtMessage.Foreground = System.Windows.Media.Brushes.Red;
                    }
                }
                else
                {
                    txtMessage.Text = "Bài học này không có video";
                    txtMessage.Foreground = System.Windows.Media.Brushes.Yellow;
                }
            }
            catch (Exception ex)
            {
                txtMessage.Text = $"Lỗi tải video: {ex.Message}";
                txtMessage.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                txtTotalTime.Text = FormatTime(VideoPlayer.NaturalDuration.TimeSpan);
                txtMessage.Visibility = Visibility.Collapsed;
            }
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _isPlaying = false;
            btnPlayPause.Content = "▶️";
            _timer.Stop();
            ProgressSlider.Value = 0;
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                VideoPlayer.Pause();
                btnPlayPause.Content = "▶️";
                _timer.Stop();
            }
            else
            {
                VideoPlayer.Play();
                btnPlayPause.Content = "⏸️";
                _timer.Start();
            }
            _isPlaying = !_isPlaying;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isDragging && VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Value = VideoPlayer.Position.TotalSeconds;
                txtCurrentTime.Text = FormatTime(VideoPlayer.Position);
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDragging)
            {
                VideoPlayer.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            VideoPlayer.Volume = VolumeSlider.Value;
        }

        private string FormatTime(TimeSpan timeSpan)
        {
            return $"{(int)timeSpan.TotalMinutes:D2}:{timeSpan.Seconds:D2}";
        }

        private void MarkComplete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _lesson.IsCompleted = true;
                _lessonService.UpdateLesson(_lesson);

                txtStatus.Text = "Đã hoàn thành";
                StatusBorder.Background = System.Windows.Media.Brushes.Green;

                MessageBox.Show("Đã đánh dấu bài học hoàn thành!", "Thành công",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveNotes_Click(object sender, RoutedEventArgs e)
        {
            // Implement save notes functionality
            MessageBox.Show("Ghi chú đã được lưu!", "Thành công",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PreviousLesson_Click(object sender, RoutedEventArgs e)
        {
            // Implement previous lesson navigation
            MessageBox.Show("Chức năng đang phát triển!", "Thông báo",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void NextLesson_Click(object sender, RoutedEventArgs e)
        {
            // Implement next lesson navigation
            MessageBox.Show("Chức năng đang phát triển!", "Thông báo",
                           MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GoHome_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            VideoPlayer.Stop();
            VideoPlayer.Close();
            _timer?.Stop();
            base.OnClosed(e);
        }
    }
}