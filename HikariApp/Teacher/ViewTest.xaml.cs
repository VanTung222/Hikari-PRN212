using System.Collections.Generic;
using System.Windows;
using DataAccessLayer.Entities;

namespace HikariApp.Teacher
{
    public partial class ViewTest : Window
    {
        private readonly Test _test;
        private readonly List<Question> _questions;
        public ViewTest(Test test, List<Question> questions)
        {
            InitializeComponent();
            _test = test;
            _questions = questions;
            LoadTestData();
        }

        private void LoadTestData()
        {
            txtTestTitle.Text = _test.Title;
            txtJlptLevel.Text = $"Cấp độ JLPT: {_test.JlptLevel}";
            txtDescription.Text = _test.Description;
            // Chuẩn bị dữ liệu cho ItemsControl
            var displayQuestions = new List<object>();
            int idx = 1;
            foreach (var q in _questions)
            {
                displayQuestions.Add(new
                {
                    Index = $"Câu {idx}",
                    QuestionText = q.QuestionText,
                    OptionA = q.OptionA,
                    OptionB = q.OptionB,
                    OptionC = q.OptionC,
                    OptionD = q.OptionD,
                    CorrectOption = q.CorrectOption
                });
                idx++;
            }
            QuestionsList.ItemsSource = displayQuestions;
        }
    }
} 