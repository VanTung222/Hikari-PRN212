using HikariBusiness.Services;
using HikariDataAccess.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace HikariApp
{
    public partial class ExerciseWindow : Window
    {
        private readonly Exercise _exercise;
        private readonly QuestionManager _questionManager;
        private readonly ProgressManager _progressManager;
        private List<Question> _questions;
        private readonly string _studentId;
        private readonly string _enrollmentId;

        // Updated constructor
        public ExerciseWindow(Exercise exercise, string studentId, string enrollmentId)
        {
            InitializeComponent();
            _exercise = exercise;
            _studentId = studentId;
            _enrollmentId = enrollmentId;
            _questionManager = new QuestionManager();
            _progressManager = new ProgressManager();
            LoadExerciseData();
        }

        private void LoadExerciseData()
        {
            ExerciseTitleTextBlock.Text = _exercise.Title;
            ExerciseDescriptionTextBlock.Text = _exercise.Description;

            _questions = _questionManager.GetExerciseQuestions(_exercise.Id);
            if (_questions == null || !_questions.Any())
            {
                MessageBox.Show("Bài tập này chưa có câu hỏi nào.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
                return;
            }

            QuestionsItemsControl.ItemsSource = _questions;
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            decimal score = 0m;
            decimal totalMark = _questions.Sum(q => q.Mark ?? 0m);
            if (totalMark == 0m) totalMark = _questions.Count; // Fallback if marks are not set

            foreach (var question in _questions)
            {
                var selectedRadioButton = FindSelectedRadioButton(question);
                if (selectedRadioButton != null && selectedRadioButton.Tag is string selectedOption)
                {
                    if (selectedOption == question.CorrectOption)
                    {
                        score += question.Mark ?? 1.00m; // Add mark of the question, default to 1 if null
                    }
                }
            }

            decimal finalPercentage = (totalMark > 0) ? (score / totalMark * 100) : 0m;

            // Save the score
            _progressManager.SaveExerciseScore(_studentId, _exercise.LessonId, finalPercentage, _enrollmentId);

            MessageBox.Show($"Điểm của bạn là: {finalPercentage:F2}%. Kết quả đã được lưu.", "Kết quả", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private RadioButton FindSelectedRadioButton(Question question)
        {
            // Lấy container của item trong ItemsControl
            var itemContainer = QuestionsItemsControl.ItemContainerGenerator.ContainerFromItem(question) as FrameworkElement;
            if (itemContainer == null) return null;

            // Tìm các RadioButton bên trong container
            return FindVisualChildren<RadioButton>(itemContainer).FirstOrDefault(rb => rb.IsChecked == true);
        }


        // Helper để tìm các control con trong visual tree
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            { 
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    { 
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
