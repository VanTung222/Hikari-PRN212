using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HikariDataAccess.Entities;

namespace HikariApp.UserControls
{
    public partial class CourseCard : UserControl
    {
        public Course? Course { get; private set; }
        
        // Events
        public event EventHandler<Course>? AddToCartClicked;
        public event EventHandler<Course>? ContinueLearningClicked;

        public CourseCard()
        {
            InitializeComponent();
        }

        public void SetCourse(Course course)
        {
            Course = course;
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (Course == null) return;

            // Update course info
            TitleText.Text = Course.Title ?? "Không có tiêu đề";
            DescriptionText.Text = Course.Description ?? "Không có mô tả";
            FeeText.Text = Course.Fee?.ToString("N0") + " VNĐ" ?? "Miễn phí";
            DurationText.Text = Course.Duration?.ToString() + " tuần" ?? "Không xác định";

            // Update icon and colors based on course title
            UpdateCourseIcon();
        }

        private void UpdateCourseIcon()
        {
            var title = Course?.Title ?? "";
            
            if (title.Contains("Nhật"))
            {
                IconText.Text = "🏩";
                IconBorder.Background = new SolidColorBrush(Color.FromRgb(255, 243, 224));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(245, 124, 0));
            }
            else if (title.Contains("C#") || title.Contains("Programming"))
            {
                IconText.Text = "💻";
                IconBorder.Background = new SolidColorBrush(Color.FromRgb(227, 242, 253));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(25, 118, 210));
            }
            else if (title.Contains("Web"))
            {
                IconText.Text = "🌐";
                IconBorder.Background = new SolidColorBrush(Color.FromRgb(232, 245, 232));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80));
            }
            else if (title.Contains("Data") || title.Contains("Python"))
            {
                IconText.Text = "📊";
                IconBorder.Background = new SolidColorBrush(Color.FromRgb(253, 231, 243));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(233, 30, 99));
            }
            else if (title.Contains("Mobile"))
            {
                IconText.Text = "📱";
                IconBorder.Background = new SolidColorBrush(Color.FromRgb(243, 229, 245));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(156, 39, 176));
            }
            else if (title.Contains("Security"))
            {
                IconText.Text = "🔒";
                IconBorder.Background = new SolidColorBrush(Color.FromRgb(255, 235, 238));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(220, 53, 69));
            }
            else
            {
                IconText.Text = "📚";
                IconBorder.Background = new SolidColorBrush(Color.FromRgb(227, 242, 253));
                IconText.Foreground = new SolidColorBrush(Color.FromRgb(25, 118, 210));
            }
        }

        public void SetButtonState(CourseButtonState state)
        {
            switch (state)
            {
                case CourseButtonState.AddToCart:
                    ActionButton.Content = "🛒 Thêm vào giỏ";
                    ActionButton.Style = (Style)FindResource("AddToCartButtonStyle");
                    ActionButton.IsEnabled = true;
                    break;
                    
                case CourseButtonState.InCart:
                    ActionButton.Content = "✅ Đã thêm vào giỏ";
                    ActionButton.Style = (Style)FindResource("AddToCartButtonStyle");
                    ActionButton.IsEnabled = false;
                    break;
                    
                case CourseButtonState.ContinueLearning:
                    ActionButton.Content = "📚 Tiếp tục học";
                    ActionButton.Style = (Style)FindResource("ContinueLearningButtonStyle");
                    ActionButton.IsEnabled = true;
                    break;
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Course == null) return;

            if (ActionButton.Content.ToString().Contains("Thêm vào giỏ"))
            {
                AddToCartClicked?.Invoke(this, Course);
            }
            else if (ActionButton.Content.ToString().Contains("Tiếp tục học"))
            {
                ContinueLearningClicked?.Invoke(this, Course);
            }
        }
    }

    public enum CourseButtonState
    {
        AddToCart,
        InCart,
        ContinueLearning
    }
}
