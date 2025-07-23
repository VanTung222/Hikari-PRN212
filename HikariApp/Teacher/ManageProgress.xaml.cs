using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class ManageProgress : UserControl
    {
        public ObservableCollection<Student> Students { get; set; }
        private readonly StudentService _studentService;
        private readonly ProgressService _progressService;

        public ManageProgress()
        {
            InitializeComponent();
            _studentService = new StudentService();
            _progressService = new ProgressService();
            Students = new ObservableCollection<Student>();
            ProgressList.ItemsSource = Students;
            LoadStudents();
        }

        private void LoadStudents()
        {
            try
            {
                Students.Clear();
                var students = _studentService.GetAllStudents();
                foreach (var student in students)
                {
                    Students.Add(student);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách học sinh: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Implement export functionality
                MessageBox.Show("Chức năng xuất báo cáo đang được phát triển!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Student student = button?.DataContext as Student;
            if (student != null)
            {
                var progresses = _progressService.GetProgressByStudent(student.StudentId);
                var detailWindow = new ProgressDetailWindow(student, progresses);
                detailWindow.ShowDialog();
            }
        }
    }
}
