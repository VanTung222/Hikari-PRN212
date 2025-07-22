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

namespace HikariApp
{
    /// <summary>
    /// Interaction logic for QuizResultPage.xaml
    /// </summary>
    public partial class QuizResultPage : Window
    {
        public QuizResultPage(string testName, decimal userScore, decimal totalMarks, int correctQuestions, int totalQuestions)
        {
            InitializeComponent();
            DisplayResults(testName, userScore, totalMarks, correctQuestions, totalQuestions);
        }

        private void DisplayResults(string testName, decimal userScore, decimal totalMarks, int correctQuestions, int totalQuestions)
        {
            TestNameTextBlock.Text = $"Bài kiểm tra: {testName}";
            ScoreTextBlock.Text = $"Điểm của bạn: {userScore}/{totalMarks}";
            CorrectAnswersTextBlock.Text = $"Số câu đúng: {correctQuestions}/{totalQuestions}";
            TotalQuestionsTextBlock.Text = $"Tổng số câu: {totalQuestions}";

            // Tùy chỉnh màu sắc dựa trên điểm số (tùy chọn)
            if (userScore >= (totalMarks * 0.7m)) // Ví dụ: trên 70% là Đạt
            {
                ScoreTextBlock.Foreground = (SolidColorBrush)FindResource("SuccessColor");
            }
            else
            {
                ScoreTextBlock.Foreground = (SolidColorBrush)FindResource("DangerColor");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
