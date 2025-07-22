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
using HikariBusiness.Services;
using HikariDataAccess.Entities;

namespace HikariApp
{
    /// <summary>
    /// Interaction logic for QuizPage.xaml
    /// </summary>
    public partial class QuizPage : Window
    {
        private int _testId;
        private List<Question> _questions;
        private int _currentQuestionIndex = 0;
        private Dictionary<int, char> _userAnswers = new Dictionary<int, char>(); // Lưu trữ đáp án người dùng (QuestionId, SelectedOption)
        private Test _currentTest; // Để lưu thông tin về bài test (title, totalQuestions)

        public QuizPage(int testId)
        {
            InitializeComponent();
            _testId = testId;
            LoadQuizData();
        }

        private void LoadQuizData()
        {
            try
            {
                TestManager testManager = new TestManager();
                _currentTest = testManager.GetTestDetails(_testId);

                if (_currentTest == null)
                {
                    MessageBox.Show("Không tìm thấy bài kiểm tra này.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
                }

                QuestionManager questionManager = new QuestionManager();
                _questions = questionManager.GetTestQuestions(_testId);

                if (_questions == null || _questions.Count == 0)
                {
                    MessageBox.Show("Không có câu hỏi nào cho bài kiểm tra này.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                    return;
                }

                // Cập nhật tiêu đề bài kiểm tra
                TestTitle.Text = _currentTest.Title ?? "Bài kiểm tra";
                TestDescription.Text = _currentTest.Description ?? "Kiểm tra kiến thức của bạn.";

                DisplayQuestion();
                UpdateNavigationButtons();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu bài kiểm tra: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        private void DisplayQuestion()
        {
            if (_questions == null || _questions.Count == 0 || _currentQuestionIndex < 0 || _currentQuestionIndex >= _questions.Count)
            {
                return;
            }

            Question currentQuestion = _questions[_currentQuestionIndex];

            // Hiển thị số câu hỏi
            QuestionNumber.Text = $"Câu hỏi {_currentQuestionIndex + 1}/{_questions.Count}";

            // Hiển thị nội dung câu hỏi
            QuestionText.Text = currentQuestion.QuestionText;

            // Xóa các RadioButton cũ và tạo mới để tránh lỗi trạng thái
            OptionsPanel.Children.Clear();

            // Tạo và hiển thị các lựa chọn
            RadioButton rbA = CreateOptionRadioButton(currentQuestion.OptionA, 'A');
            RadioButton rbB = CreateOptionRadioButton(currentQuestion.OptionB, 'B');
            RadioButton rbC = CreateOptionRadioButton(currentQuestion.OptionC, 'C');
            RadioButton rbD = CreateOptionRadioButton(currentQuestion.OptionD, 'D');

            OptionsPanel.Children.Add(rbA);
            OptionsPanel.Children.Add(rbB);
            OptionsPanel.Children.Add(rbC);
            OptionsPanel.Children.Add(rbD);

            // Kiểm tra và chọn lại đáp án đã chọn của người dùng nếu có
            if (_userAnswers.TryGetValue(currentQuestion.Id, out char savedAnswer))
            {
                switch (savedAnswer)
                {
                    case 'A': rbA.IsChecked = true; break;
                    case 'B': rbB.IsChecked = true; break;
                    case 'C': rbC.IsChecked = true; break;
                    case 'D': rbD.IsChecked = true; break;
                }
            }
            else
            {
                // Đảm bảo không có lựa chọn nào được chọn nếu chưa có đáp án
                rbA.IsChecked = rbB.IsChecked = rbC.IsChecked = rbD.IsChecked = false;
            }

            // Gắn sự kiện Checked cho các RadioButton
            rbA.Checked += Option_Checked;
            rbB.Checked += Option_Checked;
            rbC.Checked += Option_Checked;
            rbD.Checked += Option_Checked;

            UpdateNavigationButtons();
            HideMessage(); // Ẩn thông báo khi chuyển câu hỏi
        }

        private RadioButton CreateOptionRadioButton(string content, char optionChar)
        {
            RadioButton rb = new RadioButton
            {
                Content = content,
                GroupName = "QuizOptions",
                Style = (Style)FindResource("QuizOptionStyle"),
                Tag = optionChar // Lưu ký tự của đáp án vào Tag
            };
            return rb;
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton selectedOption = sender as RadioButton;
            if (selectedOption != null && selectedOption.IsChecked == true)
            {
                char selectedChar = (char)selectedOption.Tag;
                _userAnswers[_questions[_currentQuestionIndex].Id] = selectedChar;
                // Debug: MessageBox.Show($"Câu {_currentQuestionIndex + 1}: Bạn đã chọn {selectedChar}");
            }
        }

        private void UpdateNavigationButtons()
        {
            PreviousButton.IsEnabled = (_currentQuestionIndex > 0);
            NextButton.IsEnabled = (_currentQuestionIndex < _questions.Count - 1);
            SubmitButton.Visibility = (_currentQuestionIndex == _questions.Count - 1) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                _currentQuestionIndex--;
                DisplayQuestion();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex < _questions.Count - 1)
            {
                _currentQuestionIndex++;
                DisplayQuestion();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            int correctAnswers = 0;
            decimal totalMarks = 0;
            decimal userScore = 0;

            foreach (var question in _questions)
            {
                totalMarks += question.Mark ?? 0; // Cộng điểm tối đa của câu hỏi

                if (_userAnswers.TryGetValue(question.Id, out char userAnswer))
                {
                    if (!string.IsNullOrEmpty(question.CorrectOption) && question.CorrectOption.Length == 1 &&
                        char.ToUpper(userAnswer) == char.ToUpper(question.CorrectOption[0])) // So sánh không phân biệt hoa thường
                    {
                        correctAnswers++;
                        userScore += question.Mark ?? 0; // Cộng điểm cho câu trả lời đúng
                    }
                }
            }

            // Hiển thị kết quả (có thể mở một trang mới)
            ShowMessage($"Bạn đã hoàn thành bài kiểm tra! Điểm của bạn: {userScore}/{totalMarks}. Số câu đúng: {correctAnswers}/{_questions.Count}", true);

            // Mở trang kết quả
            QuizResultPage resultPage = new QuizResultPage(_currentTest.Title, userScore, totalMarks, correctAnswers, _questions.Count);
            resultPage.Show();
            this.Close(); // Đóng trang QuizPage hiện tại
        }

        private void ShowMessage(string message, bool isSuccess)
        {
            MessageText.Text = message;
            MessageBorder.Visibility = Visibility.Visible;

            if (isSuccess)
            {
                MessageBorder.Background = (SolidColorBrush)FindResource("SuccessButtonColor"); // Màu xanh
                MessageBorder.BorderBrush = (SolidColorBrush)FindResource("SuccessButtonColor");
                MessageText.Foreground = Brushes.White; // Chữ trắng
            }
            else
            {
                MessageBorder.Background = (SolidColorBrush)FindResource("DangerButtonColor"); // Màu đỏ
                MessageBorder.BorderBrush = (SolidColorBrush)FindResource("DangerButtonColor");
                MessageText.Foreground = Brushes.White; // Chữ trắng
            }
        }

        private void HideMessage()
        {
            MessageBorder.Visibility = Visibility.Collapsed;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng QuizPage và quay lại trang trước (TestListPage)
        }
    }
}
